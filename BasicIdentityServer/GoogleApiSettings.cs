using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityServer
{
    public class GoogleApiSettings
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }

        public GoogleApiSettings()
        {
        }
    }
}
