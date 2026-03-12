using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace backend.src.Infrastructure.Services
{
    public static class PayloadHasher
    {
        public static string ComputeSha256<T>(T payload)
        {
            var json = JsonSerializer.Serialize(payload);
            return ComputeSha256(json);
        }

        public static string ComputeSha256(string payload)
        {
            payload ??= string.Empty;
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
            return Convert.ToHexString(bytes);
        }
    }
}
