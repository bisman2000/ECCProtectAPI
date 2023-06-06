
namespace AT150732.Business.Exceptions
{
    [Serializable]
    public class ContactNotFoundException : Exception
    {
        public int Id { get; }

        public ContactNotFoundException()
        {
        }

        public ContactNotFoundException(int id)
        {
            Id = id;
        }

        public ContactNotFoundException(string? message) : base(message)
        {
        }

        public ContactNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ContactNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}