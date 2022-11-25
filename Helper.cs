using System.Security.Cryptography;
using System.Text;

namespace SCADAServer
{
    public static class Helper
    {
        static readonly char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
        public static string GetUniqueKey(int size)
        {
            byte[] data = RandomNumberGenerator.GetBytes(size * 4);
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;
                result.Append(chars[idx]);
            }
            return result.ToString();
        }
    }
}
