using Microsoft.Extensions.Logging;
using MusicPlayer.Services;
using MusicPlayer.ViewModels;
using CommunityToolkit.Maui;

namespace MusicPlayer;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // 注册服务
        builder.Services.AddSingleton<AudioPlayerService>();
        builder.Services.AddSingleton<LrcParserService>();

        // 注册视图模型
        builder.Services.AddSingleton<MainViewModel>();

        // 注册页面
        builder.Services.AddSingleton<MainPage>();

        return builder.Build();
    }
}
