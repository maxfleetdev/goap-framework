using System.Threading.Tasks;
using UnityEngine;

namespace GSS.AI
{
    public class MoveToPositionTask : TaskBase
    {
        private Vector3 targetPositon = Vector3.zero;

        public MoveToPositionTask()
        {
            TaskName = "Move To Position";
        }

        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            // Our target position from Agent Class
            targetPositon = agent.CurrentTargetPosition;

            // Move Agent towards target position
            return await agent.MoveToPositionAsync(targetPositon);
        }

        public override TaskBase Clone()
        {
            return new MoveToPositionTask();
        }
    }
}