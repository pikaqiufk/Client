namespace EventSystem
{
    /// <summary>
    ///     Event base
    /// </summary>
    public class EventBase : IEvent
    {
        public EventBase(string aTypeStr)
        {
            mTypeString = aTypeStr;
        }

        private string mTypeString;

        string IEvent.Type
        {
            get { return mTypeString; }
            set { mTypeString = value; }
        }

        object IEvent.Target { get; set; }
    }
}