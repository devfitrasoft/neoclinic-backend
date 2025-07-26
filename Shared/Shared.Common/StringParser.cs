using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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

        public static string ValidationErrorMessageBuilder(List<ValidationResult> validationResults)
            => string.Join(" ", validationResults.Select(vr =>
            {
                var memberNames = string.Join(", ", vr.MemberNames);
                return !string.IsNullOrEmpty(memberNames)
                    ? $"{memberNames} {vr.ErrorMessage}"
                    : $"{vr.ErrorMessage}";
            }));
    }
}
