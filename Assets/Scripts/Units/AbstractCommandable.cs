using GameDevTV.EventBus;
using GameDevTV.Events;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.Units
{
    public class AbstractCommandable : MonoBehaviour, ISelectable
    {
        [field: SerializeField]
        public int CurrentHealth { get; private set; }

        [field: SerializeField]
        public int MaxHealth { get; private set; }

        [SerializeField]
        private DecalProjector decalProjector;

        [SerializeField]
        private UnitSO UnitSO;

        protected virtual void Start()
        {
            MaxHealth = UnitSO.Health;
            CurrentHealth = UnitSO.Health;
        }

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
