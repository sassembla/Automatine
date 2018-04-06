using UnityEngine;

namespace Automatine
{
    public class OnConditionsEvent
    {
        public enum EventType : int
        {
            EVENT_NONE,
            EVENT_ADDTYPE,
            EVENT_ADDVALUE,
            EVENT_DELETETYPE,
            EVENT_DELETEVALUE,
        }

        public readonly EventType eventType;

        public readonly ConditionsGUIAuxWindow window;

        public readonly string typeName;
        public readonly string valueName;

        public OnConditionsEvent(OnConditionsEvent.EventType eventType, ConditionsGUIAuxWindow window, string typeName, string valueName)
        {
            this.eventType = eventType;
            this.window = window;
            this.typeName = typeName;
            this.valueName = valueName;
        }

        public OnConditionsEvent Copy()
        {
            return new OnConditionsEvent(this.eventType, this.window, this.typeName, this.valueName);
        }
    }
}