using System;
using System.Collections.Generic;

namespace CommonItems
{

    static class FormatNumber
    {

        public enum PadingType
        {
            x8, x16, x32, auto
        }

        private static Dictionary<PadingType, string> hexFormat = new Dictionary<PadingType, string>()
        {
            {PadingType.x8, "X1"},
            {PadingType.x16, "X2"},
            {PadingType.x32, "X4"}
        };

        private static Dictionary<PadingType, int> binFormat = new Dictionary<PadingType, int>()
        {
            {PadingType.x8, 8},
            {PadingType.x16, 16},
            {PadingType.x32, 32}
        };

        public static string ToHex(string lead, int value, PadingType pading, string tail)
        {
            if (pading == PadingType.auto)
            {
                pading = PadingType.x8;
                if (value > 255) pading = PadingType.x16;
                if (value > 65535) pading = PadingType.x32;
            }

            return lead + value.ToString(hexFormat[pading]) + tail;
        }

        public static string ToBin(string lead, int value, PadingType pading, string tail)
        {
            if (pading == PadingType.auto)
            {
                pading = PadingType.x8;
                if ((value > 127) || (value < -128)) pading = PadingType.x16;
                if ((value > 32767) || (value < -32768)) pading = PadingType.x32;
            }
            var result = Convert.ToString(value, 2).PadLeft(binFormat[pading]);

            return lead + result + tail;
        }

    }
}
