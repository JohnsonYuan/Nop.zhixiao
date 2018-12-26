using System;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace Nop.Web.Infrastructure.Mvc.Routes
{
    ///<summary>
    /// Bonus app host route constraint
    ///</summary>
    public class BonusAppHostRouteConstraint : IRouteConstraint
    {
        private readonly Regex _host;

        public BonusAppHostRouteConstraint(string pattern)
        {
            _host = new Regex(pattern,
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            return _host.IsMatch(httpContext.Request.ServerVariables["HTTP_HOST"])
                && httpContext.Request.UserAgent.Contains("Chrome");  // ���ز��� ��֤����Edge���������
        }
    }
}