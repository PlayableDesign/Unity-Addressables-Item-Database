
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemSpawner : MonoBehaviour
{
    [Space(10)]
    [Header("References")]
    [Space(10)]

    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private Transform itemsContainer;

    [Space(10)]

    [SerializeField] private Transform floor;
    [SerializeField] private float floorBuffer;

    [Space(10)]
    [Header("Assets")]
    [Space(10)]

    [SerializeField] private ItemProbabilityAsset probability;


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
        transform.Rotate(0f, 0.1f, 0f, Space.World);

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

    public void OnItemSpawned(GameObject go, string primaryKey)
    {
        // Set the key for use in saving this item later in inventory
        // The key can be used to instantiate again from save

        Debug.Log($"{name}: spawning item with key: {primaryKey}");

        go.GetComponent<Item>().PrimaryKey = primaryKey;
        go.transform.position = new Vector3(transform.position.x, y - 1f, transform.position.z);

    }

    // Private Methods

    private void GetSizeOfFloor()
    {
        floorMesh = floor.GetComponent<MeshFilter>().mesh;
        var scale = floor.localScale;
        floorX = (floorMesh.bounds.extents.x * scale.x) - floorBuffer;
        floorZ = (floorMesh.bounds.extents.z * scale.z) - floorBuffer;
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

        var label = probability.GetRandomLabel();

        itemDatabase.SpawnItemByLabel(label, itemsContainer);

        yield return new WaitForSeconds(0.4f);
        isMoving = false;
    }

}
