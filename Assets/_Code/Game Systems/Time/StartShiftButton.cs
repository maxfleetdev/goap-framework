using UnityEngine;

public class StartShiftButton : MonoBehaviour, IInteractable
{
    [SerializeField] private Material openMaterial;         // colour when shop is open
    [SerializeField] private Material closeMaterial;        // colour when shop is closed

    private TimeManager timeManager;

    private void OnEnable()
    {
        TimeEvents.OnShiftStart += StoreOpened;
        TimeEvents.OnShiftEnd += StoreClosed;
    }

    private void OnDisable()
    {
        TimeEvents.OnShiftStart -= StoreOpened;
        TimeEvents.OnShiftEnd -= StoreClosed;
    }

    public void OnInteract()
    {
        if (timeManager == null)
        {
            timeManager = TimeEvents.GetManager();
        }

        if (timeManager.IsRunning)
        {
            timeManager.EndShift();
            return;
        }

        timeManager.StartShift();
    }

    private void StoreClosed()
    {
        GetComponent<MeshRenderer>().material = closeMaterial;
    }

    private void StoreOpened()
    {
        GetComponent<MeshRenderer>().material = openMaterial;
    }
}