using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GSS.AI
{
    /// <summary>
    /// Abstract class which all agents derive from. Inherits monobehaviour for runtime methods.
    /// </summary>
    public abstract class AIAgent : MonoBehaviour
    {
        // Protected Variables
        protected AIController controller = null;
        protected AIProfileData profileData = null;
        protected Goal currentGoal = null;
        protected bool printDebug = true;

        protected List<AIHeldItem> inventory = new();
        protected AIPersonality aiPersonality;

        // Public Properties
        public string AgentName { get; private set; } = "AI";
        public List<AIHeldItem> Inventory
        {
            get => inventory;
            private set => inventory = value;
        }
        public AIPersonality AIPersonality
        {
            get => aiPersonality;
            private set => aiPersonality = value;
        }
        public AIProfileData ProfileData
        {
            get => profileData;
            private set => profileData = value;
        }

        // Public Actions
        public Action<AIAgent> OnAgentFinished { get; set; }

        #region Initialisation

        /// <summary>
        /// Assigns the goal to AIAgent
        /// </summary>
        public void Initialise(AIProfileData profile)
        {
            this.profileData = profile;
            this.aiPersonality = profileData.GetPersonality();
            this.inventory = new();
        }

        /// <summary>
        /// Executes all tasks within the AIGoal
        /// </summary>
        /// <returns>Task</returns>
        public async void StartGoal(Goal goal)
        {
            this.currentGoal = goal;
            goal.PlanTasks(this);
            await goal.ExecuteAsync(this);
        }

        #endregion

        #region Runtime Methods

        // AI Movement        
        /// <summary>
        /// Moves the Agent to a specific location. Returns true or false if Agent reaches destination
        /// </summary>
        /// <param name="target"></param>
        /// <returns>Boolean</returns>
        public virtual Task<bool> MoveToPositionAsync(Vector3 destination)
        {
            return controller.MoveToAsync(destination);
        }

        // AI Inventory
        /// <summary>
        /// Stores all items the agent has collected from shelves
        /// </summary>
        public virtual bool TryAddToInventory(int stockID)
        {
            if (StockDataRegistry.GetStockDataAsync(stockID) == null)
            {
                return false;
            }
            Inventory.Add(new AIHeldItem(stockID));
            return true;
        }

        // AI Lifecycle Complete
        public virtual void DespawnAgent()
        {
            Log($"Goal: {currentGoal.GetType().Name} Complete");
            OnAgentFinished?.Invoke(this);
        }

        #endregion

        #region Agent Targets

        // Shelf and Item Properties
        protected IShelf currentTargetShelf;
        /// <summary>
        /// Stores current shelf to interact with
        /// </summary>
        public IShelf CurrentTargetShelf
        {
            get => currentTargetShelf;
            set => currentTargetShelf = value;
        }

        protected Vector3 currentTargetPosition;
        /// <summary>
        /// Stores the target position to move
        /// </summary>
        public Vector3 CurrentTargetPosition
        {
            get => currentTargetPosition;
            set => currentTargetPosition = value;
        }

        protected int currentItemTarget;
        /// <summary>
        /// Stores the item we want to interact with
        /// </summary>
        public int CurrentItemTarget
        {
            get => currentItemTarget;
            set => currentItemTarget = value;
        }

        #endregion

        #region Debugging Tools

        public void Log(string msg) { if (printDebug) Debug.Log(msg); }
        public void Warn(string msg) { if (printDebug) Debug.LogWarning(msg); }

        #endregion
    }
}