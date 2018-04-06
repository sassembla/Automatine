using UnityEngine;

namespace Automatine
{
    public class OnChangerEvent
    {
        public enum EventType : int
        {
            EVENT_NONE,

            EVENT_ADD,
            EVENT_DELETE,


            EVENT_SELECTED,
            EVENT_ORDERUP,
            EVENT_ORDERDOWN,

            EVENT_UNDO,
            EVENT_SAVE,

            EVENT_CHANGE_AUTO,

            EVENT_REFRESHCONDITIONS,

            EVENT_ADDNEWTYPE,
            EVENT_ADDNEWVALUE,
            EVENT_ADDNEWTYPEVALUE,
        }

        public readonly EventType eventType;
        public readonly string activeObjectId;
        public readonly string message;

        public OnChangerEvent(OnChangerEvent.EventType eventType, string activeObjectId, string message = "")
        {
            this.eventType = eventType;
            this.activeObjectId = activeObjectId;
            this.message = message;
        }

    }
}