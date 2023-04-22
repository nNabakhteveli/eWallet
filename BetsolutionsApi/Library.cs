using System.Security.Cryptography;
using System.Text;

namespace BetsolutionsApi;

public static class Library
{
    private static string GetSha256(string text)
    {
        var utf8Encoding = new UTF8Encoding();
        var message = utf8Encoding.GetBytes(text);

        var hashString = new SHA256Managed();
        var hex = string.Empty;

        var hashValue = hashString.ComputeHash(message);

        return hashValue.Aggregate(hex, (current, bt) => current + $"{bt:x2}");
    }
}