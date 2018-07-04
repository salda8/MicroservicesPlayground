using System;

namespace BasicIdentityServer.Services
{
    public class EmailSenderOptions
    {
        public String SendGridUser { get; set; }
        public string SendGridKey { get; set; }
    }
}