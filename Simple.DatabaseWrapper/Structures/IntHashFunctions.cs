namespace Simple.DatabaseWrapper.Structures
{
    // https://web.archive.org/web/20061030103559/http://www.concentric.net/~Ttwang/tech/inthash.htm
    // https://gist.github.com/badboy/6267743
    public class IntHashFunctions
    {
        /// <summary>
        /// Thomas Wang's 32 bit Mix Function
        /// </summary>
        public static uint TomasIntHash(uint key)
        {
            key += ~(key << 15);
            key ^= (key >> 10);
            key += (key << 3);
            key ^= (key >> 6);
            key += ~(key << 11);
            key ^= (key >> 16);
            return key;
        }
        public static uint Tomas3v1IntHash(uint key)
        {
            key = ~key + (key << 15);
            key ^= (key >> 12);
            key += (key << 2);
            key ^= (key >> 4);
            key *= 2057;
            key ^= (key >> 16);
            return key;
        }

        /// <summary>
        /// Thomas Wang's 64 bit Mix Function
        /// </summary>
        public static ulong TomasLongHash(ulong key)
        {
            key += ~(key << 32);
            key ^= (key >> 22);
            key += ~(key << 13);
            key ^= (key >> 8);
            key += (key << 3);
            key ^= (key >> 15);
            key += ~(key << 27);
            key ^= (key >> 31);
            return key;
        }

        public static uint hash6432shift(ulong key)
        {
            key = (~key) + (key << 18); // key = (key << 18) - key - 1;
            key = key ^ (key >> 31);
            key = key * 21; // key = (key + (key << 2)) + (key << 4);
            key = key ^ (key >> 11);
            key = key + (key << 6);
            key = key ^ (key >> 22);
            return (uint)key;
        }
    }
}
