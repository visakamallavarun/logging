using System.Collections;
using System.Text;

namespace MiniLogging;

/// <summary>
/// Holds a message template plus its arguments so the original template and the
/// named values survive separately (structured logging). This is a tiny version
/// of the framework's FormattedLogValues.
/// </summary>
internal readonly struct FormattedLogValues : IReadOnlyList<KeyValuePair<string, object?>>
{
    public static readonly Func<FormattedLogValues, Exception?, string> Formatter = (state, _) => state.ToString();

    private const string OriginalFormatKey = "{OriginalFormat}";

    private readonly string _format;
    private readonly object?[] _args;
    private readonly string[] _names;

    public FormattedLogValues(string format, object?[]? args)
    {
        _format = format ?? string.Empty;
        _args = args ?? Array.Empty<object?>();
        _names = ParseNames(_format);
    }

    public int Count => _names.Length + 1;

    public KeyValuePair<string, object?> this[int index]
    {
        get
        {
            if (index == _names.Length)
                return new KeyValuePair<string, object?>(OriginalFormatKey, _format);

            object? value = index < _args.Length ? _args[index] : null;
            return new KeyValuePair<string, object?>(_names[index], value);
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder(_format.Length);
        int i = 0, arg = 0;
        while (i < _format.Length)
        {
            char c = _format[i];
            if (c == '{')
            {
                int end = _format.IndexOf('}', i);
                if (end < 0)
                {
                    sb.Append(_format, i, _format.Length - i);
                    break;
                }

                sb.Append(arg < _args.Length ? _args[arg]?.ToString() : string.Empty);
                arg++;
                i = end + 1;
            }
            else
            {
                sb.Append(c);
                i++;
            }
        }

        return sb.ToString();
    }

    public IEnumerator<KeyValuePair<string, object?>> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private static string[] ParseNames(string format)
    {
        var names = new List<string>();
        int i = 0;
        while (i < format.Length)
        {
            if (format[i] == '{')
            {
                int end = format.IndexOf('}', i);
                if (end < 0)
                    break;

                names.Add(format.Substring(i + 1, end - i - 1));
                i = end + 1;
            }
            else
            {
                i++;
            }
        }

        return names.ToArray();
    }
}
