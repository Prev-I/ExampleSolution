using System;
using System.Threading;
using System.Web.Http.Controllers;

using NLog;


namespace Example.Site.Filters
{
    /// <summary>
    /// Custom Authentication Filter Extending basic Authentication
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class FakeAuthenticationFilter : BasicAuthenticationFilter
    {
        private string _controller { get; set; }
        /// <summary>
        /// Default Authentication Constructor
        /// </summary>
        public FakeAuthenticationFilter(string controller = null)
        {
            _controller = controller;
        }

        /// <summary>
        /// AuthenticationFilter constructor with isActive parameter
        /// </summary>
        /// <param name="isActive"></param>
        public FakeAuthenticationFilter(bool isActive)
            : base(isActive)
        {
        }

        /// <summary>
        /// Protected overriden method for authorizing user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="actionContext"></param>
        /// <returns></returns>
        protected override bool OnAuthorizeUser(string username, string password, HttpActionContext actionContext)
        {
            try
            {
                int userId = 0;

                //TODO: could use a library to realy check data on DB
                if(username == "USER" && password == "PASSWD")
                {
                    userId = 1;
                }
                if (userId > 0)
                {
                    if (Thread.CurrentPrincipal.Identity is BasicAuthenticationIdentity authenticationIdentity)
                        authenticationIdentity.UserId = userId;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                LogManager.GetCurrentClassLogger().Error(ex);
                return false;
            }
        }
    }
}