using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.DTO
{
    public class AuthorizationToken
    {

        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public string Scope { get; set; }
        public int ExpiresIn { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
