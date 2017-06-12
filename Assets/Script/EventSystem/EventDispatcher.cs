#region using

using System;
using System.Collections.Generic;

#endregion

namespace EventSystem
{
    public delegate void EventDelegate(IEvent iEvent);

    public enum EventDispatcherAddMode
    {
        DEFAULT,
        SINGLE_SHOT
    }

    public class EventDispatcher : IEventDispatcher
    {
        private static EventDispatcher _instance;

        public EventDispatcher(object aTargetObject)
        {
            mTargetObject = aTargetObject;
        }

        public EventDispatcher()
        {
            mTargetObject = this;
        }

        private readonly Queue<Action> mAppendAction = new Queue<Action>();
        private string mCurrentEventName = string.Empty;

        private readonly Dictionary<string, Dictionary<string, EventListenerData>> mEventListenerDatasHashtable =
            new Dictionary<string, Dictionary<string, EventListenerData>>();

        private readonly object mTargetObject;

        public static EventDispatcher Instance
        {
            get { return _instance ?? (_instance = new EventDispatcher()); }
        }

        /// <summary>
        ///     _dos the add target value to I event.
        /// </summary>
        /// <param name='aIEvent'>
        ///     A I event.
        /// </param>
        /// <exception cref='System.NotImplementedException'>
        ///     Is thrown when a requested operation is not implemented for a given type.
        /// </exception>
        private void _doAddTargetValueToIEvent(IEvent aIEvent)
        {
            aIEvent.Target = mTargetObject;
        }

        private object _getArgumentsCallee(EventDelegate aEventDelegate)
        {
            if (aEventDelegate == null)
            {
                return null;
            }
            return aEventDelegate.Target;
        }

        private string _getKeyForInnerHashTable(EventListenerData aEventListenerData)
        {
            //VERY UNIQUE - NICE!
            return aEventListenerData.EventListener.GetType().FullName + "_" +
                   aEventListenerData.EventListener.GetHashCode() + "_" + aEventListenerData.EventName + "_" +
                   aEventListenerData.EventDelegate.Method.Name;
        }

        private string _getKeyForOuterHashTable(string aEventName_string)
        {
            //SIMPLY USING THE EVENT NAME - METHOD USED HERE, IN CASE I WANT TO TWEAK THIS MORE...
            return aEventName_string;
        }

        public void OnApplicationQuit()
        {
            //TODO, DO THIS CLEANUP HERE, OR OBLIGATE API-USER TO DO IT??
            mEventListenerDatasHashtable.Clear();
        }

        /// <summary>
        ///     Adds the event listener.
        /// </summary>
        /// <returns>
        ///     The event listener.
        /// </returns>
        /// <param name='aEventNameString'>
        ///     If set to <c>true</c> a event name_string.
        /// </param>
        /// <param name='aEventDelegate'>
        ///     If set to <c>true</c> a event delegate.
        /// </param>
        public bool AddEventListener(string aEventNameString, EventDelegate aEventDelegate)
        {
            return AddEventListener(aEventNameString, aEventDelegate, EventDispatcherAddMode.DEFAULT);
        }

        /// <summary>
        ///     Adds the event listener.
        /// </summary>
        /// <returns>
        ///     The event listener.
        /// </returns>
        /// <param name='aEventNameString'>
        ///     If set to <c>true</c> a event type_string.
        /// </param>
        /// <param name='aEventDelegate'>
        ///     If set to <c>true</c> a event delegate.
        /// </param>
        /// <param name='aEventDispatcherAddMode'>
        ///     If set to <c>true</c> event listening mode.
        /// </param>
        public bool AddEventListener(string aEventNameString,
                                     EventDelegate aEventDelegate,
                                     EventDispatcherAddMode aEventDispatcherAddMode)
        {
            //
            var wasSuccessfulBoolean = false;

            //
            var aIEventListener = _getArgumentsCallee(aEventDelegate);

            //
            if (aIEventListener != null && aEventNameString != null)
            {
                //	OUTER
                var keyForOuterHashTableString = _getKeyForOuterHashTable(aEventNameString);

                Action act = () =>
                {
                    if (!mEventListenerDatasHashtable.ContainsKey(keyForOuterHashTableString))
                    {
                        mEventListenerDatasHashtable.Add(keyForOuterHashTableString,
                            new Dictionary<string, EventListenerData>());
                    }

                    //	INNER
                    var innerHashtable = mEventListenerDatasHashtable[keyForOuterHashTableString];
                    var eventListenerData = new EventListenerData(aIEventListener, aEventNameString,
                        aEventDelegate, aEventDispatcherAddMode);
                    //
                    var keyForInnerHashTableString = _getKeyForInnerHashTable(eventListenerData);
                    if (innerHashtable != null && innerHashtable.ContainsKey(keyForInnerHashTableString))
                    {
                        //THIS SHOULD *NEVER* HAPPEN - REMOVE AFTER TESTED WELL
                        Logger.Info("TODO (FIX THIS): Event Manager: Listener: " + keyForInnerHashTableString +
                                    " is already in list for event: " + keyForOuterHashTableString);
                    }
                    else
                    {
                        //	ADD
                        if (innerHashtable != null)
                        {
                            innerHashtable.Add(keyForInnerHashTableString, eventListenerData);
                        }
                        wasSuccessfulBoolean = true;
                        //Logger.Info ("	ADDED AT: " + keyForInnerHashTable_string + " = " +  eventListenerData);
                    }
                };

                if (keyForOuterHashTableString == mCurrentEventName)
                {
                    mAppendAction.Enqueue(act);
                }
                else
                {
                    act();
                }
            }
            return wasSuccessfulBoolean;
        }

        /// <summary>
        ///     Has the event listener.
        /// </summary>
        /// <returns>
        ///     The event listener.
        /// </returns>
        /// <param name='aIEventListener'>
        ///     If set to <c>true</c> a I event listener.
        /// </param>
        /// <param name='aEventNameString'>
        ///     If set to <c>true</c> a event name_string.
        /// </param>
        /// <param name='aEventDelegate'>
        ///     If set to <c>true</c> a event delegate.
        /// </param>
        public bool HasEventListener(string aEventNameString, EventDelegate aEventDelegate)
        {
            //
            var hasEventListenerBoolean = false;

            //
            var aIEventListener = _getArgumentsCallee(aEventDelegate);

            //	OUTER
            var keyForOuterHashTableString = _getKeyForOuterHashTable(aEventNameString);
            if (mEventListenerDatasHashtable.ContainsKey(keyForOuterHashTableString))
            {
                //	INNER
                var innerHashtable = mEventListenerDatasHashtable[keyForOuterHashTableString];
                var keyForInnerHashTableString =
                    _getKeyForInnerHashTable(new EventListenerData(aIEventListener, aEventNameString, aEventDelegate,
                        EventDispatcherAddMode.DEFAULT));
                //
                EventListenerData data;
                if (innerHashtable != null && innerHashtable.TryGetValue(keyForInnerHashTableString, out data))
                {
                    hasEventListenerBoolean = data.Enable;
                }
            }

            return hasEventListenerBoolean;
        }

        /// <summary>
        ///     Removes the event listener.
        /// </summary>
        /// <returns>
        ///     The event listener.
        /// </returns>
        /// <param name='aIEventListener'>
        ///     If set to <c>true</c> a I event listener.
        /// </param>
        /// <param name='aEventNameString'>
        ///     If set to <c>true</c> a event name_string.
        /// </param>
        /// <param name='aEventDelegate'>
        ///     If set to <c>true</c> a event delegate.
        /// </param>
        public bool RemoveEventListener(string aEventNameString, EventDelegate aEventDelegate)
        {
            //
            var wasSuccessfulBoolean = false;
            //
            if (HasEventListener(aEventNameString, aEventDelegate))
            {
                //	OUTER
                var keyForOuterHashTableString = _getKeyForOuterHashTable(aEventNameString);

                Action act = () =>
                {
                    var innerHashtable = mEventListenerDatasHashtable[keyForOuterHashTableString];
                    //
                    var aIEventListener = _getArgumentsCallee(aEventDelegate);
                    //  INNER
                    var keyForInnerHashTableString =
                        _getKeyForInnerHashTable(new EventListenerData(aIEventListener, aEventNameString, aEventDelegate,
                            EventDispatcherAddMode.DEFAULT));
                    if (innerHashtable != null)
                    {
                        if (innerHashtable.Remove(keyForInnerHashTableString))
                        {
                            wasSuccessfulBoolean = true;
                        }
                    }
                };

                if (keyForOuterHashTableString == mCurrentEventName)
                {
                    mAppendAction.Enqueue(act);
                }
                else
                {
                    act();
                }
            }

            return wasSuccessfulBoolean;
        }

        /// <summary>
        ///     Removes all event listeners.
        /// </summary>
        /// <returns>
        ///     The all event listeners.
        /// </returns>
        public bool RemoveAllEventListeners()
        {
            //mEventListenerDatasHashtable = new Hashtable();					
            mEventListenerDatasHashtable.Clear();
            return true;
        }

        /// <summary>
        ///     Dispatchs the event.
        /// </summary>
        /// <returns>
        ///     The event.
        /// </returns>
        /// <param name='aIEvent'>
        ///     If set to <c>true</c> a I event.
        /// </param>
        public bool DispatchEvent(IEvent aIEvent)
        {
            //
            var wasSuccessfulBoolean = false;
            //
            _doAddTargetValueToIEvent(aIEvent);
            //	OUTER
            var keyForOuterHashTableString = _getKeyForOuterHashTable(aIEvent.Type);
            //int dispatchedCountInt = 0;
            Dictionary<string, EventListenerData> value;
            if (mEventListenerDatasHashtable.TryGetValue(keyForOuterHashTableString, out value))
            {
                //	INNER
                var innerHashtable = value;
                if (innerHashtable != null)
                {
                    mCurrentEventName = keyForOuterHashTableString;

                    {
                        // foreach(var eventListenerData in innerHashtable)
                        var __enumerator1 = (innerHashtable).GetEnumerator();
                        while (__enumerator1.MoveNext())
                        {
                            var eventListenerData = __enumerator1.Current;
                            {
                                if (eventListenerData.Value != null && eventListenerData.Value.Enable)
                                {
                                    try
                                    {
                                        eventListenerData.Value.EventDelegate(aIEvent);
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.Error("Event dispatch error. " + aIEvent.GetType().Name + " exception:" +
                                                     ex);
                                        continue;
                                    }

                                    //TODO - THIS IS PROBABLY FUNCTIONAL BUT NOT OPTIMIZED, MY APPROACH TO HOW/WHY SINGLE SHOTS ARE REMOVED
                                    //REMOVE IF ONESHOT
                                    if (eventListenerData.Value.EventListeningMode == EventDispatcherAddMode.SINGLE_SHOT)
                                    {
                                        mAppendAction.Enqueue(() => { innerHashtable.Remove(eventListenerData.Key); });
                                    }
                                }
                                //MARK SUCCESS, BUT ALSO CONTINUE LOOPING TOO
                                wasSuccessfulBoolean = true;
                            }
                        }
                    }

                    mCurrentEventName = string.Empty;
                }
            }

            while (mAppendAction.Count > 0)
            {
                var act = mAppendAction.Dequeue();
                try
                {
                    act();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString());
                }
            }

            return wasSuccessfulBoolean;
        }
    }
}