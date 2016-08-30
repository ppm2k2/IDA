using System;
using System.Collections.Generic;
using System.Linq;
using IDADataAccess.Parameters;
using IDALibrary.Constants;

namespace IDALibrary.Services
{
    public class AccountService : IDisposable
    {
        private ParametersContext parameters;

        public AccountService(ParametersContext parameters)
        {
            this.parameters = parameters;
        }

        public bool IsAuthorized(string windowsUsername, List<string> rolesToCheck)
        {
            bool result = false;

            string role = GetUserRole(windowsUsername);

            if (rolesToCheck.Contains(role) || role == Roles.SUPER_USER)
                result = true;             

            return result;
        }

        public string GetUserRole(string windowsUsername)
        {
            string result = null;

            var data = (from d in parameters.ParametersDataAppUsers.AsNoTracking()
                        where d.WindowsUsername.Equals(windowsUsername, StringComparison.InvariantCultureIgnoreCase)
                        select d).FirstOrDefault();

            if (data != null)
            {
                result = data.Role;
            }

            return result;
        }

        public void Dispose()
        {
            parameters.Dispose();
        }
    }
}