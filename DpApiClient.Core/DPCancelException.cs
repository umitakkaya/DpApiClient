using System;
using System.Runtime.Serialization;

namespace DpApiClient.Core
{
    [Serializable]
    internal class DPCancelException : Exception
    {
        public DPCancelException()
        {
        }

        public DPCancelException(string message) : base(message)
        {
        }

        public DPCancelException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DPCancelException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}