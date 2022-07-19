

using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    // Inspector References

    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform floor;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private AnimationCurve randomCurve;
    
    
    // Private Cache
    
    private Mesh floorMesh;
    private float floorX;
    private float floorZ;

    private bool isMoving = true;
    private float y;
    
    // LifeCycle
    
    private void Start()
    {
      GetSizeOfFloor();
      y = transform.position.y; // save height
    }
    
    private void Update()
    {
        if (!isMoving)
        {
            isMoving = true;
            var x = Random.Range(-floorX, floorX);
            var z = Random.Range(-floorZ, floorZ);
            StartCoroutine(Move(new Vector3(x, y, z)));
        }
    }
    
    // Event Handlers

    public void OnItemDatabaseLoaded()
    {
        Debug.Log($"{name}: ItemDatabase is ready, starting spawns.");
        isMoving = false;
    }

    public void OnItemSpawned(AsyncOperationHandle<GameObject> handle, string primaryKey)
    {
        GameObject itemGO = handle.Result;
        Item item = itemGO.GetComponent<Item>();
        
        // Set the key for use in saving this item later in inventory
        // The key can be used to instantiate again from save
        item.PrimaryKey = primaryKey;
            
        // store the handle so that the Item can clean up on destroy
        item.Handle = handle;

        itemGO.transform.position = transform.position;

    }
    
    
    // Private Methods
    
    private void GetSizeOfFloor()
    {
        floorMesh = floor.GetComponent<MeshFilter>().mesh;
        var scale = floor.localScale;
        floorX = (floorMesh.bounds.extents.x * scale.x) - 1f;
        floorZ = (floorMesh.bounds.extents.z * scale.z) - 1f;
    }
    
    // Coroutines
    
    private IEnumerator Move(Vector3 destination)
    {
        float progress = 0f;
        float duration = 1f;
        
        Vector3 start = transform.position;
        
        while (progress < duration)
        {
            transform.position = Vector3.Lerp(start, destination, progress / duration);
            progress += Time.deltaTime;
            yield return null;
        }
        
        transform.position = destination;
        
        // Spawn an item here
        var rY = Random.Range(0f, 1f);

        if (rY <= 0.48f)
        {
            itemDatabase.SpawnItemByLabel("items_basic", spawnPoint);
        }
        else if(rY is > 0.48f and <= 0.96f )
        {
            itemDatabase.SpawnItemByLabel("items_remote", spawnPoint);
        }
        else 
        {
            itemDatabase.SpawnItemByLabel("items_rare", spawnPoint);
        }
        
        yield return new WaitForSeconds(0.4f);
        isMoving = false;
    }
    

    
    
}
