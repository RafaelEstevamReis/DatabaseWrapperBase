using System;

namespace Simple.DatabaseWrapper.Structures
{
    /// <summary>
    /// Computes hash using .Net internal GetHashCode
    /// </summary>
    public class BloomHash_GetHashCode : IHashFunction
    {
        public uint ComputeHash(object o)
            => unchecked((uint)o.GetHashCode());
    }
    /// <summary>
    /// Uses jitbit's MurmurHash2 implementation
    /// </summary>
    public class BloomHash_Murmur2 : IHashFunction
    {
        public uint ComputeHash(object o)
        {
            if (o is int iVal) return intHash(iVal);
            if (o is long lVal) return longHash(lVal);
            if (o is string sVal) return MurmurHash2.Hash(sVal);
            if (o is byte[] baVal) return MurmurHash2.Hash(baVal);
            throw new NotSupportedException();
        }
        static uint intHash(int data)
            => MurmurHash2.Hash(BitConverter.GetBytes(data));
        static uint longHash(long data)
            => MurmurHash2.Hash(BitConverter.GetBytes(data));

    }
    public class BloomHash_ThomasMix : IHashFunction
    {
        public uint ComputeHash(object o)
        {
            if (o is int iVal)
            {
                return IntHashFunctions.TomasIntHash(unchecked((uint)iVal));
            }
            if (o is long lVal)
            {
                var lResult = IntHashFunctions.TomasLongHash(unchecked((ulong)lVal));
                return IntHashFunctions.hash6432shift(lResult);
            }

            throw new NotSupportedException();
        }
    }

}
