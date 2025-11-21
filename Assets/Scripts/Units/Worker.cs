using GameDevTV.EventBus;
using GameDevTV.Events;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour, ISelectable, IMovable
    {
        private NavMeshAgent agent;

        [SerializeField]
        private DecalProjector decal;

        public void Deselect()
        {
            if (decal != null)
                decal.gameObject.SetActive(false);
        }

        public void Select()
        {
            UnitSelectedEvent unitSelectedEvent = new UnitSelectedEvent(this);
            Bus<UnitSelectedEvent>.Raise(unitSelectedEvent);
            if (decal != null)
                decal.gameObject.SetActive(true);
        }

        public void MoveTo(Vector3 position)
        {
            agent.SetDestination(position);
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            // decal = GetComponentInChildren<DecalProjector>().gameObject;
        }
    }
}
