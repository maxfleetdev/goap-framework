using UnityEngine;

namespace GSS.Building
{
    /// <summary>
    /// Manager for controlling and pooling GameObjects which represent the "Ghost" placable object.
    /// </summary>
    public class GhostManager
    {
        private GameObject ghostObject;
        private MeshRenderer ghostRenderer;
        private Transform prefabTransform;
        private CustomBoundVolume customVolume;

        public GameObject GhostObject => ghostObject;
        public Transform GhostTransform => ghostObject.transform;
        public MeshRenderer GhostRenderer => ghostRenderer;
        public Transform PrefabTransform => prefabTransform;
        public CustomBoundVolume CustomVolume => customVolume;

        /// <summary>
        /// Creates a new Ghost Gameobject
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="ghost_material"></param>
        public void CreateGhost(GameObject prefab, Material ghost_material)
        {
            DestroyGhost();

            ghostObject = GhostObjectBuilder.BuildGhost(prefab, ghost_material);
            ghostRenderer = ghostObject.GetComponentInChildren<MeshRenderer>();
            customVolume = ghostObject.GetComponentInChildren<CustomBoundVolume>();
            prefabTransform = ghostRenderer.transform;
            
        }

        /// <summary>
        /// Destroys current Ghost Gameobject
        /// </summary>
        public void DestroyGhost()
        {
            if (ghostObject != null)
            {
                GameObject.Destroy(ghostObject);
            }
        }

        /// <summary>
        /// Sets the Ghost's colour on it's material
        /// </summary>
        /// <param name="color"></param>
        public void SetGhostColour(Color color)
        {
            if (ghostRenderer != null)
            {
                ghostRenderer.material.color = color;
            }
        }
    }
}