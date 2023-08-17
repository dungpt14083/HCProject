using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Network
{
    class NetworkConfig
    {
        private const string SETTING_ENVIRONMENT = "Setting_Environment";

        public IList<ServerEnvironment> Environments = new List<ServerEnvironment>()
    {
        new ServerEnvironment()
        {
            Name = "Dev",
            ServerPath = "",
            UserPoolId = "",
            AppClientId = "",
        },

        new ServerEnvironment()
        {
            Name = "Prod",
            ServerPath = "",
            UserPoolId = "",
            AppClientId = "",
    }
    };


        public int CurrentEnvironmentID
        {

            //temp hardcode
            get
            {
#if PROD
                return 1;
#else
                return 0;

#endif
            }
        } 


        public ServerEnvironment GetServerEnvironment()
        {
            return Environments[CurrentEnvironmentID];
        }
    }
}
