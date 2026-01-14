using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MusicPlayer.Models;
using MusicPlayer.Services;
using System.Collections.ObjectModel;

namespace MusicPlayer.ViewModels;

/// <summary>
/// 主视图模型
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly AudioPlayerService _audioPlayer;
    private readonly LrcParserService _lrcParser;
    private Random _random = new Random();

    [ObservableProperty]
    private ObservableCollection<Song> _playlist = new();

    [ObservableProperty]
    private Song? _currentSong;

    [ObservableProperty]
    private ObservableCollection<LrcLine> _lrcLines = new();

    [ObservableProperty]
    private bool _isPlaying;

    [ObservableProperty]
    private double _currentPosition;

    [ObservableProperty]
    private double _duration;

    [ObservableProperty]
    private string _currentTime = "00:00";

    [ObservableProperty]
    private string _totalTime = "00:00";

    [ObservableProperty]
    private string _playModeIcon = "🔁"; // 顺序播放

    [ObservableProperty]
    private string _playModeText = "顺序播放";

    [ObservableProperty]
    private int _currentSongIndex = -1;

    public MainViewModel(AudioPlayerService audioPlayer, LrcParserService lrcParser)
    {
        _audioPlayer = audioPlayer;
        _lrcParser = lrcParser;

        // 订阅播放器事件
        _audioPlayer.PlayStateChanged += OnPlayStateChanged;
        _audioPlayer.PositionChanged += OnPositionChanged;
        _audioPlayer.SongEnded += OnSongEnded;
        _audioPlayer.MediaOpened += OnMediaOpened;
    }

    /// <summary>
    /// 加载音乐文件
    /// </summary>
    [RelayCommand]
    private async Task LoadMusicFilesAsync()
    {
        try
        {
            // 请求存储权限
            var status = await Permissions.RequestAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                await Application.Current!.MainPage!.DisplayAlert("提示", "需要存储权限才能加载音乐文件", "确定");
                return;
            }

            // 选择音乐文件
            var result = await FilePicker.PickMultipleAsync(new PickOptions
            {
                PickerTitle = "选择音乐文件",
                FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
                {
                    { DevicePlatform.Android, new[] { "audio/mpeg", "audio/mp3", "audio/wav", "audio/flac", "audio/ogg" } },
                    { DevicePlatform.iOS, new[] { "public.mp3", "public.audio" } }
                })
            });

            if (result != null)
            {
                foreach (var file in result)
                {
                    var song = new Song
                    {
                        FilePath = file.FullPath,
                        Title = Path.GetFileNameWithoutExtension(file.FileName),
                        Artist = "未知艺术家",
                        Album = "未知专辑"
                    };

                    // 检查是否存在同名的LRC文件
                    var lrcPath = Path.ChangeExtension(file.FullPath, ".lrc");
                    if (File.Exists(lrcPath))
                    {
                        song.LrcFilePath = lrcPath;
                    }

                    Playlist.Add(song);
                }

                await Application.Current!.MainPage!.DisplayAlert("成功", $"已加载 {result.Count()} 首歌曲", "确定");
            }
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("错误", $"加载音乐文件失败: {ex.Message}", "确定");
        }
    }

    /// <summary>
    /// 播放/暂停
    /// </summary>
    [RelayCommand]
    private async Task PlayPauseAsync()
    {
        if (CurrentSong == null && Playlist.Count > 0)
        {
            // 如果没有当前歌曲，播放第一首
            await PlaySongAsync(Playlist[0]);
            CurrentSongIndex = 0;
        }
        else if (IsPlaying)
        {
            _audioPlayer.Pause();
        }
        else
        {
            _audioPlayer.Resume();
        }
    }

    /// <summary>
    /// 播放指定歌曲
    /// </summary>
    [RelayCommand]
    private async Task PlaySongAsync(Song song)
    {
        try
        {
            CurrentSong = song;
            CurrentSongIndex = Playlist.IndexOf(song);

            await _audioPlayer.PlayAsync(song);

            // Duration 和 TotalTime 会通过 MediaOpened 事件自动设置

            // 加载歌词
            await LoadLrcAsync(song);
        }
        catch (Exception ex)
        {
            await Application.Current!.MainPage!.DisplayAlert("错误", $"播放失败: {ex.Message}", "确定");
        }
    }

    /// <summary>
    /// 上一曲
    /// </summary>
    [RelayCommand]
    private async Task PreviousSongAsync()
    {
        if (Playlist.Count == 0) return;

        int nextIndex;

        if (_audioPlayer.CurrentPlayMode == AudioPlayerService.PlayMode.Random)
        {
            // 随机模式：随机选择一首
            nextIndex = _random.Next(Playlist.Count);
        }
        else
        {
            // 顺序模式和单曲循环：播放上一首
            nextIndex = CurrentSongIndex - 1;
            if (nextIndex < 0)
                nextIndex = Playlist.Count - 1;
        }

        await PlaySongAsync(Playlist[nextIndex]);
    }

    /// <summary>
    /// 下一曲
    /// </summary>
    [RelayCommand]
    private async Task NextSongAsync()
    {
        if (Playlist.Count == 0) return;

        int nextIndex;

        switch (_audioPlayer.CurrentPlayMode)
        {
            case AudioPlayerService.PlayMode.Random:
                // 随机模式
                nextIndex = _random.Next(Playlist.Count);
                break;

            case AudioPlayerService.PlayMode.SingleLoop:
                // 单曲循环：重新播放当前歌曲
                nextIndex = CurrentSongIndex;
                break;

            default: // Sequential
                // 顺序播放
                nextIndex = CurrentSongIndex + 1;
                if (nextIndex >= Playlist.Count)
                    nextIndex = 0;
                break;
        }

        await PlaySongAsync(Playlist[nextIndex]);
    }

    /// <summary>
    /// 切换播放模式
    /// </summary>
    [RelayCommand]
    private void TogglePlayMode()
    {
        _audioPlayer.CurrentPlayMode = _audioPlayer.CurrentPlayMode switch
        {
            AudioPlayerService.PlayMode.Sequential => AudioPlayerService.PlayMode.Random,
            AudioPlayerService.PlayMode.Random => AudioPlayerService.PlayMode.SingleLoop,
            AudioPlayerService.PlayMode.SingleLoop => AudioPlayerService.PlayMode.Sequential,
            _ => AudioPlayerService.PlayMode.Sequential
        };

        UpdatePlayModeUI();
    }

    /// <summary>
    /// 更新播放模式UI
    /// </summary>
    private void UpdatePlayModeUI()
    {
        (PlayModeIcon, PlayModeText) = _audioPlayer.CurrentPlayMode switch
        {
            AudioPlayerService.PlayMode.Sequential => ("🔁", "顺序播放"),
            AudioPlayerService.PlayMode.Random => ("🔀", "随机播放"),
            AudioPlayerService.PlayMode.SingleLoop => ("🔂", "单曲循环"),
            _ => ("🔁", "顺序播放")
        };
    }

    /// <summary>
    /// 拖动进度条
    /// </summary>
    [RelayCommand]
    private void Seek(double position)
    {
        _audioPlayer.SeekTo(position);
    }

    /// <summary>
    /// 加载歌词
    /// </summary>
    private async Task LoadLrcAsync(Song song)
    {
        LrcLines.Clear();

        if (string.IsNullOrEmpty(song.LrcFilePath))
            return;

        try
        {
            var lines = await _lrcParser.ParseLrcFileAsync(song.LrcFilePath);
            foreach (var line in lines)
            {
                LrcLines.Add(line);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"加载歌词失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 播放状态改变事件处理
    /// </summary>
    private void OnPlayStateChanged(object? sender, EventArgs e)
    {
        IsPlaying = _audioPlayer.IsPlaying;
    }

    /// <summary>
    /// 播放位置改变事件处理
    /// </summary>
    private void OnPositionChanged(object? sender, double position)
    {
        CurrentPosition = position;
        CurrentTime = TimeSpan.FromSeconds(position).ToString(@"mm\:ss");

        // 更新歌词高亮
        UpdateLrcHighlight(position);
    }

    /// <summary>
    /// 更新歌词高亮
    /// </summary>
    private void UpdateLrcHighlight(double currentTime)
    {
        var currentIndex = _lrcParser.GetCurrentLrcIndex(LrcLines.ToList(), currentTime);
        _lrcParser.UpdateHighlight(LrcLines.ToList(), currentIndex);

        // 通知UI更新
        OnPropertyChanged(nameof(LrcLines));
    }

    /// <summary>
    /// 媒体打开事件处理
    /// </summary>
    private void OnMediaOpened(object? sender, double duration)
    {
        Duration = duration;
        TotalTime = TimeSpan.FromSeconds(duration).ToString(@"mm\:ss");

        // 更新当前歌曲的时长
        if (CurrentSong != null)
        {
            CurrentSong.Duration = duration;
        }
    }

    /// <summary>
    /// 歌曲结束事件处理
    /// </summary>
    private async void OnSongEnded(object? sender, EventArgs e)
    {
        // 自动播放下一曲
        await NextSongAsync();
    }
}
