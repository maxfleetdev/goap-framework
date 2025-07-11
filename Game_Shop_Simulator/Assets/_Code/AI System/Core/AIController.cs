using UnityEngine;
using Pathfinding;
using System.Threading.Tasks;

namespace GSS.AI
{
    [RequireComponent(typeof(AIAgent))]
    public class AIController : MonoBehaviour
    {
        private FollowerEntity movementEntity;
        private TaskCompletionSource<bool> moveTaskSource;
        private Vector3 targetPosition;
        private bool isMoving = false;

        private void Awake()
        {
            movementEntity = GetComponent<FollowerEntity>();
        }

        private void Update()
        {
            if (isMoving)
            {
                if (movementEntity.reachedEndOfPath)
                {
                    moveTaskSource.TrySetResult(movementEntity.reachedEndOfPath);
                    isMoving = false;
                }
            }
        }

        public async Task<bool> MoveToAsync(Vector3 target)
        {
            if (moveTaskSource != null)
            {
                moveTaskSource.TrySetResult(false);
            }

            if (movementEntity == null)
            {
                movementEntity = GetComponent<FollowerEntity>();
                if (movementEntity == null)
                {
                    Debug.LogWarning("[AIController] No FollowerEntity found.");
                    return false;
                }
            }

            // Wait until the entity exists
            int timeout = 50;
            while (!movementEntity.entityExists && timeout-- > 0)
            {
                await Task.Yield();

                if (this == null || !this.gameObject.activeInHierarchy)
                {
                    Debug.LogWarning("[AIController] Aborting MoveToAsync — AIController was destroyed or disabled.");
                    return false;
                }
            }

            if (!movementEntity.entityExists)
            {
                Debug.LogWarning("[AIController] FollowerEntity failed to initialize (entity does not exist).");
                return false;
            }

            moveTaskSource = new TaskCompletionSource<bool>();
            targetPosition = target;
            movementEntity.destination = target;
            isMoving = true;

            return await moveTaskSource.Task;
        }
    }
}