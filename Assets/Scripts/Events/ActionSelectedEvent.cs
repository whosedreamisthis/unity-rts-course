using System;
using GameDevTV.Commands;
using GameDevTV.EventBus;
using GameDevTV.Units;

namespace GameDevTV.Events
{
    public struct ActionSelectedEvent : IEvent
    {
        public ActionBase Action { get; }

        public ActionSelectedEvent(ActionBase action)
        {
            Action = action;
        }
    }
}
