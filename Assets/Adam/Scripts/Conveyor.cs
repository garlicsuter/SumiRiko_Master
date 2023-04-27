using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

public class Conveyor : MonoBehaviour
{
    private GameObject[] clones;
    private _Transform[] points;
    private Node[] nodes;
    private float length;

    public int amount;
    public GameObject clone;

    private void Start()
    {
        InitNodes();

        points = GeneratePoints().ToArray();

        for ( int i = 0; i < points.Length; i++)
        {
            clones[i] = Instantiate(clone, transform);
            clones[i].transform.position = points[i].position;
            clones[i].transform.rotation = points[i].rotation;
        }
    }
    private void Update()
    {
        for( int i = 0; i < points.Length; i++)
        {
            clones[i].transform.position += points[i].forward * Time.deltaTime;
        }
    }
    private void OnValidate()
    {
        if (Application.isPlaying == false)
        {
            InitNodes();
            points = GeneratePoints().ToArray();
        }
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < nodes.Length; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(nodes[i].position, 0.1f);

            // Does this node continue?
            if (nodes[i].isEnd == false)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(nodes[i].position, nodes[i + 1].position);
            }
        }

        int count = 1;
        foreach (var point in points)
        {
            Handles.DrawWireCube(point.position, Vector3.one * 0.05f);
            Handles.Label(point.position, (count++).ToString());
        }
    }

    private void InitNodes()
    {
        // ...
        length = 0.0f;
        nodes = new Node[transform.childCount];

        // ...
        for (int i = 0; i < transform.childCount; i++)
        {
            // Fetch current & next child
            Transform child = transform.GetChild(i);
            Transform next = ((i + 1) < transform.childCount) ? transform.GetChild(i + 1) : null;

            // ...
            float distance = 0.0f;
            Vector3 forward = Vector3.zero;

            // ...
            if (next != null)
            {
                distance = Vector3.Distance(next.position, child.position);
                forward = next.position - child.position;
            }

            // Set values
            nodes[i].position = child.position;
            nodes[i].forward = forward;
            nodes[i].length = distance;
            length += distance;
        }
    }
    private IEnumerable<_Transform> GeneratePoints()
    {
        int index = 0;
        float offset = 0.0f;

        float increment = length / amount;

        // Generate evenly spaced points (this was an absolute pain)
        for (int count = 1; count <= amount; count++)
        {
            // ...
            var node = nodes[index];

            // No space remains so switch to the next path
            if (offset > node.length)
            {
                // Completed point generation
                if (nodes[index + 1].isEnd)
                {
                    break;
                }

                // ...
                offset -= node.length;
                node = nodes[index + 1];
                index += 1;
            }

            // ...
            Vector3 position = node.position + (offset * node.forward.normalized);
            Quaternion rotation = Quaternion.LookRotation(node.forward);

            // ...
            offset += increment;

            yield return new _Transform(position, node.forward, rotation);
        }
    }
}

[Serializable]
internal struct Node
{
    internal Vector3 position;
    internal Vector3 forward;

    internal float length;
    internal bool isEnd
    {
        get => length <= 0;
    }
}

[Serializable]
internal struct _Transform
{
    internal Vector3 position;
    internal Vector3 forward;
    internal Quaternion rotation;

    internal _Transform(Vector3 pos, Vector3 fwd, Quaternion rot)
    {
        position = pos;
        forward = fwd;
        rotation = rot;
    }
}