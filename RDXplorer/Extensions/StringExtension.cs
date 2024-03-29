﻿using System;

namespace RDXplorer.Extensions
{
    public static class StringExtension
    {
        public static string RemoveEnd(this string input, string value, StringComparison comparisonType = StringComparison.CurrentCultureIgnoreCase)
        {
            if (!string.IsNullOrEmpty(value))
                while (!string.IsNullOrEmpty(input) && input.EndsWith(value, comparisonType))
                    input = input[..^value.Length];
            return input;
        }
    }
}
