using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SettingsApi {
    public class AuthorizeCheckOperationFilter : IOperationFilter {
        public void Apply (Operation operation, OperationFilterContext context) {
            // Check for authorize attribute
            context.ApiDescription.TryGetMethodInfo (out MethodInfo methodInfo);
            var hasAuthorize = methodInfo.GetCustomAttribute<AuthorizeAttribute> () != null;

            if (hasAuthorize) {
                operation.Responses.Add ("401", new Response { Description = "Unauthorized" });
                operation.Responses.Add ("403", new Response { Description = "Forbidden" });

                operation.Security = new List<IDictionary<string, IEnumerable<string>> > ();
                operation.Security.Add (new Dictionary<string, IEnumerable<string>> { { "oauth2", new [] { "settings" } }
                });
            }
        }
    }
}