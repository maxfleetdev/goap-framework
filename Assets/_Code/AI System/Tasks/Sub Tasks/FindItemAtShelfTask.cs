using System.Threading.Tasks;
using UnityEngine;

namespace GSS.AI
{
    public class FindItemAtShelfTask : TaskBase
    {
        private int requestedItemID = 1;
        private IShelf shelfWithItem = null;
        private bool narrowSearch = true;

        public FindItemAtShelfTask(int stockID, bool narrow_search = true)
        {
            // Find first shelf with given ID
            TaskName = "Find Item on Shelf";
            requestedItemID = stockID;
            narrowSearch = narrow_search;
        }

        public override async Task<bool> ExecuteAsync(AIAgent agent)
        {
            // Find shelf with item (closest or any)
            shelfWithItem = narrowSearch ?
                ShelfDatabase.FindShelfWithItem(requestedItemID, agent.transform.position) :
                ShelfDatabase.FindShelfWithItem(requestedItemID);

            if (shelfWithItem == null)
            {
                return false;
            }

            // Get shelf browsing spot, if any available
            Vector3 shelf_spot;
            if (!shelfWithItem.GetBrowsePoint(agent, out shelf_spot))
            {
                return false;
            }

            // Set agents target shelf and browsing
            agent.CurrentTargetPosition = shelf_spot;
            agent.CurrentTargetShelf = shelfWithItem;
            agent.CurrentItemTarget = requestedItemID;

            // Add slight delay to make async
            await Task.Yield();
            return true;
        }

        public override TaskBase Clone()
        {
            return new FindItemAtShelfTask(this.requestedItemID);
        }
    }
}