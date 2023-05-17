using System.Collections;
using UnityEngine;

public class SpawnParts : MonoBehaviour
{
    public GameObject prefab;
    public Vector3 position;

    private GameObject clone;

    private void Start() => StartCoroutine(Spawn());
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == clone)
        {
            StartCoroutine(Spawn());
        }
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSecondsRealtime(1.0f);

        clone = Instantiate(prefab, position, transform.rotation);
    }
}
