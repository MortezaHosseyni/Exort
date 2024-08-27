using System.Text.RegularExpressions;

namespace Shared.Utilities
{
    public class Sanitize
    {
        public static string Clarify(string? input)
        {
            string unSanitizedInput = input ?? string.Empty;

            // Check empty string
            if (string.IsNullOrEmpty(unSanitizedInput))
            {
                return string.Empty;
            }

            // Remove HTML tags
            string sanitizedInput = Regex.Replace(unSanitizedInput, "<.*?>", string.Empty);

            // Check for links and remove them
            sanitizedInput = Regex.Replace(sanitizedInput, @"<a\b[^>]*>(.*?)</a>", string.Empty);

            // Check for scripts and remove them
            sanitizedInput = Regex.Replace(sanitizedInput, @"<script\b[^>]*>(.*?)</script>", string.Empty);

            // Check for common SQL injection patterns and remove them
            sanitizedInput = Regex.Replace(sanitizedInput, @"(;|--|;|\bDROP\b|\bDELETE\b|\bTRUNCATE\b|\bINSERT\b|\bUPDATE\b|\bCREATE\b|\bALTER\b|\bSELECT\b|\bFROM\b|\bWHERE\b|\bAND\b|\bOR\b)", string.Empty, RegexOptions.IgnoreCase);

            // Check for common MongoDB injection patterns and remove them
            sanitizedInput = Regex.Replace(sanitizedInput, @"(\bdb\.collection\.\b|\bdb\.drop\(\)\s*;|\bdb\.dropDatabase\(\)\s*;)", string.Empty, RegexOptions.IgnoreCase);

            return sanitizedInput.Trim();
        }

        public static List<string> Clarify(List<string> inputs)
        {
            List<string> sanitizedList = new List<string>();

            foreach (var input in inputs)
            {
                string unSanitizedInput = input ?? string.Empty;

                // Check empty string
                if (string.IsNullOrEmpty(unSanitizedInput))
                {
                    sanitizedList.Add(string.Empty);
                    continue;
                }

                // Remove HTML tags
                string sanitizedInput = Regex.Replace(unSanitizedInput, "<.*?>", string.Empty);

                // Check for links and remove them
                sanitizedInput = Regex.Replace(sanitizedInput, @"<a\b[^>]*>(.*?)</a>", string.Empty);

                // Check for scripts and remove them
                sanitizedInput = Regex.Replace(sanitizedInput, @"<script\b[^>]*>(.*?)</script>", string.Empty);

                // Check for common SQL injection patterns
                sanitizedInput = Regex.Replace(sanitizedInput, @"(;|--|;|\bDROP\b|\bDELETE\b|\bTRUNCATE\b|\bINSERT\b|\bUPDATE\b|\bCREATE\b|\bALTER\b|\bSELECT\b|\bFROM\b|\bWHERE\b|\bAND\b|\bOR\b)", string.Empty, RegexOptions.IgnoreCase);

                // Check for common MongoDB injection patterns
                sanitizedInput = Regex.Replace(sanitizedInput, @"(\bdb\.collection\.\b|\bdb\.drop\(\)\s*;|\bdb\.dropDatabase\(\)\s*;)", string.Empty, RegexOptions.IgnoreCase);

                sanitizedList.Add(sanitizedInput.Trim());
            }

            return sanitizedList;
        }

        public static Dictionary<string, string> Clarify(Dictionary<string, string> inputs)
        {
            Dictionary<string, string> sanitizedDict = new Dictionary<string, string>();

            foreach (var kvp in inputs)
            {
                string key = kvp.Key;
                string unSanitizedInput = kvp.Value ?? string.Empty;

                // Check empty string
                if (string.IsNullOrEmpty(unSanitizedInput))
                {
                    sanitizedDict[key] = string.Empty;
                    continue;
                }

                // Remove HTML tags
                string sanitizedInput = Regex.Replace(unSanitizedInput, "<.*?>", string.Empty);

                // Check for links and remove them
                sanitizedInput = Regex.Replace(sanitizedInput, @"<a\b[^>]*>(.*?)</a>", string.Empty);

                // Check for scripts and remove them
                sanitizedInput = Regex.Replace(sanitizedInput, @"<script\b[^>]*>(.*?)</script>", string.Empty);

                // Check for common SQL injection patterns
                sanitizedInput = Regex.Replace(sanitizedInput, @"(;|--|;|\bDROP\b|\bDELETE\b|\bTRUNCATE\b|\bINSERT\b|\bUPDATE\b|\bCREATE\b|\bALTER\b|\bSELECT\b|\bFROM\b|\bWHERE\b|\bAND\b|\bOR\b)", string.Empty, RegexOptions.IgnoreCase);

                // Check for common MongoDB injection patterns
                sanitizedInput = Regex.Replace(sanitizedInput, @"(\bdb\.collection\.\b|\bdb\.drop\(\)\s*;|\bdb\.dropDatabase\(\)\s*;)", string.Empty, RegexOptions.IgnoreCase);

                sanitizedDict[key] = sanitizedInput.Trim();
            }

            return sanitizedDict;
        }
    }
}
