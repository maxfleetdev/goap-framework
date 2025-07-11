using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(ManagerInitialiser))]
public class RegistryManager : MonoBehaviour, IManageableAsync
{
    public async Task InitialiseAsync()
    {
        // Load StockData
        var tcs = new TaskCompletionSource<bool>();
        StockDataRegistry.LoadAll(() => tcs.SetResult(true));
        await tcs.Task;

        // Load Stock Prefabs
        tcs = new TaskCompletionSource<bool>();
        StockPrefabRegistry.LoadAll(() => tcs.SetResult(true));
        await tcs.Task;

        // Load Buildable Prefabs
        tcs = new TaskCompletionSource<bool>();
        BuildableAssetRegistry.LoadAll(() => tcs.SetResult(true));
        await tcs.Task;

        // Load AI Prefabs
        tcs = new TaskCompletionSource<bool>();
        AIPrefabRegistry.LoadAll(() => tcs.SetResult(true));
        await tcs.Task;
    }

    public void Terminate()
    {
        StockDataRegistry.ResetDatabase();
        StockPrefabRegistry.ResetDatabase();
        BuildableAssetRegistry.ResetDatabase();
        AIPrefabRegistry.ResetDatabase();
    }
}