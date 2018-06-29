using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BasicIdentityServer.Models.ManageViewModels
{
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }
    }
}
