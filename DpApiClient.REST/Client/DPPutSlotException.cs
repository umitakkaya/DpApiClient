using System;
using System.Runtime.Serialization;

namespace DpApiClient.REST.Client
{
    [Serializable]
    internal class DPPutSlotException : Exception
    {
        public DPPutSlotException()
        {
        }

        public DPPutSlotException(string message) : base(message)
        {
        }

        public DPPutSlotException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DPPutSlotException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}