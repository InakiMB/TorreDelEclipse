using FMODUnity;
   using UnityEngine;
   using UnityEngine.AI;

   [DisallowMultipleComponent]
   [RequireComponent(typeof(NavMeshAgent))]
   public sealed class NavMeshFootstepAudio : MonoBehaviour
   {
       [SerializeField] private EventReference footstepEvent = default;
       [SerializeField, Min(0.1f)] private float stepInterval = 0.42f;
       [SerializeField, Min(0.01f)] private float minimumSpeed = 0.2f;

       private NavMeshAgent agent;
       private float nextStepTime;

       private void Awake() => agent = GetComponent<NavMeshAgent>();

       private void Update()
       {
           if (!agent.enabled || !agent.isOnNavMesh || agent.isStopped
               || agent.velocity.sqrMagnitude < minimumSpeed * minimumSpeed)
           {
               return;
           }

           if (Time.time < nextStepTime) return;

           FmodOneShot.Play(footstepEvent, transform.position);
           nextStepTime = Time.time + stepInterval;
       }
   }