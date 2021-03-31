namespace XSNotifications.Exception
{
    public class XSRuntimeException : System.Exception
    {
        public XSRuntimeException() { }
        public XSRuntimeException(string message) : base(message) { }
        public XSRuntimeException(string message, System.Exception inner) : base(message, inner) { }
        public XSRuntimeException(System.Exception ex) : base(ex.Message, ex.InnerException) { }
    }
}
