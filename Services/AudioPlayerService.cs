using MusicPlayer.Models;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core.Primitives;

namespace MusicPlayer.Services;

/// <summary>
/// 音频播放服务
/// </summary>
public class AudioPlayerService : IDisposable
{
    private MediaElement? _mediaElement;
    private Song? _currentSong;
    private bool _isPlaying;
    private double _currentPosition;

    /// <summary>
    /// 播放模式枚举
    /// </summary>
    public enum PlayMode
    {
        Sequential,  // 顺序播放
        Random,      // 随机播放
        SingleLoop   // 单曲循环
    }

    /// <summary>
    /// 当前播放模式
    /// </summary>
    public PlayMode CurrentPlayMode { get; set; } = PlayMode.Sequential;

    /// <summary>
    /// 当前播放的歌曲
    /// </summary>
    public Song? CurrentSong => _currentSong;

    /// <summary>
    /// 是否正在播放
    /// </summary>
    public bool IsPlaying => _isPlaying;

    /// <summary>
    /// 当前播放位置（秒）
    /// </summary>
    public double CurrentPosition => _currentPosition;

    /// <summary>
    /// 播放状态改变事件
    /// </summary>
    public event EventHandler? PlayStateChanged;

    /// <summary>
    /// 播放位置改变事件
    /// </summary>
    public event EventHandler<double>? PositionChanged;

    /// <summary>
    /// 歌曲结束事件
    /// </summary>
    public event EventHandler? SongEnded;

    /// <summary>
    /// 媒体打开事件（加载完成）
    /// </summary>
    public event EventHandler<double>? MediaOpened;

    private System.Timers.Timer? _positionTimer;

    public AudioPlayerService()
    {
        // 初始化位置更新定时器
        _positionTimer = new System.Timers.Timer(100); // 每100毫秒更新一次
        _positionTimer.Elapsed += (s, e) =>
        {
            if (_isPlaying && _mediaElement != null)
            {
                _currentPosition = _mediaElement.Position.TotalSeconds;
                PositionChanged?.Invoke(this, _currentPosition);
            }
        };
    }

    /// <summary>
    /// 设置媒体元素（需要从UI层传入）
    /// </summary>
    public void SetMediaElement(MediaElement mediaElement)
    {
        _mediaElement = mediaElement;

        // 订阅媒体元素事件
        _mediaElement.MediaEnded += (s, e) =>
        {
            _isPlaying = false;
            SongEnded?.Invoke(this, EventArgs.Empty);
        };

        _mediaElement.StateChanged += (s, e) =>
        {
            _isPlaying = _mediaElement.CurrentState == MediaElementState.Playing;
            PlayStateChanged?.Invoke(this, EventArgs.Empty);
        };

        _mediaElement.MediaOpened += (s, e) =>
        {
            // 媒体加载完成，获取时长
            var duration = _mediaElement.Duration.TotalSeconds;
            if (duration > 0)
            {
                MediaOpened?.Invoke(this, duration);
            }
        };
    }

    /// <summary>
    /// 播放歌曲
    /// </summary>
    public async Task PlayAsync(Song song)
    {
        if (_mediaElement == null) return;

        try
        {
            _currentSong = song;

            // 加载并播放
            _mediaElement.Source = MediaSource.FromFile(song.FilePath);
            _mediaElement.ShouldAutoPlay = true;

            await Task.Delay(200); // 等待加载

            _isPlaying = true;
            _positionTimer?.Start();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"播放失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    public void Pause()
    {
        if (_mediaElement == null || !_isPlaying) return;

        _mediaElement.Pause();
        _isPlaying = false;
        _positionTimer?.Stop();
    }

    /// <summary>
    /// 继续播放
    /// </summary>
    public void Resume()
    {
        if (_mediaElement == null || _isPlaying) return;

        _mediaElement.Play();
        _isPlaying = true;
        _positionTimer?.Start();
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void Stop()
    {
        if (_mediaElement == null) return;

        _mediaElement.Stop();
        _isPlaying = false;
        _currentPosition = 0;
        _positionTimer?.Stop();
    }

    /// <summary>
    /// 跳转到指定位置（秒）
    /// </summary>
    public void SeekTo(double seconds)
    {
        if (_mediaElement == null) return;

        _mediaElement.SeekTo(TimeSpan.FromSeconds(seconds));
        _currentPosition = seconds;

        PositionChanged?.Invoke(this, seconds);
    }

    /// <summary>
    /// 设置音量（0.0 - 1.0）
    /// </summary>
    public void SetVolume(double volume)
    {
        if (_mediaElement == null) return;

        _mediaElement.Volume = Math.Clamp(volume, 0.0, 1.0);
    }

    public void Dispose()
    {
        _positionTimer?.Stop();
        _positionTimer?.Dispose();
        Stop();
    }
}
