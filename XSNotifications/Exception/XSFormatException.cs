namespace XSNotifications.Exception
{
    public class XSFormatException : System.Exception
    {
        public XSFormatException() { }
        public XSFormatException(string message) : base(message) { }
        public XSFormatException(string message, System.Exception inner) : base(message, inner) { }
        public XSFormatException(System.Exception ex) : base(ex.Message, ex.InnerException) { }
    }
}
