using System;
using System.Runtime.Serialization;

namespace DpApiClient.Core
{
    [Serializable]
    internal class DpBookingException : Exception
    {
        public DpBookingException()
        {
        }

        public DpBookingException(string message) : base(message)
        {
        }

        public DpBookingException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DpBookingException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}