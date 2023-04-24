using System;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [NonSerialized]
    public Transform[] nodes;

    public float socketSpacing;

    private void Start()
    {

    }
    private void Update()
    {

    }
    private void OnValidate()
    {
        nodes = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            nodes[i] = transform.GetChild(i);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        for (int i = 0; i < nodes.Length; i++)
        {
            Gizmos.DrawWireSphere(nodes[i].position, 0.1f);

            if (i < nodes.Length - 1 && nodes[i + 1] != null)
            {
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
            }
        }
    }
}