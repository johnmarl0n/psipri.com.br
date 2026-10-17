using System;
using System.Text.RegularExpressions;

namespace psipri.com.br.Services
{
    /// <summary>
    /// Helper for parsing and validating YouTube URLs and extracting video IDs.
    /// Supports standard, short (youtu.be), embed, shorts, and raw ID formats.
    /// </summary>
    public static class YouTubeHelper
    {
        private static readonly Regex YouTubeRegex = new Regex(
            @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/|youtube\.com\/shorts\/)([^""&?\/\s]{11})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex DirectIdRegex = new Regex(
            @"^[a-zA-Z0-9_-]{11}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Extracts the 11-character YouTube video ID from various link formats.
        /// </summary>
        public static string? ExtractVideoId(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = input.Trim();

            // Handle accidental iframe paste
            if (input.Contains("<iframe", StringComparison.OrdinalIgnoreCase))
            {
                var srcMatch = Regex.Match(input, @"src=[""']([^""']+)[""']", RegexOptions.IgnoreCase);
                if (srcMatch.Success)
                {
                    input = srcMatch.Groups[1].Value;
                }
            }

            // Direct 11-char ID check
            if (DirectIdRegex.IsMatch(input))
            {
                return input;
            }

            var match = YouTubeRegex.Match(input);
            if (match.Success && match.Groups.Count > 1)
            {
                return match.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// Generates a privacy-friendly YouTube embed URL (youtube-nocookie.com).
        /// </summary>
        public static string? GetEmbedUrl(string? videoId)
        {
            if (string.IsNullOrWhiteSpace(videoId))
                return null;

            return $"https://www.youtube-nocookie.com/embed/{videoId}?rel=0&modestbranding=1";
        }
    }
}
