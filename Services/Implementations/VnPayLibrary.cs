using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Getdata1.Services.Implementations
{
    public class VnPayLibrary
    {
        public const string VERSION = "2.1.0";
        private SortedList<String, String> _requestData = new SortedList<String, String>(new VnPayCompare());
        private SortedList<String, String> _responseData = new SortedList<String, String>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                 if(_requestData.ContainsKey(key))
                {
                    _requestData[key] = value;
                }    
                else
                {
                    _requestData.Add(key, value);
                }    
                
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            string retValue;
            if (_responseData.TryGetValue(key, out retValue))
            {
                return retValue;
            }
            else
            {
                return string.Empty;
            }
        }

        #region Request
        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            StringBuilder signData = new StringBuilder(); // Chuỗi để Hash (KHÔNG ENCODE)

            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    // 1. Tạo chuỗi cho URL (CÓ ENCODE)
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                    //Console.WriteLine("CHUỖI ĐANG BĂM: " + signData);
                    //// 2. Tạo chuỗi cho SignData (KHÔNG ENCODE KEY/VALUE)
                    signData.Append(kv.Key + "=" + kv.Value + "&");
                }
            }

            // Xử lý chuỗi để Hash
            string rawSignData = signData.ToString().Remove(signData.Length - 1, 1);
            System.Diagnostics.Debug.WriteLine("=== DEBUG VNPAY ===");
            System.Diagnostics.Debug.WriteLine("RAW_SIGN_DATA: " + rawSignData);
            System.Diagnostics.Debug.WriteLine("HASH_SECRET: " + vnp_HashSecret);
            string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, rawSignData);

            // Tạo URL cuối cùng
            return baseUrl + "?" + data.ToString() + "vnp_SecureHash=" + vnp_SecureHash;
        }

        //public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        //{
        //    StringBuilder data = new StringBuilder();
        //    foreach (KeyValuePair<string, string> kv in _requestData)
        //    {
        //        if (!String.IsNullOrEmpty(kv.Value))
        //        {
        //            data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
        //        }
        //    }
        //    string queryString = data.ToString();

        //    baseUrl += "?" + queryString;
        //    String signData = queryString;
        //    if (signData.Length > 0)
        //    {

        //        signData = signData.Remove(data.Length - 1, 1);
        //    }
        //    string vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);
        //    baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

        //    return baseUrl;
        //}



        #endregion

        #region Response process

        public bool ValidateSignature(string inputHash, string secretKey)
        {
            string rspRaw = GetResponseData();
            string myChecksum = Utils.HmacSHA512(secretKey, rspRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
        private string GetResponseData()
        {
            StringBuilder data = new StringBuilder();
            // ... bỏ các phần remove cũ ...

            foreach (KeyValuePair<string, string> kv in _responseData)
            {
                if (!String.IsNullOrEmpty(kv.Value))
                {
                    // BỎ WebUtility.UrlEncode Ở ĐÂY, VNPAY KHÔNG ENCODE KHI BĂM!
                    data.Append(kv.Key + "=" + kv.Value + "&");
                }
            }
            if (data.Length > 0) data.Remove(data.Length - 1, 1);
            return data.ToString();
        }

        #endregion
    }

    public class Utils
    {


        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
        //public static string GetIpAddress()
        //{
        //    string ipAddress;
        //    try
        //    {
        //        ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        //        if (string.IsNullOrEmpty(ipAddress) || (ipAddress.ToLower() == "unknown") || ipAddress.Length > 45)
        //            ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
        //    }
        //    catch (Exception ex)
        //    {
        //        ipAddress = "Invalid IP:" + ex.Message;
        //    }

        //    return ipAddress;
        //}
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            //var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            //return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
            return string.CompareOrdinal(x, y);
        }
    }
}