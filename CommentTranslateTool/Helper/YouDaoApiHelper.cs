﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Workshop.Model;
using Workshop.Service;

namespace Workshop.Helper
{
    public class YouDaoApiHelper
    {
        public YouDaoApiHelper()
        {
        }


        public static List<YouDaoTranslationType> TranslationTypes => new List<YouDaoTranslationType>(
            )
        {
            new YouDaoTranslationType()
            {
                Name = "自动检测",
                Type = "auto"
            },
            new YouDaoTranslationType()
            {
                Name = "英文",
                Type = "en"
            },
            new YouDaoTranslationType()
            {
                Name = "中文",
                Type = "zh-CHS"
            },
            new YouDaoTranslationType()
            {
                Name = "日文",
                Type = "ja"
            },
            new YouDaoTranslationType()
            {
                Name = "俄文",
                Type = "ru"
            },
            new YouDaoTranslationType()
            {
                Name = "法文",
                Type = "fr"
            },

        };

        public static async Task<YouDaoTranslationData> GetWordsAsync(string queryText)
        {
            var requestUrl = GetRequestUrl(queryText);

            WebRequest translationWebRequest = WebRequest.Create(requestUrl);

            var response = await translationWebRequest.GetResponseAsync();

            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream ?? throw new InvalidOperationException(), Encoding.GetEncoding("utf-8")))
                {
                    string result = reader.ReadToEnd();
                    var youDaoTranslationResponse = JsonConvert.DeserializeObject<YouDaoTranslationResponse>(result);

                    return new YouDaoTranslationData { ResultDetail = result, YouDaoTranslation = youDaoTranslationResponse };
                }
            }
        }

        private static string GetRequestUrl(string queryText)
        {
            var settingInfo = LocalDataService.ReadObjectLocal<SettingInfo>();
            var _from = settingInfo.TranslateFrom.Type;
            var _to = settingInfo.TranslateTo.Type;
            string salt = DateTime.Now.Millisecond.ToString();

            MD5 md5 = new MD5CryptoServiceProvider();
            string md5Str = ApiKeys._appKey + queryText + salt + ApiKeys._appSecret;
            byte[] output = md5.ComputeHash(Encoding.UTF8.GetBytes(md5Str));
            string sign = BitConverter.ToString(output).Replace("-", "");

            var requestUrl = string.Format(
                "http://openapi.youdao.com/api?appKey={0}&q={1}&from={2}&to={3}&sign={4}&salt={5}",
ApiKeys._appKey,
                queryText,
                _from, _to, sign, salt);

            return requestUrl;
        }
    }

    public class YouDaoTranslationData
    {
        public YouDaoTranslationResponse YouDaoTranslation { get; set; }
        public string ResultDetail { get; set; }
    }
}
