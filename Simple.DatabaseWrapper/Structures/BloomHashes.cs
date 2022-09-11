using System;
using System.Drawing;

namespace Simple.DatabaseWrapper.Structures
{
    /// <summary>
    /// Computes hash using .Net internal GetHashCode, and un-randomized String.GetHashCode
    /// </summary>
    public class BloomHash_GetHashCode : IHashFunction
    {
        public uint ComputeHash(object o)
        {
            if (o is string str)
            {
                return unchecked((uint)GetDeterministicHashCode(str));
            }

            return unchecked((uint)o.GetHashCode());
        }
        // https://github.com/dotnet/corefx/blob/a10890f4ffe0fadf090c922578ba0e606ebdd16c/src/Common/src/System/Text/StringOrCharArray.cs#L140
        static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
    /// <summary>
    /// Uses jitbit's MurmurHash2 implementation
    /// </summary>
    public class BloomHash_Murmur2 : IHashFunction
    {
        public uint ComputeHash(object o)
        {
            if (o is int iVal) return intHash(iVal);
            if (o is uint uiVal) return uintHash(uiVal);
            if (o is long lVal) return longHash(lVal);
            if (o is ulong ulVal) return ulongHash(ulVal);
            if (o is string sVal) return MurmurHash2.Hash(sVal);
            if (o is byte[] baVal) return MurmurHash2.Hash(baVal);
            if (o is Guid gVal) return MurmurHash2.Hash(gVal.ToByteArray());
            throw new NotSupportedException();
        }
        static uint intHash(int data)
            => MurmurHash2.Hash(BitConverter.GetBytes(data));
        static uint uintHash(uint data)
            => MurmurHash2.Hash(BitConverter.GetBytes(data));
        static uint longHash(long data)
            => MurmurHash2.Hash(BitConverter.GetBytes(data));
        static uint ulongHash(ulong data)
            => MurmurHash2.Hash(BitConverter.GetBytes(data));

    }
    /// <summary>
    /// Uses Thomas Wang's bitMix implementation
    /// </summary>
    public class BloomHash_ThomasMix : IHashFunction
    {
        public uint ComputeHash(object o)
        {
            if (o is int iVal)
            {
                return IntHashFunctions.Tomas3v1IntHash(unchecked((uint)iVal));
            }
            if (o is uint uiVal)
            {
                return IntHashFunctions.Tomas3v1IntHash(uiVal);
            }
            if (o is long lVal)
            {
                var lResult = IntHashFunctions.TomasLongHash(unchecked((ulong)lVal));
                return IntHashFunctions.hash6432shift(lResult);
            }
            if (o is ulong ulVal)
            {
                var lResult = IntHashFunctions.TomasLongHash(ulVal);
                return IntHashFunctions.hash6432shift(lResult);
            }

            throw new NotSupportedException();
        }
    }

    public class BloomHash_Murmur3 : IHashFunction
    {
        public bool LowSide { get; }
        public BloomHash_Murmur3()
        { LowSide = true; }
        public BloomHash_Murmur3(bool lowSide)
        { LowSide = lowSide; }

        public uint ComputeHash(object o)
        {
            ulong hash;

            if (o is int iVal)
            {
                hash = IntHashFunctions.Murmur3_64(unchecked((ulong)iVal));
            }
            else if (o is uint uiVal)
            {
                hash = IntHashFunctions.Murmur3_64(uiVal);
            }
            else if (o is long lVal)
            {
                hash = IntHashFunctions.Murmur3_64(unchecked((ulong)lVal));
            }
            else if (o is ulong ulVal)
            {
                hash = IntHashFunctions.Murmur3_64(ulVal);
            }
            else throw new NotSupportedException();

            if (LowSide) return (uint)(hash & 0xFFFFFFFF);
            return (uint)(hash >> 32);
        }
    }

}
