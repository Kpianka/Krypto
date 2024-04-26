using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        int[] lengths = { 1, 10, 100, 1000, 10000 };
        foreach (var length in lengths)
        {
            Console.WriteLine("length: " + length);
            var hashes = new Dictionary<string, HashSet<string>>();
            string randomString = GenerateRandomString(length);
            MeasureHashTimeAndCheckCollisions("MD5", MD5.Create(), randomString, hashes);
            MeasureHashTimeAndCheckCollisions("SHA-256", SHA256.Create(), randomString, hashes);
            MeasureHashTimeAndCheckCollisions("SHA-1", SHA1.Create(), randomString, hashes);
            MeasureHashTimeAndCheckCollisions("SHA-512", SHA512.Create(), randomString, hashes);
            MeasureHashTimeAndCheckCollisions("SHA-384", SHA384.Create(), randomString, hashes);
        }
    }

    static string GenerateRandomString(int length)
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    static void MeasureHashTimeAndCheckCollisions(string hashName, HashAlgorithm hashAlgorithm, string input, Dictionary<string, HashSet<string>> hashes)
    {
        var stopwatch = Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            var hash = GetHash(hashAlgorithm, input);
            if (!hashes.ContainsKey(hashName))
            {
                hashes[hashName] = new HashSet<string>();
            }
            hashes[hashName].Add(hash);
        }

        stopwatch.Stop();
        int collisionsCount = 0;
        foreach (var hashSet in hashes.Values)
        {
            var collisions = hashSet.GroupBy(x => x)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key);

            collisionsCount += collisions.Count();
        }
        Console.WriteLine($"{hashName} - Time: {stopwatch.Elapsed.TotalSeconds:F6} seconds, Collisions: {collisionsCount}");
    }

    static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
        var data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
        var sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }
}
