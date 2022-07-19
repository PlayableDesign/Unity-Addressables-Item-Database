
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class Item : MonoBehaviour
{
    
    // Inspector Settings
    
    [Header("Settings")]
    [Space(10)]
    
    public string ItemName;
    public string ItemDescription;
    public int ItemValue;
    
    // Public Set-Only Properties
    public string PrimaryKey
    {
        set => primaryKey = value;
    }

    public AsyncOperationHandle Handle
    {
        set => handle = value;
    }

    // Private State

    private string primaryKey;
    private AsyncOperationHandle handle;
    
    
    private void OnDestroy()
    {
        if(handle.IsValid()) Addressables.Release(handle);
    }
}
