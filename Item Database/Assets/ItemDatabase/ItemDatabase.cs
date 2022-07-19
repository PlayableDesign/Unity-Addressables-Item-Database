using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.ResourceLocations;
using Random = UnityEngine.Random;

public class ItemDatabase : MonoBehaviour
{
    // Inspector Settings

    [Header("Settings")]
    [Space(10)]

    [Tooltip("Add one or more Addressables lables here for items to add to the database.")]
    [SerializeField] private AssetLabelReference[] itemLabels;

    [Space(20)]
    [Header("Events")]
    [Space(10)]

    [Tooltip("Designate one or more game objects that will get notified when the Item Database is ready for use.")]
    public UnityEvent OnItemDatabaseLoaded;

    [Tooltip("Designate one or more game objects that will get notified on item instantiation.")]
    public UnityEvent<GameObject, string> OnItemSpawned;

    // Private Data

    // This dictionary will hold a list of asset primary keys organized by the label 
    private Dictionary<string, List<string>> itemKeys;

    // Private State
    private bool isLoading;

    // Unity LifeCycle

    private void Start()
    {
        ReloadDatabase();
    }

    // Public API

    public void ReloadDatabase()
    {
        if (!isLoading) StartCoroutine(LoadItemLocations());
    }

    public void SpawnItemByLabel(string label, Transform parent)
    {
        // Choose a random item for this label from our dictionary of locations
        var key = ChooseRandomItem(label);

        if (key == String.Empty)
        {
            Debug.LogWarning($"{name}: no item location keys available for label: {label}");
            return;
        }

        // kick off the async load and handle the completed event with inline callback
        // We don't need to track the handle returned, because the item will handle this
        // individually

        Addressables.InstantiateAsync(key, parent, false, true).Completed += result =>
        {
            // Once completed, notify listeners passing the resulting handle and primary key
            OnItemSpawned.Invoke(result.Result.gameObject, key);
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
        itemKeys = new Dictionary<string, List<string>>();

        // Process each label
        foreach (var label in itemLabels)
        {
            Debug.Log($"{name}: Loading asset locations for label: {label.labelString}");

            // initiate the async loading of resource locations for our labeled prefabs
            var handle = Addressables.LoadResourceLocationsAsync(label.labelString, typeof(GameObject));

            // yield execution until the async operation completes
            if (!handle.IsDone) yield return handle;

            itemKeys[label.labelString] = new List<string>();

            Debug.Log($"{name}: Found {handle.Result.Count} {label.labelString} item locations");

            foreach (IResourceLocation location in handle.Result)
            {
                itemKeys[label.labelString].Add(location.PrimaryKey);
            }

            Addressables.Release(handle);
        }

        isLoading = false;

        // Notify listeners that the database has loaded resource locations
        OnItemDatabaseLoaded.Invoke();
    }

    private string ChooseRandomItem(string label)
    {
        if (itemKeys[label].Count > 0)
        {
            // Choose a random index from the list of items for this label
            int randomIndex = Random.Range(0, itemKeys[label].Count);

            // Return its primary key to use for instantiating the prefab
            return itemKeys[label][randomIndex];
        }
        else
        {
            return String.Empty;
        }
    }


}
