using DpApiClient.REST.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.REST.Client
{
    public sealed class Globals
    {

        private static object locker = new object();
        private static Dictionary<string, AuthorizationToken> _tokenStorage = new Dictionary<string, AuthorizationToken>();
        public static Dictionary<string, AuthorizationToken> TokenStorage
        {
            get
            {
                lock (locker)
                {
                    return _tokenStorage;
                }
            }
            internal set
            {
                lock (locker)
                {
                    _tokenStorage = value;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static AuthorizationToken GetToken(string clientId)
        {
            AuthorizationToken token = null;
            _tokenStorage.TryGetValue(clientId, out token);
            return token;
        }

        public static void SetToken(string clientId, AuthorizationToken token)
        {
            if (_tokenStorage.ContainsKey(clientId))
            {
                _tokenStorage[clientId] = token;
            }
            else
            {
                _tokenStorage.Add(clientId, token);
            }
        }

    }
}
