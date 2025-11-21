using GameDevTV.EventBus;
using GameDevTV.Units;

namespace GameDevTV.Events
{
    public struct UnitSpawnEvent : IEvent
    {
        public AbstractUnit Unit { get; private set; }

        public UnitSpawnEvent(AbstractUnit unit)
        {
            Unit = unit;
        }
    }
}
