using System.Collections;
using UnityEngine;

/// <summary>
/// SHOWCASE CODE
/// </summary>
public class SpawnParts : MonoBehaviour
{
    public GameObject partPrefab;
    public Vector3 position;

    public void Spawn()
    {
        Instantiate(partPrefab, position, transform.rotation);
    }
}
