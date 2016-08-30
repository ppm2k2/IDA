using System;
using System.Net;

namespace ConceptONE.Infrastructure.Web
{
    public class WebClientWithTimeout : WebClient
    {
        private int _TimeoutMinutes = 1;

        public WebClientWithTimeout(int timeoutMinutes)
        {
            _TimeoutMinutes = timeoutMinutes;
        }

        protected override WebRequest GetWebRequest(Uri uri)
        {
            WebRequest result = base.GetWebRequest(uri);
            result.Timeout = _TimeoutMinutes * 60 * 1000;

            return result;
        }
    }
}
