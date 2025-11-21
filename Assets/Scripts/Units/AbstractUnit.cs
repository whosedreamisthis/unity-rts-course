using GameDevTV.EventBus;
using GameDevTV.Events;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AbstractUnit : MonoBehaviour, ISelectable, IMovable
    {
        [SerializeField]
        private DecalProjector decal;
        public float AgentRadius => agent.radius;
        private NavMeshAgent agent;

        private void Start()
        {
            UnitSpawnEvent spawnEvent = new UnitSpawnEvent(this);
            Bus<UnitSpawnEvent>.Raise(spawnEvent);
        }

        public void Deselect()
        {
            UnitDeselectedEvent unitDeselectedEvent = new UnitDeselectedEvent(this);
            Bus<UnitDeselectedEvent>.Raise(unitDeselectedEvent);

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
