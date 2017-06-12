namespace EventSystem
{
    //--------------------------------------
    //  Class
    //--------------------------------------
    /// <summary>
    ///     Test event.
    /// </summary>
    public class Event : IEvent
    {
        public Event(string aTypeStr)
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