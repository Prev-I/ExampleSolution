using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace Example.Site
{
    public static class TraceExt
    {
        public static string ToStringJson(this object obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None);
        }

        public static string ToStringLog(this NameValueCollection obj)
        {
            NameValueCollection log = new NameValueCollection(obj);

            //List<string> logObfuscatorFieldsMd5 = ConfigurationManager.AppSettings["logObfuscatorFieldsMd5"].Split(',').ToList<string>();
            //List<string> logObfuscatorFieldsStar = ConfigurationManager.AppSettings["logObfuscatorFieldsStar"].Split(',').ToList<string>();

            //foreach (string itm in logObfuscatorFieldsMd5)
            //{
            //    string value = log[itm];
            //    if (value.IsValid())
            //        value = value.ComputeHash_MD5();

            //    log[itm] = value;
            //}

            //foreach (string itm in logObfuscatorFieldsStar)
            //{
            //    string value = log[itm];
            //    if (value.IsValid())
            //        value = new string('*', value.Length);

            //    log[itm] = value;
            //}

            return string.Join("&", log.AllKeys.Select(key => string.Format("{0}={1}", key, log[key])));
        }

        public static string ToStringLog(this object obj)
        {
            string log = obj.ToStringJson();

            //string logObfuscatorFieldsMd5 = ConfigurationManager.AppSettings["logObfuscatorFieldsMd5"];
            //string logObfuscatorFieldsStar = ConfigurationManager.AppSettings["logObfuscatorFieldsStar"];

            //if (!string.IsNullOrWhiteSpace(logObfuscatorFieldsMd5))
            //{
            //    foreach (string itm in logObfuscatorFieldsMd5.Split(',').ToList<string>())
            //        log = Regex.Replace(log, "(" + itm + "\":\")(.+?)\"", m => m.Groups[1].Value + m.Groups[2].Value.ComputeHash_MD5() + "\"");
            //}

            //if (!string.IsNullOrWhiteSpace(logObfuscatorFieldsStar))
            //{
            //    foreach (string itm in logObfuscatorFieldsStar.Split(',').ToList<string>())
            //        log = Regex.Replace(log, "(" + itm + "\":\")(.+?)\"", m => m.Groups[1].Value + new string('*', m.Groups[2].Value.Length) + "\"");
            //}

            return log;
        }
    }
}
