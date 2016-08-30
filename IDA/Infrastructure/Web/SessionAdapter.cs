using System.Web;

namespace ConceptONE.Infrastructure.Web
{
    public class SessionAdapter
    {

        public static T GetValue<T>(string key)
        {
            T result = default(T);

            if (HttpSession[key] != null)
                result = (T)HttpSession[key];

            return result;
        }

        public static void Delete(string key)
        {
            if (HttpSession[key] != null)
                HttpSession[key] = null;
        }

        public static bool Contains(string key)
        {
            bool result = (HttpSession[key] != null);
            return result;
        }

        public static void SetValue<T>(string key, T value)
        {
            HttpSession[key] = value;
        }

        private static HttpSessionStateBase HttpSession
        {
            get
            {
                HttpSessionStateBase result = null;

                if (HttpContext.Current != null && HttpContext.Current.Session != null)
                    result = new HttpSessionStateWrapper(HttpContext.Current.Session);

                return result;
            }
        }

    }
}