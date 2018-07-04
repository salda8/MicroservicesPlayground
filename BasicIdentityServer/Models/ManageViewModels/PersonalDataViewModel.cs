using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicIdentityServer.Models.ManageViewModels
{
    public class PersonalDataViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class DeletePersonalDataViewModel
    {
        public bool RequirePassword { get; set; } = false;
    }

    public class DownloadPersonalDataViewModel
    {
        
    }
}
