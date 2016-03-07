using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using DpApiClient.REST.DTO;

namespace DpApiClient.REST.Client
{
    class DPAuthenticator : IAuthenticator
    {
        private AuthorizationToken AuthorizationToken { get; set; }


        public DPAuthenticator(AuthorizationToken authorizationToken)
        {
            AuthorizationToken = authorizationToken;
        }

        public void Authenticate(IRestClient client, IRestRequest request)
        {
            client.AddDefaultHeader("Authorization", $"Bearer {AuthorizationToken.AccessToken}");
            
        }
    }
}
