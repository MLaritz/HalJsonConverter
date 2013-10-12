namespace Hal.Json.Extensions
{
    internal static class StringExtensions
    {
        public static string FirstLetterToLower(this string s)
        {
            return s[0].ToString().ToLowerInvariant() + s.Substring(1);
        }
    }
}