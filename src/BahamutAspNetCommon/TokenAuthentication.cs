using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using BahamutService;
using Microsoft.AspNetCore.Http;
using NLog;
using System.Net;

namespace BahamutAspNetCommon
{
    // You may need to install the Microsoft.AspNet.Http.Abstractions package into your project
    public class TokenAuthentication
    {
        public static IDictionary<string, bool> NoAuthenticationRoutes { private set; get; }
        private readonly RequestDelegate _next;
        public string Appkey { get; private set; }
        public TokenService TokenService { get; private set; }

        public TokenAuthentication(RequestDelegate next, string appkey, TokenService tokenService, IEnumerable<string> openRoutes = null)
        {
            NoAuthenticationRoutes = new Dictionary<string, bool>();
            if(openRoutes != null && openRoutes.Count() > 0)
            {
                foreach (var route in openRoutes)
                {
                    NoAuthenticationRoutes[route] = true;
                }
            }
            Appkey = appkey;
            _next = next;
            TokenService = tokenService;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var reqPath = httpContext.Request.Path;
            if (NoAuthenticationRoutes.Keys.Contains(reqPath))
            {
                LogManager.GetLogger("Route").Info("account:{0},request:{1}", httpContext.Request.Query["accountId"], reqPath);
                return _next(httpContext);
            }
            var userId = httpContext.Request.Headers["userId"];
            var token = httpContext.Request.Headers["token"];
            var appkey = httpContext.Request.Headers["appkey"];
            LogManager.GetLogger("Route").Info("user:{0},request:{1}", userId, reqPath);
            if (string.IsNullOrWhiteSpace(appkey))
            {
                appkey = Appkey;
            }
            try
            {
                var res = TokenService.ValidateAppToken(appkey, userId, token).Result;
                if (res != null)
                {
                    httpContext.Items.Add("AccountSessionData", res);
                    httpContext.Request.Headers["accountId"] = res.AccountId;
                    return _next(httpContext);
                }
                else
                {
                    LogManager.GetLogger("Route").Info("Validate Failed -> userId:{0}", userId);
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogManager.GetLogger("Error").Error(ex);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return null;
            }
        }
    }
}
