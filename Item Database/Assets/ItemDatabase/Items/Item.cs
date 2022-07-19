
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Item : MonoBehaviour
{

    // Inspector Settings

    [Header("Settings")]
    [Space(10)]

    [SerializeField] private float expiration;

    public string PrimaryKey { set => primaryKey = value; }

    private string primaryKey;


    private IEnumerator Start()
    {
        yield return null;

        if (expiration > 0f)
        {
            yield return new WaitForSeconds(expiration);
            Destroy(gameObject);
        }

        yield return null;

    }

    public override string ToString()
    {
        return primaryKey;
    }

    private void OnDestroy()
    {
        Addressables.ReleaseInstance(gameObject);
    }
}
