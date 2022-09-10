using System;

namespace Simple.DatabaseWrapper.Structures
{
    /// <summary>
    /// Computes hash using .Net internal GetHashCode
    /// </summary>
    public class BloomHash_GetHashCode : IHashFunction
    {
        public int ComputeHash(object o)
            => o.GetHashCode();
    }
    /// <summary>
    /// Uses jitbit's MurmurHash2 implementation
    /// </summary>
    public class BloomHash_Murmur2 : IHashFunction
    {
        public int ComputeHash(object o)
        {
            return unchecked((int)hash(o)); // leave negative
        }
        private uint hash(object o)
        {
            if (o is int iVal) return intHash(iVal);
            if (o is string sVal) return MurmurHash2.Hash(sVal);
            if (o is byte[] baVal) return MurmurHash2.Hash(baVal);
            throw new NotSupportedException();
        }
        static uint intHash(int data)
            => MurmurHash2.Hash(BitConverter.GetBytes(data));

    }

}
