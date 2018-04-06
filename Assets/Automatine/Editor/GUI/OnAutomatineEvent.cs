using UnityEngine;

namespace Automatine
{
    public class OnAutomatineEvent
    {
        public enum EventType : int
        {
            EVENT_NONE,

            EVENT_OBJECT_SELECTED,
            EVENT_UNSELECTED,

            // auto
            EVENT_AUTO_ADDTIMELINE,
            EVENT_AUTO_ADDAUTO,
            EVENT_AUTO_ADDCHANGER,
            EVENT_AUTO_DELETED,

            // timeline
            EVENT_TIMELINE_ADDTACK,
            EVENT_TIMELINE_DELETE,
            EVENT_TIMELINE_COPY,
            EVENT_TIMELINE_CUT,
            EVENT_TIMELINE_PASTE,

            // tack
            EVENT_TACK_MOVING,
            EVENT_TACK_MOVED,
            EVENT_TACK_MOVED_AFTER,
            EVENT_TACK_DELETEAFTERUNLIMITED,
            EVENT_TACK_REMOVECOROUTINE,
            EVENT_TACK_SETNEWTYPE,
            EVENT_TACK_SETNEWVALUE,
            EVENT_TACK_DELETED,
            EVENT_TACK_COPY,
            EVENT_TACK_CUT,
            EVENT_TACK_PASTE,
            EVENT_TACK_SHOWCOROUTINES,

            // coroutine
            EVENT_SHOW_COROUTINEWINDOW,
            EVENT_SHOW_COROUTINEONFRAMEWINDOW,


            EVENT_REFRESHTIMELINECONDITIONS,
            EVENT_REFRESHTACKCONDITIONS,
            EVENT_ADDNEWTYPE,
            EVENT_ADDNEWVALUE,

            EVENT_ADDNEWCOROUTINE,


            EVENT_UNDO,
            EVENT_SAVE,
        }

        public readonly EventType eventType;
        public readonly string activeObjectId;
        public readonly int frame;

        public readonly string message;

        public OnAutomatineEvent(OnAutomatineEvent.EventType eventType, string activeObjectId, int frame = -1, string message = "")
        {
            this.eventType = eventType;
            this.activeObjectId = activeObjectId;
            this.frame = frame;
            this.message = message;
        }

        public OnAutomatineEvent Copy()
        {
            return new OnAutomatineEvent(this.eventType, this.activeObjectId, this.frame, this.message);
        }
    }
}