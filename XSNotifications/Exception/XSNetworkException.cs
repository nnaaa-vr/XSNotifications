namespace XSNotifications.Exception
{
    public class XSNetworkException: System.Exception
    {
        public XSNetworkException() { }
        public XSNetworkException(string message) : base(message) { }
        public XSNetworkException(string message, System.Exception inner) : base(message, inner) { }
        public XSNetworkException(System.Exception ex) : base(ex.Message, ex.InnerException) { }
    }
}
