using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;


namespace Example.Site.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class BasicAuthenticationFilter : AuthorizationFilterAttribute
    {

        /// <summary>
        /// Public default Constructor
        /// </summary>
        public BasicAuthenticationFilter()
        {
        }

        private readonly bool _isActive = true;

        /// <summary>
        /// parameter isActive explicitly enables/disables this filetr.
        /// </summary>
        /// <param name="isActive"></param>
        public BasicAuthenticationFilter(bool isActive)
        {
            _isActive = isActive;
        }

        /// <summary>
        /// Checks basic authentication request
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!_isActive) return;
            var identity = FetchAuthHeader(actionContext);
            if (identity == null)
            {
                ChallengeAuthRequest(actionContext);
                return;
            }
            var genericPrincipal = new GenericPrincipal(identity, null);
            Thread.CurrentPrincipal = genericPrincipal;
            if (!OnAuthorizeUser(identity.Name, identity.Password, actionContext))
            {
                ChallengeAuthRequest(actionContext);
                return;
            }
            base.OnAuthorization(actionContext);
        }

        /// <summary>
        /// Virtual method.Can be overriden with the custom Authorization.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected virtual bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;
            return true;
        }

        /// <summary>
        /// Checks for autrhorization header in the request and parses it, creates user credentials and returns as BasicAuthenticationIdentity
        /// </summary>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected virtual BasicAuthenticationIdentity FetchAuthHeader(HttpActionContext actionContext)
        {
            string authHeaderValue = null;
            var authRequest = actionContext.Request.Headers.Authorization;
            if (authRequest != null && !string.IsNullOrEmpty(authRequest.Scheme) && authRequest.Scheme == "Basic")
                authHeaderValue = authRequest.Parameter;
            if (string.IsNullOrEmpty(authHeaderValue))
                return null;
            authHeaderValue = Encoding.Default.GetString(Convert.FromBase64String(authHeaderValue));
            var credentials = authHeaderValue.Split(':');
            return credentials.Length < 2 ? null : new BasicAuthenticationIdentity(credentials[0], credentials[1]);
        }


        /// <summary>
        /// Send the Authentication Challenge request
        /// </summary>
        /// <param name="actionContext"></param>
        private static void ChallengeAuthRequest(HttpActionContext actionContext)
        {
            var dnsHost = actionContext.Request.RequestUri.DnsSafeHost;
            actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
            actionContext.Response.Headers.Add("WWW-Authenticate", string.Format("Basic realm=\"{0}\"", dnsHost));
        }
    }
}