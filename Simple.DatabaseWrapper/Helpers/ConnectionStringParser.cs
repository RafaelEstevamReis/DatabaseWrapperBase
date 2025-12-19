namespace Simple.DatabaseWrapper.Helpers;

using System;
using System.Collections.Generic;
using System.Text;

public static class ConnectionStringParser
{
    public static Dictionary<string, string> Parse(string connectionString)
    {
#if NET20
        if (string.IsNullOrEmpty(connectionString) || connectionString.Trim().Length == 0) return [];
#else
        if (string.IsNullOrWhiteSpace(connectionString)) return [];
#endif

        if (connectionString.Contains("\"\""))
        {
            throw new ArgumentException("Double-quotted quotes are not supported");
        }

        var result = new Dictionary<string, string>();
        var currentKey = new StringBuilder();
        var currentValue = new StringBuilder();
        bool inKey = true;
        bool inQuotes = false;
        bool escapeNext = false;

        for (int i = 0; i < connectionString.Length; i++)
        {
            char c = connectionString[i];

            if (escapeNext)
            {
                (inKey ? currentKey : currentValue).Append(c);
                escapeNext = false;
                continue;
            }

            if (c == '\\')
            {
                escapeNext = true;
                continue;
            }

            if (c == '"' && !inKey)
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (c == '=' && !inQuotes && inKey)
            {
                inKey = false;
                continue;
            }

            if (c == ';' && !inQuotes)
            {
                // If [inKey], no Equal sign

                if (currentKey.Length > 0 && !inKey)
                {
                    result[currentKey.ToString().Trim()] = currentValue.ToString().Trim();
                }

#if NET20
                currentKey = new StringBuilder();
                currentValue = new StringBuilder();
#else
                currentKey.Clear();
                currentValue.Clear();
#endif

                inKey = true;
                continue;
            }

            (inKey ? currentKey : currentValue).Append(c);
        }

        // Handle the last key-value pair
        if (currentKey.Length > 0)
        {
            result[currentKey.ToString().Trim()] = currentValue.ToString().Trim();
        }

        return result;
    }
}
