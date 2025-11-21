using GameDevTV.EventBus;
using GameDevTV.Units;

namespace GameDevTV.Events
{
    public struct UnitSelectedEvent : IEvent
    {
        public ISelectable Unit { get; private set; }

        public UnitSelectedEvent(ISelectable unit)
        {
            Unit = unit;
        }
    }
}
