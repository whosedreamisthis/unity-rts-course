using GameDevTV.EventBus;
using GameDevTV.Events;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMovable
    {
        public float AgentRadius => agent.radius;
        private NavMeshAgent agent;

        public void MoveTo(Vector3 position)
        {
            agent.SetDestination(position);
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            // decal = GetComponentInChildren<DecalProjector>().gameObject;
        }

        protected override void Start()
        {
            base.Start();
            UnitSpawnEvent spawnEvent = new UnitSpawnEvent(this);
            Bus<UnitSpawnEvent>.Raise(spawnEvent);
        }
    }
}
