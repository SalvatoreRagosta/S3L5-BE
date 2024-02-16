using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ecommerce.Helpers
{
    [Serializable]
    public sealed class SessionSingleton
    {
        // region Singleton

        private const string session_name = "ecommerce_session";

        private SessionSingleton()
        {

        }

        public static SessionSingleton Current
        {
            get
            {
                if (HttpContext.Current.Session[session_name] == null)
                {
                    HttpContext.Current.Session[session_name] = new SessionSingleton();
                }

                return HttpContext.Current.Session[session_name] as SessionSingleton;
            }
        }

        public Dictionary<int, string> Cart { get; set; }

    }
}