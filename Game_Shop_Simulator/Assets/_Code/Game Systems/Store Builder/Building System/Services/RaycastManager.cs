using UnityEngine;

namespace GSS.Building
{
    /// <summary>
    /// Raycast manager which controls a raycast from the Player. Contains functions for firing a ray and retrieving the ray's information.
    /// </summary>
    public class RaycastManager
    {
        private static readonly int MAX_RAYCASTS = 2;

        private readonly Transform rayOrigin;
        private readonly float rayLength;
        private readonly LayerMask ignoreLayer;
        private readonly string[] blockedTags;
        private readonly bool showDebug;

        public RaycastManager(Transform rayOrigin, float rayLength, LayerMask ignoreLayer, string[] blockedTags, bool showDebug)
        {
            this.rayOrigin = rayOrigin;
            this.rayLength = rayLength;
            this.ignoreLayer = ignoreLayer;
            this.blockedTags = blockedTags;
            this.showDebug = showDebug;
        }

        /// <summary>
        /// Fires a raycast from the given origin in the forward direction and compares against the given conditionals. 
        /// </summary>
        /// <param name="allow_stacking"></param>
        /// <returns>RaycastResult</returns>
        public RaycastResult Raycast(bool allow_stacking)
        {
            Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
            RaycastHit[] hits = new RaycastHit[MAX_RAYCASTS];
            int hit_count = Physics.RaycastNonAlloc(ray, hits, rayLength, ~ignoreLayer);

            for (int i = 0; i < hit_count; i++)
            {
                Transform hit_transform = hits[i].transform;

                // Skip if stacking is not allowed and the tag is blocked
                if (!allow_stacking && IsBlockedTag(hit_transform.tag))
                {
                    continue;
                }

                // Store hit data
                Vector3 hit_point = hits[i].point;
                Vector3 hit_normal = hits[i].normal;

                if (showDebug)
                {
                    Debug.DrawLine(rayOrigin.position, hit_point, Color.green);
                }

                return new RaycastResult(true, hit_point, hit_normal, hit_transform.tag);
            }

            return new RaycastResult(false, Vector3.zero, Vector3.zero, null);
        }

        /// <summary>
        /// Fires a raycast and returns if a Transform is found with a given tag
        /// </summary>
        /// <param name="find_tag"></param>
        /// <returns></returns>
        public RaycastResult FindRaycast(string find_tag)
        {
            Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
            RaycastHit[] hits = new RaycastHit[MAX_RAYCASTS];
            int hit_count = Physics.RaycastNonAlloc(ray, hits, rayLength, ~ignoreLayer);

            for (int i = 0; i < hit_count; i++)
            {
                Transform hit_transform = hits[i].transform;

                // Store hit data
                Vector3 hit_point = hits[i].point;

                if (showDebug)
                {
                    Debug.DrawLine(rayOrigin.position, hit_point, Color.green, 1f);
                }

                // Return found object
                if (hit_transform.tag == find_tag)
                {
                    int id = hit_transform.GetComponent<IProvidable<PlaceableObject>>().Get().ObjectID;
                    return new RaycastResult(id, hit_transform);
                }
            }

            return new RaycastResult();
        }

        // Checks if given tag is within the BlockedTags string array
        private bool IsBlockedTag(string tag)
        {
            foreach (string blocked in blockedTags)
            {
                if (tag == blocked)
                {
                    return true;
                }
            }
            return false;
        }
    }
}