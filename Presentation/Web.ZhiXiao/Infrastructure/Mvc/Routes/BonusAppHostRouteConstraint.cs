using System.Collections.Generic;
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
        private readonly List<Regex> _hostRegexes;

        public BonusAppHostRouteConstraint(string[] patterns)
        {
            _hostRegexes = new List<Regex>();

            foreach (var pattern in patterns)
            {
                _hostRegexes.Add(new Regex(
                    pattern,
                    RegexOptions.Compiled | RegexOptions.IgnoreCase));
            }
        }

        public bool Match(HttpContextBase httpContext, Route route, string parameterName, RouteValueDictionary values, RouteDirection routeDirection)
        {
            foreach (var regex in _hostRegexes)
            {

#if !DEBUG
                if (regex.IsMatch(httpContext.Request.ServerVariables["HTTP_HOST"]))
                {
                    return true;
                }
#else
                // ���ز��� ��֤����Edge���������
                if (regex.IsMatch(httpContext.Request.ServerVariables["HTTP_HOST"])
                    && httpContext.Request.UserAgent.Contains("Chrome"))    // ���ز��� ��֤����Edge���������
                {
                    return true;
                }
#endif
            }

            return false;
        }
    }
}