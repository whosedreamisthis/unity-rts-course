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
        name: "StopAgent",
        story: "[Agent] stops moving.",
        category: "Action/Navigation",
        id: "029906da80af92d0a2a1e4616b9058fe"
    )]
    public partial class StopAgentAction : Action
    {
        [SerializeReference]
        public BlackboardVariable<GameObject> Agent;

        protected override Status OnStart()
        {
            if (Agent.Value.TryGetComponent(out NavMeshAgent agent))
            {
                agent.ResetPath();
                return Status.Success;
            }
            return Status.Failure;
        }
    }
}
