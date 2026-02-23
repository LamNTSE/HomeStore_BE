using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace HomeStore.BLL.Helpers;

public class VnpayHelper
{
    private readonly SortedList<string, string> _requestData = new(new VnpayCompare());
    private readonly SortedList<string, string> _responseData = new(new VnpayCompare());

    public void AddRequestData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            _requestData[key] = value;
    }

    public void AddResponseData(string key, string value)
    {
        if (!string.IsNullOrEmpty(value))
            _responseData[key] = value;
    }

    public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
    {
        var data = new StringBuilder();
        foreach (var kv in _requestData)
        {
            if (data.Length > 0) data.Append('&');
            data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value));
        }

        var queryString = data.ToString();
        var signData = queryString;
        var vnpSecureHash = HmacSha512(vnpHashSecret, signData);

        return baseUrl + "?" + queryString + "&vnp_SecureHash=" + vnpSecureHash;
    }

    public bool ValidateSignature(string inputHash, string secretKey)
    {
        var rspRaw = GetResponseData();
        var myChecksum = HmacSha512(secretKey, rspRaw);
        return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
    }

    public string GetResponseData()
    {
        var data = new StringBuilder();
        if (_responseData.ContainsKey("vnp_SecureHashType"))
            _responseData.Remove("vnp_SecureHashType");
        if (_responseData.ContainsKey("vnp_SecureHash"))
            _responseData.Remove("vnp_SecureHash");

        foreach (var kv in _responseData)
        {
            if (data.Length > 0) data.Append('&');
            data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value));
        }
        return data.ToString();
    }

    public static string HmacSha512(string key, string inputData)
    {
        var hash = new StringBuilder();
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var inputBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmac = new HMACSHA512(keyBytes))
        {
            var hashValue = hmac.ComputeHash(inputBytes);
            foreach (var b in hashValue)
                hash.Append(b.ToString("x2"));
        }
        return hash.ToString();
    }

    private class VnpayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
            => string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase);
    }
}
