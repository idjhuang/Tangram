using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace TangramCMS.Infrastructure
{
    public static class JsonUtil
    {
        public static string ConvertObjectId(this string jsonStr)
        {
            return Regex.Replace(jsonStr, @"ObjectId\(""\w+""\)", delegate(Match match)
            {
                var matchedStr = match.ToString();
                var result = matchedStr.Substring(9, matchedStr.Length - 10);
                return result;
            });
        }

        public static string ConvertIsoDate(this string jsonStr)
        {
            return Regex.Replace(jsonStr, @"ISODate\(""\w+-\w+-\w+T\w+:\w+:\w+""\)", delegate(Match match)
            {
                var matchedStr = match.ToString();
                var dateStr = matchedStr.Substring(9, matchedStr.Length - 11);
                var result = dateStr.Replace("-", "/").Replace("T", " ").Replace("Z", "");
                return string.Format("\"{0}\"", result);
            });
        }
    }
}