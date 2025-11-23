using GameDevTV.EventBus;
using GameDevTV.Events;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.Units
{
    [RequireComponent(typeof(NavMeshAgent), typeof(BehaviorGraphAgent))]
    public abstract class AbstractUnit : AbstractCommandable, IMovable
    {
        public float AgentRadius => agent.radius;
        private NavMeshAgent agent;
        private BehaviorGraphAgent behaviourAgent;

        public void MoveTo(Vector3 position)
        {
            Debug.Log($"Move to {position}");
            string targetLocationName = "TargetLocation";
            behaviourAgent.SetVariableValue(targetLocationName, position);
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            behaviourAgent = GetComponent<BehaviorGraphAgent>();
            MoveTo(transform.position);

            // decal = GetComponentInChildren<DecalProjector>().gameObject;
        }

        protected override void Start()
        {
            base.Start();
            UnitSpawnEvent spawnEvent = new UnitSpawnEvent(this);
            Bus<UnitSpawnEvent>.Raise(spawnEvent);
            MoveTo(transform.position);
        }
    }
}
