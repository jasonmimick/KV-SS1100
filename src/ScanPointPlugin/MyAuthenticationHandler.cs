using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Panasonic.KV_SS1100.API.Account;
using Panasonic.KV_SS1100.API.Authentication;

namespace PluginExample
{
    /// <summary>
    /// Handles authentication for username "johndoe".
    /// </summary>
    public class MyAuthenticationHandler : IAuthenticationHandler
    {
        #region IAuthenticationHandler Members

        public string Id
        {
            get { return "My Authentication Handler"; }
        }

        public void LogOff(AccountEntry entry)
        {
            // TODO: Handle log off here. In our case, there's nothing to do.
        }

        public AccountEntry LogOn(string userName, string password)
        {
            if (userName == "johndoe" && password == "12345")
            {
                // Login is successful.
                // Return the account information.
                AccountEntry entry = new AccountEntry(this, "johndoe", "12345", false);
                return entry;
            }

            return null;
        }

        #endregion
    }
}
