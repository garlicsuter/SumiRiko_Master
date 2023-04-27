using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    private Vector3[] points;
    private Node[] nodes;
    private float length;

    public int amount;

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

        points = GeneratePoints().ToArray();
    }
    private void OnDrawGizmos()
    {
        for(int i = 0; i < nodes.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(nodes[i].position, 0.1f);

            // Does this node continue?
            if(nodes[i].isEnd == false)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
            }
        }

        for(int i = 0; i < points.Length; i++)
        {
            Handles.DrawWireCube(points[i], Vector3.one * 0.05f);
            Handles.Label(points[i], (i + 1).ToString());
        }
    }

    private IEnumerable<Vector3> GeneratePoints()
    {
        int index = 0;
        float offset = 0.0f;

        float increment = length / amount;

        // Generate evenly spaced points (this was an absolute pain)
        for(int count = 1; count <= amount; count++)
        {
            // No space remains so switch to the next path
            if(offset > nodes[index].length)
            {
                offset -= nodes[index].length;
                index += 1;

                // Completed point generation
                if(nodes[index].isEnd)
                {
                    break;
                }
            }

            // Calculate location
            Vector3 direction = (nodes[index + 1].position - nodes[index].position).normalized;
            Vector3 position = nodes[index].position + (offset * direction);

            // ...
            offset += increment;

            yield return position;
        }
    }
}

[Serializable]
internal struct Node
{
    internal Vector3 position;
    internal float length;
    internal bool isEnd
    {
        get => length <= 0;
    }
}