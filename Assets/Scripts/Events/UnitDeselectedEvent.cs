using GameDevTV.EventBus;
using GameDevTV.Units;

namespace GameDevTV.Events
{
    public struct UnitDeselectedEvent : IEvent
    {
        public ISelectable Unit { get; private set; }

        public UnitDeselectedEvent(ISelectable unit)
        {
            Unit = unit;
        }
    }
}
