using System.Runtime.Serialization;

namespace CN.Server.Exceptions;

[Serializable]
public class CourierException : Exception
{
    public CourierException() { }
    public CourierException(string message) : base(message) { }
    public CourierException(string message, Exception inner) : base(message, inner) { }
    protected CourierException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
