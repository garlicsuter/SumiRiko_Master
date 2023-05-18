using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//
// Written by Adam Calvelage -> adamjasoncalvelage@gmail.com
//

public class Conveyor : MonoBehaviour
{
    public Node[] nodes;
    private Point[] points;

    public GameObject clone;
    public int amount;

    private float beltLength;
    private float increment;

    private void Start()
    {
        InitNodes();
        float speed = 3.0f;
        InvokeRepeating("Move", speed, speed);
    }
    private void Update()
    {
        for(int i = 0; i < points.Length; i++)
        {
            Point point = points[i];
            Node node = nodes[point.node];

            if(point.offset > node.length)
            {
                point.offset -= node.length;

                int next = point.node + 1;
                bool end = nodes[next].isEnd;

                next = end ? 0 : next;
                node = nodes[next];

                point.node = next;

                point.wishPosition = (node.forward * point.offset) + node.position;

                point.transform.forward = node.forward;
                point.transform.rotation = node.rotation;

                if(end)
                {
                    point.transform.position = node.position;
                    point.wishPosition = node.position;
                }

                if(point.transform.childCount > 0)
                {
                    Destroy(point.transform.GetChild(0).gameObject);
                }
            }

            point.transform.position = Vector3.Lerp(point.transform.position, point.wishPosition, 0.005f);
        }
    }

    private void Move()
    {
        // Iterate through each point and update transform
        for(int i = 0; i < points.Length; i++)
        {
            Point point = points[i];
            Node node = nodes[point.node];

            point.offset += increment;
            point.wishPosition = (node.forward * point.offset) + node.position;
        }
    }
    public void InitNodes()
    {
        // Handle runtime edits
        if(Application.isPlaying)
        {
            // Destroy the children
            for(int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            // ...
            beltLength = 0.0f;
        }

        // ...
        for(int i = 0; i < nodes.Length; i++)
        {
            // Mark endpoint
            if(i + 1 >= nodes.Length)
            {
                nodes[i].isEnd = true;
                break;
            }

            // Helpers
            Node current = nodes[i];
            Node next = nodes[i + 1];
            float distance = Vector3.Distance(next.position, current.position);
            Vector3 forward = (next.position - current.position).normalized;

            // ...
            nodes[i].position = current.position;
            nodes[i].forward = forward;
            nodes[i].rotation = Quaternion.LookRotation(forward);
            nodes[i].length = distance;

            // Increment total length
            beltLength += distance;
        }

        increment = beltLength / amount;

        points = Populate().ToArray();
    }
    private IEnumerable<Point> Populate()
    {
        int index = 0;
        float offset = 0.0f;

        // Generate evenly spaced points
        for(int count = 1; count <= amount; count++)
        {
            // ...
            var current = nodes[index];
            var next = nodes[index + 1];

            // Try to switch to the next node
            if(offset > current.length)
            {
                // ...
                offset -= current.length;
                current = next;
                index += 1;
            }

            // ...
            var point = new Point();
            if(Application.isPlaying)
            {
                point.gameObject = Instantiate(clone, transform);
                point.gameObject.SetActive(true); // They start disabled
                point.transform.position = (current.forward * offset) + current.position;
                point.transform.forward = current.forward;
                point.transform.rotation = current.rotation;

                point.wishPosition = point.transform.position;
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
public class Node
{
    public Vector3 position;
    public Vector3 forward;
    public Quaternion rotation;

    public float length;
    public bool isEnd;
}

[Serializable]
public class Point
{
    public GameObject gameObject;
    public Transform transform => gameObject.transform;

    public Vector3 wishPosition;

    public float offset;
    public int node;
}