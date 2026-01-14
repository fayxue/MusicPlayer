namespace MusicPlayer.Models;

/// <summary>
/// 歌曲模型
/// </summary>
public class Song
{
    /// <summary>
    /// 歌曲文件路径
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// 歌曲名称
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// 艺术家
    /// </summary>
    public string Artist { get; set; } = "未知艺术家";

    /// <summary>
    /// 专辑
    /// </summary>
    public string Album { get; set; } = "未知专辑";

    /// <summary>
    /// 歌曲时长（秒）
    /// </summary>
    public double Duration { get; set; }

    /// <summary>
    /// 歌词文件路径
    /// </summary>
    public string? LrcFilePath { get; set; }

    /// <summary>
    /// 格式化的时长（MM:SS）
    /// </summary>
    public string FormattedDuration => TimeSpan.FromSeconds(Duration).ToString(@"mm\:ss");

    /// <summary>
    /// 歌曲显示名称
    /// </summary>
    public string DisplayName => $"{Title} - {Artist}";
}
