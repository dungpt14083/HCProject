using Salday.EventBus;

namespace Assets.Demo
{
    //Class that wil hold data about chat event (message)
    public class ChatEvArgs : CancelableEventBase
    {
        public string Message { get; set; }
    }
}
