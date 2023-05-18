using System.Collections;
using UnityEngine;

//
// Written by Adam Calvelage -> adamjasoncalvelage@gmail.com
//

public class SpawnParts : MonoBehaviour
{
    public GameObject prefab;
    private int objectID;

    private void Start() => StartCoroutine(Spawn());
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.GetInstanceID() == objectID)
        {
            StartCoroutine(Spawn());
            objectID = -1;
        }
    }

    public IEnumerator Spawn()
    {
        yield return new WaitForSecondsRealtime(1.5f);

        var clone = Instantiate(prefab, transform.position, transform.rotation);
        objectID = clone.GetInstanceID();
    }
}
