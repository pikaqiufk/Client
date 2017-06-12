namespace EventSystem
{
    public interface IEventDispatcher
    {
        // PUBLIC
        /// <summary>
        ///     Adds the event listener.
        /// </summary>
        /// <returns><c>true</c>, if event listener was added, <c>false</c> otherwise.</returns>
        /// <param name="aEventTypeString">A event type_string.</param>
        /// <param name="aEventDelegate">A event delegate.</param>
        bool AddEventListener(string aEventTypeString, EventDelegate aEventDelegate);

        /// <summary>
        ///     Adds the event listener.
        /// </summary>
        /// <returns><c>true</c>, if event listener was added, <c>false</c> otherwise.</returns>
        /// <param name="aEventTypeString">A event type_string.</param>
        /// <param name="aEventDelegate">A event delegate.</param>
        /// <param name="eventDispatcherAddMode">Event dispatcher add mode.</param>
        bool AddEventListener(string aEventTypeString,
                              EventDelegate aEventDelegate,
                              EventDispatcherAddMode eventDispatcherAddMode);

        /// <summary>
        ///     Dispatch the event.
        /// </summary>
        /// <returns><c>true</c>, if event was dispatched, <c>false</c> otherwise.</returns>
        /// <param name="aIEvent">A I event.</param>
        bool DispatchEvent(IEvent aIEvent);

        /// <summary>
        ///     Has the event listener.
        /// </summary>
        /// <returns><c>true</c>, if event listener was hased, <c>false</c> otherwise.</returns>
        /// <param name="aEventNameString">A event type_string.</param>
        /// <param name="aEventDelegate">A event delegate.</param>
        bool HasEventListener(string aEventNameString, EventDelegate aEventDelegate);

        /// <summary>
        ///     Removes all event listeners.
        /// </summary>
        /// <returns><c>true</c>, if all event listeners was removed, <c>false</c> otherwise.</returns>
        bool RemoveAllEventListeners();

        /// <summary>
        ///     Removes the event listener.
        /// </summary>
        /// <returns><c>true</c>, if event listener was removed, <c>false</c> otherwise.</returns>
        /// <param name="aEventNameString">A event type_string.</param>
        /// <param name="aEventDelegate">A event delegate.</param>
        bool RemoveEventListener(string aEventNameString, EventDelegate aEventDelegate);
    }
}