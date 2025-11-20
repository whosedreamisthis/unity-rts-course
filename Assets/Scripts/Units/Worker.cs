using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

namespace GameDevTV.Units
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Worker : MonoBehaviour, ISelectable
    {
        [SerializeField]
        private Transform target;
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
            if (decal != null)
                decal.gameObject.SetActive(true);
        }

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            // decal = GetComponentInChildren<DecalProjector>().gameObject;
        }

        // Update is called once per frame
        private void Update()
        {
            if (target != null)
            {
                agent.SetDestination(target.position);
            }
        }
    }
}
