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

        public DateTime ExpiresAt { get; private set; }

        private int _expiresIn = 0;
        public int ExpiresIn
        {
            get { return _expiresIn; }
            set
            {
                _expiresIn = value;
                ExpiresAt = DateTime.Now.AddSeconds(value);
            }
        }
    }
}
