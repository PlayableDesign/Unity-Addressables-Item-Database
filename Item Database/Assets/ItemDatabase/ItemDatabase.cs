using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Random = UnityEngine.Random;

public class ItemDatabase : MonoBehaviour
{
    // Inspector Settings
    
    [Header("Settings")]
    [Space(10)]
    
    [Tooltip("Add one or more Addressables lables here for items to add to the database.")]
    [SerializeField] private List<string> itemLabels;

    [Space(20)]
    [Header("Events")]
    [Space(10)]
    
    [Tooltip("Designate one or more game objects that will get notified when the Item Database is ready for use.")]
    public UnityEvent OnItemDatabaseLoaded;

    [Tooltip("Designate one or more game objects that will get notified on item instantiation.")]
    public UnityEvent<AsyncOperationHandle<GameObject>, string> OnItemSpawned;
    
    // Private Data
    
    // This dictionary will hold a list of resource locations organized by the label (key)
    private Dictionary<string, IList<IResourceLocation>> itemLocations;
    
    // Private State
    private bool isLoading;
    private AsyncOperationHandle<IList<IResourceLocation>> itemLocationsHandle;

    // Unity ifeCycle
    
    private void Start()
    {
        ReloadDatabase();
    }

    private void OnDestroy()
    {
        if(itemLocationsHandle.IsValid()) Addressables.Release(itemLocationsHandle);
    }
    
    // Public API
    
    public void ReloadDatabase()
    {
        if(!isLoading) StartCoroutine(LoadItemLocations());
    }
    
    public void SpawnItemByLabel(string label, Transform parent)
    {
        // Choose a random item for this label from our dictionary of locations
        var key = ChooseRandomItemLocation(label);


        if (key == String.Empty)
        {
            Debug.LogWarning($"{name}: no item location keys available for label: {label}");
            return;
        }
        
        // kick off the async load
        var itemHandle = Addressables.InstantiateAsync(key, parent, false, true);
        
        itemHandle.Completed += result =>
        {
            // Once completed, notify listeners passing the resulting handle and primary key
            OnItemSpawned.Invoke(result, key);
        };
        
    }

    public void SpawnItemsByLabel(string label, Transform parent, int count)
    {
        for (int i = 0; i < count; i++)
        {
            SpawnItemByLabel(label, parent);
        }
    }
    
    // Private Methods
    
    private IEnumerator LoadItemLocations()
    {
        isLoading = true;
        
        // Clear the dictionary
        itemLocations = new Dictionary<string, IList<IResourceLocation>>();
        
        // Process each label
        foreach (string label in itemLabels)
        {
            Debug.Log($"{name}: Loading asset locations for label: {label}");
            
            // initiate the async loading of resource locations for our labeled prefabs
            itemLocationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(GameObject));
            
            // yield execution until the async operation completes
            if (!itemLocationsHandle.IsDone) yield return itemLocationsHandle;
            
            Debug.Log($"{name}: Found {itemLocationsHandle.Result.Count} {label} item locations");
            
            if (itemLocationsHandle.Result.Count > 0)
            {
                // Add the resulting list of locations to the dictionary with this label as the key
                itemLocations.Add(label, itemLocationsHandle.Result);
            }
            else
            {
                // no results, store an empty list
                itemLocations[label] = new List<IResourceLocation>();
            }
        }

        isLoading = false;
        
        // Notify listeners that the database has loaded resource locations
        OnItemDatabaseLoaded.Invoke();
    }

    private string ChooseRandomItemLocation(string label)
    {
        var count = itemLocations[label].Count;

        if (count > 0)
        {
            // Choose a random index from the list of items for this label
            int randomIndex = Random.Range(0, itemLocations[label].Count);

            // Return its primary key to use for instantiating the prefab
            return itemLocations[label][randomIndex].PrimaryKey;
        }
        else
        {
            return String.Empty;
        }
    }



}
