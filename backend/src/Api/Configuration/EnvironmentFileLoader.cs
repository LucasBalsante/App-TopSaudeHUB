namespace backend.src.Api.Configuration;

public static class EnvironmentFileLoader
{
    public static void LoadIfExists(params string[] filePaths)
    {
        foreach (var filePath in filePaths.Distinct())
        {
            if (!File.Exists(filePath))
            {
                continue;
            }

            foreach (var rawLine in File.ReadAllLines(filePath))
            {
                var line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                {
                    continue;
                }

                var separatorIndex = line.IndexOf('=');
                if (separatorIndex <= 0)
                {
                    continue;
                }

                var key = line[..separatorIndex].Trim();
                var value = line[(separatorIndex + 1)..].Trim();

                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                value = TrimQuotes(value);

                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                {
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }
    }

    private static string TrimQuotes(string value)
    {
        if (value.Length < 2)
        {
            return value;
        }

        var hasDoubleQuotes = value.StartsWith('"') && value.EndsWith('"');
        var hasSingleQuotes = value.StartsWith('\'') && value.EndsWith('\'');

        return hasDoubleQuotes || hasSingleQuotes
            ? value[1..^1]
            : value;
    }
}