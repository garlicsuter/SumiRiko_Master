using System.Collections;
using UnityEngine;

/// <summary>
/// SHOWCASE CODE
/// </summary>
public class SpawnParts : MonoBehaviour
{
    public GameObject partPrefab;
    public Vector3 position;

    private void Start()
    {
        StartCoroutine("Routine");
    }

    private IEnumerator Routine()
    {
        Instantiate(partPrefab, position, transform.rotation);

        yield return new WaitForSecondsRealtime(5.0f);

        StartCoroutine("Routine");
    }
}
