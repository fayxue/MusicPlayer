namespace MusicPlayer.Models;

/// <summary>
/// 歌词行模型
/// </summary>
public class LrcLine
{
    /// <summary>
    /// 时间戳（秒）
    /// </summary>
    public double Time { get; set; }

    /// <summary>
    /// 歌词文本
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// 是否为当前高亮行
    /// </summary>
    public bool IsHighlighted { get; set; }
}
