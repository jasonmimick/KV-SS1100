using System;
using System.Collections.Generic;
using System.Text;
using PluginExample;
using Panasonic.KV_SS1100.API.Account;

namespace PluginExample
{
    /// <summary>
    /// Implements an account handler that contains a single user account called "Test Account".
    /// </summary>
    class MyAccountHandler : IAccountHandler
    {
        private AccountEntry theAccount;

        public MyAccountHandler()
        {
            theAccount = new AccountEntry(this, "Test Account", "testacct@example.com");
        }

        #region IAccountHandler Members

        public string Id
        {
            get { return "My Account Handler"; }
        }

        public bool SupportsAsyncSearch
        {
            get
            {
                return false;
            }
        }

        public void SearchAsync(string queryString, ISearchResultReceiver resultReceiver, int maxEntries, int serverTimeout, AccountSearchType searchType)
        {
            throw new NotSupportedException();
        }

        public AccountEntry[] Search(string queryString, int maxEntries, AccountSearchType searchType)
        {
            if (searchType != AccountSearchType.Any &&
                searchType != AccountSearchType.WithMailAddress)
            {
                return new AccountEntry[0];
            }

            if (maxEntries < 1 || (!theAccount.CommonName.Contains(queryString)
                                   && !theAccount.Email.Contains(queryString)))
            {
                return new AccountEntry[0];
            }
            else
            {
                // Only return the test account if it matches the query string.
                return new AccountEntry[] { theAccount };
            }
        }

        #endregion
    }
}
