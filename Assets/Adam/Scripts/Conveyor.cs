using System;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    private Node[] nodes;
    private float length;

    public float spacing;

    private void OnValidate()
    {
        // Reset value
        length = 0.0f;

        // Init nodes
        nodes = new Node[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            // Fetch current & next child
            Transform child = transform.GetChild(i);
            Transform next = ((i + 1) < transform.childCount) ? transform.GetChild(i + 1) : null;

            // Calculate distance between children
            float dist = next != null ? Vector3.Distance(child.position, next.position) : 0.0f;

            // Set values
            nodes[i].position = child.position;
            nodes[i].length = dist;
            length += dist;
        }
    }
    private void OnDrawGizmos()
    {
        for(int i = 0; i < nodes.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(nodes[i].position, 0.1f);

            // Does this node continue?
            if(i + 1 < nodes.Length)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);

                Vector3 dir = (nodes[i + 1].position - nodes[i].position).normalized;
                float total = (nodes[i].length / spacing);
                for(int f = 1; f < (int)(total); f++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawWireSphere(nodes[i].position + (dir * (nodes.Length / f)), 0.025f);
                }
            }
        }
    }
}

[Serializable]
internal struct Node
{
    internal Vector3 position;
    internal float length;
    internal bool end => length == 0;
}