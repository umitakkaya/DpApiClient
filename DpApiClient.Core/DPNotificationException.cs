using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DpApiClient.Core
{
        [Serializable]
    internal class DPNotificationException : Exception
    {
        public DPNotificationException()
        {
        }

        public DPNotificationException(string message) : base(message)
        {
        }

        public DPNotificationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DPNotificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
