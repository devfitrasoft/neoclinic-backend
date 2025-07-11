using System;

namespace Shared.Common
{
    public sealed class StringParser
    {
        public static string[] DivideToSegmentsByDots(string rawString)
        {
            var segments = rawString.Split('.');
            if (segments.Length < 2)
                throw new ArgumentException("Bad format", nameof(rawString));

            return segments;
        }
    }
}
