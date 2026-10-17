using System;
using System.Text.RegularExpressions;

namespace psipri.com.br.Services
{
    /// <summary>
    /// Helper for parsing and validating video URLs (YouTube & Instagram)
    /// and generating privacy-friendly embed codes for standard and vertical (Reels/Shorts) formats.
    /// </summary>
    public static class YouTubeHelper
    {
        // YouTube: standard watch, short youtu.be, embed, and shorts
        private static readonly Regex YouTubeRegex = new Regex(
            @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/|youtube\.com\/shorts\/)([^""&?\/\s]{11})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex YouTubeShortsRegex = new Regex(
            @"youtube\.com\/shorts\/([^""&?\/\s]{11})",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        // Instagram: reels, posts, tv
        private static readonly Regex InstagramRegex = new Regex(
            @"(?:instagram\.com\/(?:reel|reels|p|tv)\/)([a-zA-Z0-9_-]+)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex DirectIdRegex = new Regex(
            @"^[a-zA-Z0-9_-]{11}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Extracts YouTube video ID or Instagram code from various link formats.
        /// Maintained for backwards compatibility.
        /// </summary>
        public static string? ExtractVideoId(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = CleanUrl(input);

            // Direct 11-char ID check (YouTube)
            if (DirectIdRegex.IsMatch(input))
            {
                return input;
            }

            // YouTube match
            var ytMatch = YouTubeRegex.Match(input);
            if (ytMatch.Success && ytMatch.Groups.Count > 1)
            {
                return ytMatch.Groups[1].Value;
            }

            // Instagram match
            var igMatch = InstagramRegex.Match(input);
            if (igMatch.Success && igMatch.Groups.Count > 1)
            {
                return igMatch.Groups[1].Value;
            }

            return null;
        }

        /// <summary>
        /// Returns the platform provider: "YouTube", "Instagram", or null.
        /// </summary>
        public static string? GetProvider(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = CleanUrl(input);

            if (InstagramRegex.IsMatch(input))
                return "Instagram";

            if (DirectIdRegex.IsMatch(input) || YouTubeRegex.IsMatch(input))
                return "YouTube";

            return null;
        }

        /// <summary>
        /// Detects if the URL represents a naturally vertical video (Reels or Shorts).
        /// </summary>
        public static bool IsLikelyVertical(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true; // Default to vertical (Reels/Shorts)

            input = CleanUrl(input);

            if (InstagramRegex.IsMatch(input))
                return true;

            if (YouTubeShortsRegex.IsMatch(input))
                return true;

            return false;
        }

        /// <summary>
        /// Generates the appropriate embed URL for YouTube or Instagram.
        /// </summary>
        public static string? GetEmbedUrl(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            input = CleanUrl(input);

            // Check Instagram first
            var igMatch = InstagramRegex.Match(input);
            if (igMatch.Success && igMatch.Groups.Count > 1)
            {
                var code = igMatch.Groups[1].Value;
                return $"https://www.instagram.com/reel/{code}/embed/";
            }

            // Check YouTube
            string? videoId = null;
            if (DirectIdRegex.IsMatch(input))
            {
                videoId = input;
            }
            else
            {
                var ytMatch = YouTubeRegex.Match(input);
                if (ytMatch.Success && ytMatch.Groups.Count > 1)
                {
                    videoId = ytMatch.Groups[1].Value;
                }
            }

            if (!string.IsNullOrEmpty(videoId))
            {
                return $"https://www.youtube-nocookie.com/embed/{videoId}?rel=0&modestbranding=1";
            }

            return null;
        }

        private static string CleanUrl(string input)
        {
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

            return input;
        }
    }
}
