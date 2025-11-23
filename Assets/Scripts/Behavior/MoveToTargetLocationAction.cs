using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using UnityEngine.AI;
using Action = Unity.Behavior.Action;

namespace GameDevTV.Behavior
{
    [Serializable, GeneratePropertyBag]
    [NodeDescription(
        name: "Move to Target Location",
        story: "[Agent] moves to [TargetLocation]",
        category: "Action/Navigation",
        id: "2104d525ea0113c6d228f51129c6fbf2"
    )]
    public partial class MoveToTargetLocationAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Agent;

        [SerializeReference]
        public BlackboardVariable<Vector3> TargetLocation;

        private NavMeshAgent agent;

        protected override Status OnStart()
        {
            if (!Agent.Value.TryGetComponent<NavMeshAgent>(out agent))
                return Status.Failure;

            if (
                Vector3.Distance(agent.transform.position, TargetLocation.Value)
                <= agent.stoppingDistance
            )
                return Status.Success;

            agent.SetDestination(TargetLocation.Value);
            return Status.Running;
        }

        protected override Status OnUpdate()
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                return Status.Success;
            return Status.Running;
        }
    }
}
