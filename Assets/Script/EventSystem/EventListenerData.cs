namespace EventSystem
{
    public class EventListenerData
    {
        public EventListenerData(object aEventListener,
                                 string aEventNameString,
                                 EventDelegate aEventDelegate,
                                 EventDispatcherAddMode aEventListeningMode)
        {
            EventListener = aEventListener;
            EventName = aEventNameString;
            EventDelegate = aEventDelegate;
            EventListeningMode = aEventListeningMode;
            Enable = true;
        }

        public bool Enable { get; set; }
        public EventDelegate EventDelegate { get; set; }
        public object EventListener { get; set; }
        public EventDispatcherAddMode EventListeningMode { get; set; }
        public string EventName { get; set; }
    }
}