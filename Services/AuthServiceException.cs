namespace CrocoManager.Services
{
    [Serializable]
    public class AuthServiceException : Exception
    {
        public AuthServiceException() { }
        public AuthServiceException(string message) : base(message) { }
        public AuthServiceException(string message, Exception inner) : base(message, inner) { }
    }
}
