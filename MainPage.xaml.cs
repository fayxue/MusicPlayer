using MusicPlayer.ViewModels;
using MusicPlayer.Services;

namespace MusicPlayer;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;
    private readonly AudioPlayerService _audioPlayerService;

    public MainPage(MainViewModel viewModel, AudioPlayerService audioPlayerService)
    {
        InitializeComponent();

        _viewModel = viewModel;
        _audioPlayerService = audioPlayerService;
        BindingContext = _viewModel;

        // 将MediaElement传递给AudioPlayerService
        _audioPlayerService.SetMediaElement(MediaPlayer);
    }

    /// <summary>
    /// 点击播放列表按钮，切换播放列表显示/隐藏
    /// </summary>
    private void OnPlaylistClicked(object sender, EventArgs e)
    {
        PlaylistFrame.IsVisible = !PlaylistFrame.IsVisible;
    }

    /// <summary>
    /// 进度条拖动完成事件处理
    /// </summary>
    private void OnProgressSliderDragCompleted(object? sender, EventArgs e)
    {
        if (sender is Slider slider)
        {
            _viewModel.SeekCommand.Execute(slider.Value);
        }
    }
}
