using System.Text.RegularExpressions;
using MusicPlayer.Models;

namespace MusicPlayer.Services;

/// <summary>
/// LRC歌词解析服务
/// </summary>
public class LrcParserService
{
    // LRC时间标签正则表达式：[mm:ss.xx]
    private static readonly Regex TimeTagRegex = new(@"\[(\d{2}):(\d{2})\.(\d{2,3})\]", RegexOptions.Compiled);

    /// <summary>
    /// 解析LRC歌词文件
    /// </summary>
    /// <param name="lrcFilePath">LRC文件路径</param>
    /// <returns>歌词行列表</returns>
    public async Task<List<LrcLine>> ParseLrcFileAsync(string lrcFilePath)
    {
        var lrcLines = new List<LrcLine>();

        try
        {
            if (!File.Exists(lrcFilePath))
                return lrcLines;

            var content = await File.ReadAllTextAsync(lrcFilePath);
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                var parsedLines = ParseLrcLine(line);
                lrcLines.AddRange(parsedLines);
            }

            // 按时间排序
            lrcLines = lrcLines.OrderBy(l => l.Time).ToList();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"解析LRC文件失败: {ex.Message}");
        }

        return lrcLines;
    }

    /// <summary>
    /// 解析单行LRC歌词
    /// </summary>
    /// <param name="line">歌词行</param>
    /// <returns>解析后的歌词行列表（一行可能包含多个时间标签）</returns>
    private List<LrcLine> ParseLrcLine(string line)
    {
        var result = new List<LrcLine>();

        if (string.IsNullOrWhiteSpace(line))
            return result;

        // 查找所有时间标签
        var matches = TimeTagRegex.Matches(line);

        if (matches.Count == 0)
            return result;

        // 提取歌词文本（去除所有时间标签）
        var text = TimeTagRegex.Replace(line, "").Trim();

        // 跳过元数据标签（如：ar:艺术家, ti:标题等）
        if (text.Contains(':') && text.Length < 50)
            return result;

        // 为每个时间标签创建一个歌词行
        foreach (Match match in matches)
        {
            var minutes = int.Parse(match.Groups[1].Value);
            var seconds = int.Parse(match.Groups[2].Value);
            var milliseconds = int.Parse(match.Groups[3].Value);

            // 如果毫秒只有两位，需要乘以10
            if (match.Groups[3].Value.Length == 2)
                milliseconds *= 10;

            var totalSeconds = minutes * 60 + seconds + milliseconds / 1000.0;

            result.Add(new LrcLine
            {
                Time = totalSeconds,
                Text = text,
                IsHighlighted = false
            });
        }

        return result;
    }

    /// <summary>
    /// 根据当前播放时间获取应该高亮的歌词索引
    /// </summary>
    /// <param name="lrcLines">歌词列表</param>
    /// <param name="currentTime">当前播放时间（秒）</param>
    /// <returns>当前歌词索引，如果没有则返回-1</returns>
    public int GetCurrentLrcIndex(List<LrcLine> lrcLines, double currentTime)
    {
        if (lrcLines == null || lrcLines.Count == 0)
            return -1;

        // 找到最后一个时间小于等于当前时间的歌词
        for (int i = lrcLines.Count - 1; i >= 0; i--)
        {
            if (lrcLines[i].Time <= currentTime)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// 更新歌词高亮状态
    /// </summary>
    /// <param name="lrcLines">歌词列表</param>
    /// <param name="highlightIndex">要高亮的索引</param>
    public void UpdateHighlight(List<LrcLine> lrcLines, int highlightIndex)
    {
        if (lrcLines == null || lrcLines.Count == 0)
            return;

        // 清除所有高亮
        foreach (var line in lrcLines)
        {
            line.IsHighlighted = false;
        }

        // 设置当前高亮
        if (highlightIndex >= 0 && highlightIndex < lrcLines.Count)
        {
            lrcLines[highlightIndex].IsHighlighted = true;
        }
    }
}
