using GameDevTV.EventBus;
using GameDevTV.Events;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.Units
{
    public class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [SerializeField]
        private DecalProjector decalProjector;

        [field: SerializeField]
        public int Health { get; private set; }

        public void Deselect()
        {
            UnitDeselectedEvent unitDeselectedEvent = new UnitDeselectedEvent(this);
            Bus<UnitDeselectedEvent>.Raise(unitDeselectedEvent);

            if (decalProjector != null)
                decalProjector.gameObject.SetActive(false);
        }

        public void Select()
        {
            UnitSelectedEvent unitSelectedEvent = new UnitSelectedEvent(this);
            Bus<UnitSelectedEvent>.Raise(unitSelectedEvent);
            if (decalProjector != null)
                decalProjector.gameObject.SetActive(true);
        }
    }
}
