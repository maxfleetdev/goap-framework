using UnityEngine;

namespace GSS.Building
{
    /// <summary>
    /// Main logic for raycasting and checking whether an object can be placed.
    /// </summary>
    public class PlacementController
    {
        private readonly float refreshRate;
        private float nextRefreshTime;
        private int snapAmount = 10;

        private readonly RaycastManager raycastManager;
        private readonly GhostManager ghostManager;

        private readonly string[] overlapTags = new[] { "Ghost", "Player" };

        public Vector3 LastRaycastPosition { get; private set; }
        public bool IsPlacementValid { get; private set; }

        public PlacementController(float refresh_rate, RaycastManager raycast_manager, GhostManager ghost_manager)
        {
            this.refreshRate = refresh_rate;
            this.raycastManager = raycast_manager;
            this.ghostManager = ghost_manager;
        }

        /// <summary>
        /// Returns true if a certain amount of time has passed
        /// </summary>
        /// <param name="current_time"></param>
        /// <returns></returns>
        public bool ShouldUpdate(float current_time)
        {
            if (current_time < nextRefreshTime)
                return false;

            nextRefreshTime = current_time + refreshRate;
            return true;
        }

        /// <summary>
        /// Updates a Ghost object's position using RaycastManager and checks if its obstructed and has correct orientation
        /// </summary>
        /// <param name="object_data"></param>
        /// <param name="rule"></param>
        /// <param name="valid"></param>
        /// <param name="invalid"></param>
        public void UpdatePlacement(PlaceableObject object_data, Color valid, Color invalid)
        {
            if (ghostManager.GhostRenderer == null)
            {
                return;
            }

            RaycastResult result = raycastManager.Raycast(object_data.IsStackable);
            if (result.Hit)
            {
                // Overlap Check
                bool is_overlapping = false;
                if (!object_data.IsOverlappable)
                {
                    is_overlapping = BoxOverlapCheck.IsObjectOverlapping(ghostManager.GhostRenderer.localBounds, ghostManager.GhostRenderer.transform, overlapTags);
                    
                    // Only check CustomBoundCast if mesh bound is unobstructed
                    if (!is_overlapping && object_data.HasExtendedBounds)
                    {
                        is_overlapping = BoxOverlapCheck.IsCustomBoundOverlapping(ghostManager.CustomVolume, overlapTags);
                    }
                }
                
                // Orientation Check
                bool validAngle = object_data.BuildRule.IsPlacementValid(result.Normal);

                IsPlacementValid = !is_overlapping && validAngle;
                LastRaycastPosition = result.Point;

                ghostManager.GhostTransform.position = PositionSnap.Snap(result.Point, snapAmount);
            }
            else
            {
                IsPlacementValid = false;
            }

            ghostManager.SetGhostColour(IsPlacementValid ? valid : invalid);
        }

        /// <summary>
        /// Sets the current snap amount used in UpdatePlacement
        /// </summary>
        /// <param name="amount"></param>
        public void SetSnapAmount(int amount)
        {
            snapAmount = amount;
        }
    }
}