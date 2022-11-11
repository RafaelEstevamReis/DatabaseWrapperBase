using System;

namespace Simple.DatabaseWrapper.FrameworkCompatibility
{
    internal class String_NullOrWhitespace
    {
#if NET20
        internal static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null) return true;

            for (int i = 0; i < value.Length; i++)
            {
                if (!Char.IsWhiteSpace(value[i])) return false;
            }

            return true;
        }
#else
        internal static bool IsNullOrWhiteSpace(string value)
            => string.IsNullOrWhiteSpace(value);
#endif

    }
}
