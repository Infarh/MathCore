namespace WPFTest.Models
{
    public class ServiceMessage
    {
        public string Message { get; set; }

        public ServiceMessage() { }
        public ServiceMessage(string Message) => this.Message = Message;

        public static implicit operator ServiceMessage(string Message) => new ServiceMessage(Message);

        public static implicit operator string(ServiceMessage Message) => Message.Message;
    }
}
