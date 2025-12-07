using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Booking.Infrastructure.Extensions;
public sealed class VnPayLibrary
{
    private readonly SortedList<string, string> _requestData = new(new VnPayCompare());
    private readonly SortedList<string, string> _responseData = new(new VnPayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            _requestData[key] = value;
    }

   public string CreateRequestUrl(string baseUrl, string hashSecret, ILogger logger = null!)
    {
        var data = _requestData.Where(kv => !string.IsNullOrEmpty(kv.Value));

        // 1️⃣ RAW DATA FOR HASH (CHƯA ENCODE)
        var rawData = string.Join("&", data.Select(kv => $"{kv.Key}={kv.Value}"));
        
        logger?.LogInformation("VNPay RAW DATA (for hash): {raw}", rawData);

        var secureHash = HmacSHA512(hashSecret, rawData);

        // 2️⃣ BUILD URL (CHỈ ENCODE VALUES, KHÔNG ENCODE KEY)
        var queryString = string.Join("&", data.Select(kv => 
            $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));
        
        var finalUrl = $"{baseUrl}?{queryString}&vnp_SecureHash={secureHash}";

        logger?.LogInformation("VNPay FINAL URL: {url}", finalUrl);

        return finalUrl;
    }



    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            _responseData[key] = value;
    }

    public string GetResponseData(string key)
        => _responseData.TryGetValue(key, out var value) ? value : string.Empty;
    public bool ValidateSignature(string inputHash, string hashSecret)
    {
        var rawData = string.Join("&",
            _responseData
                .Where(kv => !string.IsNullOrEmpty(kv.Value) &&
                             kv.Key != "vnp_SecureHashType" &&
                             kv.Key != "vnp_SecureHash")
                .Select(kv => $"{kv.Key}={kv.Value}"));

        var myChecksum = HmacSHA512(hashSecret, rawData);
        return myChecksum.Equals(inputHash, StringComparison.OrdinalIgnoreCase);
    }
    private static string HmacSHA512(string key, string input)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

internal sealed class VnPayCompare : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (x == y) return 0;
        if (x == null) return -1;
        if (y == null) return 1;
        return string.Compare(x, y, StringComparison.Ordinal);
    }
}