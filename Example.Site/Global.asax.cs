using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using System.IO;
using System.Configuration;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static Example.Site.TraceUtil;


namespace Example.Site
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Codice eseguito all'avvio dell'applicazione
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Rimuovo il serializzatore in XML dato che tutti i messaggi devono essere in JSON per non causare errori nel TRACE
            GlobalConfiguration.Configuration.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpResponse response = HttpContext.Current.Response;
            OutputFilterStream filter = new OutputFilterStream(response.Filter);
            response.Filter = filter;
            HttpContext.Current.Items.Add("logFilter", filter);

            switch (TraceUtil.TraceIsEnabled())
            {
                case TraceUtil.TraceMode.ON:
                    NLog.LogManager.GetCurrentClassLogger().Trace(String.Format("REQUEST[{0}]({1}) - [{2}] {3}",
                        filter.GuidRequest,
                        HttpContext.Current.Request.UserHostAddress,
                        HttpContext.Current.Request.HttpMethod,
                        HttpContext.Current.Request.Url));
                    break;
                case TraceUtil.TraceMode.DATA:
                    string content = RequestBody();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        NLog.LogManager.GetCurrentClassLogger().Trace(String.Format("REQUEST[{0}]({1}) - [{2}] {3}",
                            filter.GuidRequest,
                            HttpContext.Current.Request.UserHostAddress,
                            HttpContext.Current.Request.HttpMethod,
                            content));
                    }
                    break;
            }
        }

        private string RequestBody()
        {
            try
            {
                string traceRequest = HttpContext.Current.Request.Form.ToString();

                if (String.IsNullOrEmpty(traceRequest))
                {
                    var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
                    if (bodyStream.BaseStream.Length <= 0) return HttpContext.Current.Request.Url.ToString();

                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                    var bodyText = bodyStream.ReadToEnd();
                    bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);

                    dynamic dynBodyText = JsonConvert.DeserializeObject(bodyText);
                    HttpContext.Current.Items.Add("DeviceCode", dynBodyText.Customer.DeviceCode);
                    HttpContext.Current.Items.Add("GdSession", dynBodyText.GdSession?.Value ?? "GdSessionUnitialized");

                    return HttpContext.Current.Request.Url + "?" + ((JObject)dynBodyText).ToStringLog();
                }
                else
                {
                    return HttpContext.Current.Request.Url + " - FormData:[" + HttpContext.Current.Request.Form.ToStringLog() + "]";
                }
            }
            catch
            {
                return HttpContext.Current.Request.Url + "?" + HttpContext.Current.Request.Form ?? "";
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            HttpResponse response = HttpContext.Current.Response;
            OutputFilterStream filter = (OutputFilterStream)HttpContext.Current.Items["logFilter"];

            switch (TraceUtil.TraceIsEnabled())
            {
                case TraceUtil.TraceMode.ON:
                    NLog.LogManager.GetCurrentClassLogger().Trace(String.Format("RESPONSE.{0}[{1}] - [{2}] {3}",
                        response.StatusCode.ToString(),
                        filter.GuidRequest,
                        ((HttpApplication)sender).Request.HttpMethod,
                        ((HttpApplication)sender).Request.Url));
                    break;
                case TraceUtil.TraceMode.DATA:
                    if (filter.CopyStreamLength > Int64.Parse(ConfigurationManager.AppSettings["traceResponseMaxSize"]))
                    {
                        NLog.LogManager.GetCurrentClassLogger().Trace(String.Format("RESPONSE.{0}[{1}] - Big response skipped from trace.",
                            response.StatusCode.ToString(),
                            filter?.GuidRequest ?? "N.A."));
                    }
                    else if (response.ContentType.Contains("application/json"))
                    {
                        dynamic dynBodyText = JsonConvert.DeserializeObject(filter.ReadStream());
                        NLog.LogManager.GetCurrentClassLogger().Trace(String.Format("RESPONSE.{0}[{1}] - {2}",
                            response.StatusCode.ToString(),
                            filter?.GuidRequest ?? "N.A.",
                            ((JToken)dynBodyText).ToStringLog()));
                    }
                    break;
            }
        }
    }
}