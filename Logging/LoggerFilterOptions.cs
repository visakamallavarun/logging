namespace MiniLogging;

/// <summary>A single category filter rule: "for this category, use this minimum level".</summary>
public sealed class LoggerFilterRule
{
    public string? Category { get; init; }
    public LogLevel Level { get; init; }
}

/// <summary>
/// Represents the minimum levels and category rules, like LoggerFilterOptions.
/// The most specific (longest matching) category rule wins.
/// </summary>
public sealed class LoggerFilterOptions
{
    public LogLevel MinLevel { get; set; } = LogLevel.Information;

    public List<LoggerFilterRule> Rules { get; } = new();

    public LogLevel GetMinLevel(string category)
    {
        LoggerFilterRule? best = null;
        foreach (var rule in Rules)
        {
            bool matches = rule.Category == null
                || category.StartsWith(rule.Category, StringComparison.Ordinal);
            if (!matches)
                continue;

            int currentLength = best?.Category?.Length ?? -1;
            int ruleLength = rule.Category?.Length ?? 0;
            if (best == null || ruleLength > currentLength)
                best = rule;
        }

        return best?.Level ?? MinLevel;
    }
}
