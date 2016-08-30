using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptONE.Infrastructure.Extensions
{
    public static class ExceptionExtension
    {

        #region Inner Messages (Extension)

        public static string GetInnerMessage(this Exception ex)
        {
            string result = string.Empty;

            if (ex.InnerException == null)
                result = ex.Message;
            else if (ex.InnerException.InnerException == null)
                result = ex.InnerException.Message;
            else
                result = ex.InnerException.InnerException.Message;

            return result;
        }

        public static Exception GetInnerException(this Exception ex)
        {
            if (ex.InnerException == null)
                return ex;
            else if (ex.InnerException.InnerException == null)
                return ex.InnerException;
            else
                return ex.InnerException.InnerException;
        }

        #endregion

    }

    public class CustomException : Exception
    {
        public CustomException(string message, params object[] parameters) : base(string.Format(message, parameters))
        { 
            
        }
    }
}
