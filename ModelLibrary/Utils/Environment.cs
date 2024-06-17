using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelLib.Utils
{
    public static class EnvironmentHandler
    {
        public enum Environment
        {
            Production,
            Development,
            Local
        }

        public static Environment GetEnvironment(IConfigurationRoot config) {
            var env = config["ASPNETCORE_ENVIRONMENT"];
            return Enum.Parse<Environment>(env);
        }
    }
}
