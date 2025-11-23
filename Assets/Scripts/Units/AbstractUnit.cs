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

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            behaviourAgent = GetComponent<BehaviorGraphAgent>();
            behaviourAgent.SetVariableValue("Command", UnitCommands.Stop);
        }

        protected override void Start()
        {
            base.Start();
            UnitSpawnEvent spawnEvent = new UnitSpawnEvent(this);
            Bus<UnitSpawnEvent>.Raise(spawnEvent);
        }

        public void MoveTo(Vector3 position)
        {
            Debug.Log("abstaract unit moveto");
            behaviourAgent.SetVariableValue("TargetLocation", position);
            behaviourAgent.SetVariableValue("Command", UnitCommands.Move);
        }

        public void Stop()
        {
            behaviourAgent.SetVariableValue("Command", UnitCommands.Stop);
        }
    }
}
