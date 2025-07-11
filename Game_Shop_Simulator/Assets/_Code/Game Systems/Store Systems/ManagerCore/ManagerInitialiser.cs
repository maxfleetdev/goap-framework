using System.Threading.Tasks;
using UnityEngine;

public class ManagerInitialiser : MonoBehaviour
{
    private IManageable[] syncManagers = null;
    private IManageableAsync[] asyncManagers = null;

    private readonly string managerTag = "Manager";

    #region Awake/Destroy

    private async void Awake()
    {
        RegisterManagers();
        await InitialiseManagers();
    }

    private void OnDestroy()
    {
        TerminateManagers();
    }

    #endregion

    #region Register Managers

    private void RegisterManagers()
    {
        // Find each gameobject with Manager tag
        foreach (var obj in GameObject.FindGameObjectsWithTag(managerTag))
        {
            // Get all managers from found gameobject
            syncManagers = obj.GetComponents<IManageable>();
            asyncManagers = obj.GetComponents<IManageableAsync>();

            // Register all sync/async managers to registry
            foreach (var manager in syncManagers)
            {
                ManagerDatabase.Register(manager);
            }
            foreach (var manager in asyncManagers)
            {
                ManagerDatabase.Register(manager);
            }
        }
    }

    private async Task InitialiseManagers()
    {
        await ManagerDatabase.InitialiseAll();
    }

    #endregion

    #region Terminate Managers

    private void TerminateManagers()
    {
        ManagerDatabase.TerminateAll();
    }

    #endregion
}