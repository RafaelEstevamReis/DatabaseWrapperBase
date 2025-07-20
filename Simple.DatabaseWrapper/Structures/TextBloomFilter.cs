namespace Simple.DatabaseWrapper.Structures;

using System;
using System.Collections;
using System.IO;
using System.Text;

/// <summary>
/// Text optmized, case insensitive, bloom filter with MurmurHash2
/// </summary>
public class BloomFilter
{
    private readonly BitArray bitArray;
    private readonly int size;
    private readonly int hashCount;
    private readonly int expectedItems;
    private readonly MurmurHash2[] hashFunctions;
    private readonly uint[] hashSeeds;

    /// <summary>
    /// Creates a new BloomFilter instance
    /// </summary>
    /// <param name="expectedItems">Expected items</param>
    /// <param name="falsePositiveRate">Expected false positives, default: 1%</param>
    public BloomFilter(int expectedItems, double falsePositiveRate = 0.01)
    {
        this.expectedItems = expectedItems;
        // calculate filter size: m = -n * ln(p) / (ln(2)^2)
        size = (int)Math.Ceiling(-expectedItems * Math.Log(falsePositiveRate) / (Math.Log(2) * Math.Log(2)));
        size = ((size + 7) / 8) * 8; // align bytes
        bitArray = new BitArray(size);
        // calculate ammount of functions hash: k = (m/n) * ln(2)
        hashCount = (int)Math.Ceiling((size / (double)expectedItems) * Math.Log(2));
        // Initialize
        hashFunctions = new MurmurHash2[hashCount];
        hashSeeds = new uint[hashCount];
        for (int i = 0; i < hashCount; i++)
        {
            hashSeeds[i] = (uint)i; // different seed
            hashFunctions[i] = new MurmurHash2(hashSeeds[i]);
        }
    }

    private BloomFilter(int expectedItems, BitArray bitArray, uint[] seeds)
    {
        this.expectedItems = expectedItems;
        this.size = bitArray.Length;
        this.bitArray = bitArray;
        this.hashCount = seeds.Length;
        this.hashSeeds = seeds;
        this.hashFunctions = new MurmurHash2[hashCount];
        for (int i = 0; i < hashCount; i++)
        {
            hashFunctions[i] = new MurmurHash2(seeds[i]);
        }
    }

    /// <summary>
    /// Adds a new text to the filter
    /// </summary>
    public void Add(string text)
    {
        foreach (var hash in GetHashes(text))
        {
            bitArray[(int)(hash % size)] = true;
        }
    }
    /// <summary>
    /// Check if the text can be in the filter
    /// </summary>
    public bool MightContain(string text)
    {
        foreach (var hash in GetHashes(text))
        {
            if (!bitArray[(int)(hash % size)]) return false;
        }
        return true;
    }

    private uint[] GetHashes(string text)
    {
        uint[] hashes = new uint[hashCount];
        byte[] data = Encoding.UTF8.GetBytes(text.ToLower());
        for (int i = 0; i < hashCount; i++)
        {
            hashes[i] = hashFunctions[i].ComputeHash(data);
        }
        return hashes;
    }
    /// <summary>
    /// Serialize to a binary file
    /// </summary>
    public void SaveToFile(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Create);
        using var writer = new BinaryWriter(stream);
        writer.Write([1, 0, 0]); // version 1.0.0
        // write metadata
        writer.Write(expectedItems);
        writer.Write(hashCount);
        // write seeds
        foreach (var seed in hashSeeds)
        {
            writer.Write(seed);
        }
        // write BitArray
        byte[] bytes = new byte[bitArray.Length / 8];
        bitArray.CopyTo(bytes, 0);
        writer.Write(bytes);
    }
    /// <summary>
    /// Loads from a File
    /// </summary>
    public static BloomFilter LoadFromFile(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open);
        using var reader = new BinaryReader(stream);
        // read version
        byte[] version = reader.ReadBytes(3);
        if (version.Length != 3) throw new ArgumentException("Invalid file version");
        if (version[0] != 1 || version[1] != 0 || version[2] != 0) throw new ArgumentException("File from older or newer version, filter can not be loaded and must be recreated");

        // read metadata
        int expectedItems = reader.ReadInt32();
        int hashCount = reader.ReadInt32();
        // read seeds
        uint[] seeds = new uint[hashCount];
        for (int i = 0; i < hashCount; i++)
        {
            seeds[i] = reader.ReadUInt32();
        }
        // read BitArray
        byte[] bytes = reader.ReadBytes((int)(stream.Length - stream.Position));
        var bitArray = new BitArray(bytes);
        return new BloomFilter(expectedItems, bitArray, seeds);
    }
}

internal static class BitArrayExtensions
{
    public static void CopyFrom(this BitArray bitArray, byte[] bytes)
    {
        if (bytes.Length * 8 != bitArray.Length)
            throw new ArgumentException("Tamanho do byte array não corresponde ao tamanho do BitArray.");
        for (int i = 0; i < bytes.Length; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                bitArray[i * 8 + j] = (bytes[i] & (1 << j)) != 0;
            }
        }
    }
}