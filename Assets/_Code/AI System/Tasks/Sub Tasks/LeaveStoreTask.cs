using System.Threading.Tasks;
using UnityEngine;

namespace GSS.AI
{
    public class LeaveStoreTask : TaskBase
    {
        public LeaveStoreTask()
        {
            TaskName = "Leave Store";
        }

        public override TaskBase Clone()
        {
            return new LeaveStoreTask();
        }

        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            return await agent.MoveToPositionAsync(Vector3.zero);
        }
    }
}