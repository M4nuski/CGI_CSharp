using System;

namespace CommonItems
{
    static class SafeParse
    {
        public static byte ToByte(string value, int numerbase, byte fallback)
        {
            byte result;
            try
            {
                result = Convert.ToByte(value, numerbase);
            }
            catch (Exception)
            {
                result = fallback;
            }
            return result;
        }

        public static int ToInt(string value, int numerbase, int fallback)
        {
            int result;
            try
            {
                result = Convert.ToInt32(value, numerbase);
            }
            catch (Exception)
            {
                result = fallback;
            }
            return result;
        }

    }

}
