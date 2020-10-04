using System;

namespace Lambada.Base
{
    public class StringHelpers
    {
        private static readonly Random rng = new Random();
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        public static string RandomString(int size)
        {
            var buffer = new char[size];
            for (var i = 0; i < size; i++)
            {
                buffer[i] = chars[rng.Next(chars.Length)];
            }

            return new string(buffer);
        }
    }
}