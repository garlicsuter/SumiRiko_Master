using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Conveyor : MonoBehaviour
{
    public Node[] nodes;
    private Point[] points;
    private float beltLength;

    public int amount;
    public GameObject clone;

    private void Start()
    {
        InitNodes();
    }
    private void Update()
    {
        for(int i = 0; i < points.Length; i++)
        {
            var point = points[i];

            var time = Time.deltaTime;

            points[i].transform.position += point.transform.forward * time;
            points[i].offset += time;

            if(point.offset > nodes[point.node].length)
            {
                points[i].node = !nodes[point.node].isEnd ? point.node + 1 : 0;
                points[i].offset = 0.0f;

                point = points[i];

                points[i].transform.position = nodes[point.node].position;
                points[i].transform.forward = nodes[point.node].forward;
                points[i].transform.rotation = Quaternion.LookRotation(nodes[point.node].forward);
            }
        }
    }

    public void InitNodes()
    {
        // ...
        beltLength = 0.0f;

        // ...
        for (int i = 0; i < nodes.Length; i++)
        {
            // ...
            if(i + 1 >= nodes.Length)
                break;

            Node cur = nodes[i];
            Node next = nodes[i + 1];

            float distance = Vector3.Distance(next.position, cur.position);

            // ...

            var node = nodes[i];

            node.position = cur.position;
            node.forward = next.position - cur.position;
            node.length = distance;

            nodes[i] = node;


            beltLength += distance;
        }

        points = GeneratePoints().ToArray();
    }
    private IEnumerable<Point> GeneratePoints()
    {
        int index = 0;
        float offset = 0.0f;

        float increment = beltLength / amount;

        // Generate evenly spaced points (this was an absolute pain)
        for(int count = 1; count <= amount; count++)
        {
            // ...
            var node = nodes[index];

            // No space remains so switch to the next path
            if(offset > node.length)
            {
                // Completed point generation
                if(nodes[index + 1].isEnd)
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
            Vector3 forward = node.forward;
            Quaternion rotation = Quaternion.LookRotation(node.forward);

            // ...
            var point = new Point();
            if(Application.isPlaying)
            {
                point.gameObject = Instantiate(clone, transform);
                point.transform.position = position;
                point.transform.forward = forward;
                point.transform.rotation = rotation;
            }
            // Editor
            else
            {
                point.position = position;
                point.forward = forward;
                point.rotation = rotation;
            }

            // ...
            point.offset = offset;
            point.node = index;

            // ...
            offset += increment;

            yield return point;
        }
    }
}

[Serializable]
public struct Node
{
    public Vector3 position;
    public Vector3 forward;

    public float length;
    public bool isEnd
    {
        get => length <= 0;
    }
}

[Serializable]
public struct Point
{
    public GameObject gameObject;
    public Transform transform => gameObject.transform;

    public float offset;
    public int node;

    // Editor
    public Vector3 position;
    public Vector3 forward;
    public Quaternion rotation;
}