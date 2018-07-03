using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityServer.Models.ManageViewModels
{
    public class EnableAuthenticatorViewModel
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
        public string Code { get; set; }
    }
}
