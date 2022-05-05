using MexcTriangularArbitrage.Schema;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace MexcTriangularArbitrage
{
    public static class QueryExecutor
    {
        private static int _callCount = 0;
        private static readonly object _lockObj = new();
        private static Stopwatch _stopWatch;

        /// <summary>
        /// APIの呼び出し回数が20リクエスト/秒を超えないように調整する。
        /// この値はMEXC側の制限
        /// </summary>
        private static void EnsureRequestsRate()
        {
            lock (_lockObj)
            {
                if (_stopWatch == null)
                {
                    _stopWatch = new Stopwatch();
                    _stopWatch.Start();
                }
                if (_callCount >= 19)
                {
                    _stopWatch.Stop();
                    TimeSpan ts = _stopWatch.Elapsed;
                    int waitTime = Math.Max(1000 - (int)ts.TotalMilliseconds, 0);
                    Thread.Sleep(waitTime);
                    _stopWatch.Restart();
                    _callCount = 0;
                }

                _callCount++;
            }
        }

        public static string GetRequestParamString(SortedDictionary<string, string> param)
        {
            if (param?.Count == 0)
            {
                return "";
            }
            var paramStringList = param
                .Select(_ => $"{UrlEncode(_.Key)}={UrlEncode(_.Value)}")
                .ToList();
            return string.Join("&", paramStringList);
        }

        /**
         * signature
         */
        public static string GetSign(SignVo signVo)
        {
            string str = signVo.AccessKey + signVo.ReqTime + signVo.RequestParam;
            return ComputeActualSignature(str, signVo.SecretKey);
        }

        public static string ComputeActualSignature(string inputStr, string key)
        {
            var secretHmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            secretHmacSha256.Initialize();
            byte[] rawHmac = secretHmacSha256.ComputeHash(Encoding.UTF8.GetBytes(inputStr));
            var hmac = BitConverter.ToString(rawHmac).Replace("-", "").ToLower();
            return hmac;
        }

        private static string UrlEncode(string s)
        {
            return System.Web.HttpUtility.UrlEncode(s, Encoding.UTF8).Replace("\\+", "%20");
        }

        private static SignVo CreateNewSignVo(string requestParam)
        {
            return new SignVo
            {
                ReqTime = Utils.ToUtcUnixTimeMilliseconds(DateTime.Now),
                AccessKey = GlobalSetting.TokenConfig.AccessKey,
                SecretKey = GlobalSetting.TokenConfig.SecretKey,
                RequestParam = requestParam,
            };
        }

        private static TDataType ExecuteGetRequest<TDataType>(string url, SignVo signVo = null)
        {
            return Utils.RetryDo(() =>
            {
                var client = new HttpClient();
                if (signVo != null)
                {
                    client.DefaultRequestHeaders.Add("ApiKey", signVo.AccessKey);
                    client.DefaultRequestHeaders.Add("Signature", GetSign(signVo));
                    client.DefaultRequestHeaders.Add("Request-Time", signVo.ReqTime.ToString());
                }
                EnsureRequestsRate();
                var httpResponse = client.GetAsync(url).Result;
                var responseJson = httpResponse.Content.ReadAsStringAsync().Result;
                var responseData = JsonSerializer.Deserialize<MexcResponseData<TDataType>>(responseJson);
                return responseData.data;
            });
        }

        private static TDataType ExecutePostRequest<TDataType>(string url, string bodyJson, SignVo signVo = null)
        {
            return Utils.RetryDo(() =>
            {
                var client = new HttpClient();
                if (signVo != null)
                {
                    client.DefaultRequestHeaders.Add("ApiKey", signVo.AccessKey);
                    client.DefaultRequestHeaders.Add("Signature", GetSign(signVo));
                    client.DefaultRequestHeaders.Add("Request-Time", signVo.ReqTime.ToString());
                }
                EnsureRequestsRate();

                var content = new StringContent(bodyJson, Encoding.UTF8, @"application/json");
                var httpResponse = client.PostAsync(url, content).Result;
                var responseJson = httpResponse.Content.ReadAsStringAsync().Result;
                var responseData = JsonSerializer.Deserialize<MexcResponseData<TDataType>>(responseJson);
                return responseData.data;
            });
        }

        #region public API

        public static HashSet<SymbolData> GetAllTargetSymbols()
        {
            return GetAllSymbols()
                .Where(_ => _.state == "ENABLED" && _.etf_mark == 0)
                .ToHashSet();
        }

        public static HashSet<SymbolData> GetAllSymbols()
        {
            return ExecuteGetRequest<HashSet<SymbolData>>("https://www.mexc.com/open/api/v2/market/symbols");
        }

        public static List<SymbolTicker> GetSymbolTickerInformation()
        {
            return ExecuteGetRequest<List<SymbolTicker>>("https://www.mexc.com/open/api/v2/market/ticker");
        }

        public static MarketDepth GetMarketDepth(string symbol, int depth = 50)
        {
            return ExecuteGetRequest<MarketDepth>($"https://www.mexc.com/open/api/v2/market/depth?symbol={symbol}&depth={depth}");
        }
        #endregion

        #region private API
        public static BalanceDictionary GetAccountBalance()
        {
            var signVo = CreateNewSignVo("");
            return ExecuteGetRequest<BalanceDictionary>($"https://www.mexc.com/open/api/v2/account/info", signVo);
        }

        public static List<OpenOrder> GetOpenOrders(string symbol, int limit = 50)
        {
            var param = new SortedDictionary<string, string>
            {
                { "symbol", symbol },
                { "limit", limit.ToString() }
            };
            var requestParam = GetRequestParamString(param);
            var signVo = CreateNewSignVo(requestParam);
            return ExecuteGetRequest<List<OpenOrder>>($"https://www.mexc.com/open/api/v2/order/open_orders?{requestParam}", signVo);
        }

        public static BalanceDictionary GetBalanceDictionary(string symbol, int limit = 50)
        {
            var param = new SortedDictionary<string, string>
            {
                { "symbol", symbol },
                { "limit", limit.ToString() }
            };
            var requestParam = GetRequestParamString(param);
            var signVo = CreateNewSignVo(requestParam);
            return ExecuteGetRequest<BalanceDictionary>($"https://www.mexc.com/open/api/v2/order/open_orders?{requestParam}", signVo);
        }

        public static string PostPlaceOrder(PlaceOrderPostData postData)
        {
            var json = JsonSerializer.Serialize(postData);
            var signVo = CreateNewSignVo(json);

            return ExecutePostRequest<string>("https://www.mexc.com/open/api/v2/order/place", json, signVo);
        }

        public static List<DealHistory> GetDealHistory(string symbol, int limit = 50)
        {
            var param = new SortedDictionary<string, string>
            {
                { "symbol", symbol },
                { "limit", limit.ToString() }
            };
            var requestParam = GetRequestParamString(param);
            var signVo = CreateNewSignVo(requestParam);
            return ExecuteGetRequest<List<DealHistory>>($"https://www.mexc.com/open/api/v2/order/deals?{requestParam}", signVo);
        }
        #endregion
    }
}
