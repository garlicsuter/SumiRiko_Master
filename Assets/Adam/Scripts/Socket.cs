using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Written by Adam Calvelage | adamjasoncalvelage@gmail.com
/// </summary>
public class Socket : MonoBehaviour
{
    private XRRayInteractor rightController, leftController;
    private SocketChild child = new SocketChild();

    private Rigidbody prevChild;

    private GameObject clone;
    private MeshRenderer cloneRenderer;

    public Material cloneMaterial;

    // TODO: Make it ratio between 0 to 1 where radius impacts speed?
    public float lerpSpeed = 5.0f;

    public float radius = 0.5f;

    // TODO: Tolerance for each axis
    public float tolerance;

    private void Start()
    {
        rightController = GameObject.Find("LeftHand Controller").GetComponent<XRRayInteractor>();
        leftController = GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>();

        var collider = gameObject.AddComponent<SphereCollider>();
        collider.hideFlags = HideFlags.HideInInspector;
        collider.isTrigger = true;
        collider.radius = radius;
    }

    private void Update()
    {
        if (prevChild.isKinematic)
        {
            prevChild.isKinematic = false;
        }

        if (child.gameObject != null)
        {
            if (!IsGrabbed(child.gameObject))
            {
                // Lerp child position to the center of the socket
                float distance = Vector3.Distance(child.gameObject.transform.position, transform.position);
                child.gameObject.transform.position = distance <= 0.001f ? transform.position : Vector3.Lerp(child.gameObject.transform.position, transform.position, lerpSpeed * Time.deltaTime);

                // Lerp child rotation to the forward direction of the socket
                float angle = Quaternion.Angle(child.gameObject.transform.rotation, transform.rotation);
                child.gameObject.transform.rotation = angle <= 5 ? transform.rotation : Quaternion.Lerp(child.gameObject.transform.rotation, transform.rotation, lerpSpeed * Time.deltaTime);
            }
            else
            {
                RemoveChild();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (child.gameObject == null)
        {
            var obj = other.gameObject;

            // Provide a visual aid of how the object should be positioned
            if (clone == null)
            {
                clone = new GameObject("Reference");

                var refMesh = obj.GetComponent<MeshFilter>();

                cloneRenderer = clone.AddComponent<MeshRenderer>();
                var cloneFilter = clone.AddComponent<MeshFilter>();


                cloneRenderer.material = cloneMaterial;
                cloneFilter.mesh = refMesh.sharedMesh;

                clone.transform.SetParent(transform);

                clone.transform.position = transform.position;
                clone.transform.rotation = transform.rotation;
                clone.transform.localScale = obj.transform.localScale;
            }

            // Modify color of the socket based off of the transform of the held object
            if (IsValid(obj))
            {
                cloneRenderer.material.color = Color.green;

                // Releasing the object will confirm the selection
                if (!IsGrabbed(obj))
                {
                    print("ASSIGNED");

                    child.gameObject = obj;
                    child.body = obj.GetComponent<Rigidbody>();
                    child.gameObject.transform.SetParent(transform);
                    child.body.isKinematic = true;

                    // Make mesh transparent
                    cloneRenderer.material.color = Color.clear;
                }
            }
            else
            {
                cloneRenderer.material.color = Color.red;
            }
        }
    }

    // NOTE: I have no idea if this even does anything
    private void OnTriggerExit(Collider collider)
    {
        Vector3 point = collider.ClosestPoint(transform.position);
        float distance = Vector3.Distance(point, transform.position);

        if (distance >= radius)
        {
            if (collider.gameObject == child.gameObject)
            {
                RemoveChild();
            }
        }
    }

    /// <summary>
    /// Does the user have this object in their hand?
    /// </summary>
    private bool IsGrabbed(GameObject obj)
    {
        if(rightController.hasSelection)
        {
            if(rightController.interactablesSelected[0].transform.gameObject == obj)
            {
                return true;
            }
        }
        if(leftController.hasSelection)
        {
            if(leftController.interactablesSelected[0].transform.gameObject == obj)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Does this object align with this socket?
    /// </summary>
    public bool IsValid(GameObject obj)
    {
        float angle = Quaternion.Angle(obj.transform.rotation, transform.rotation);
        angle = Math.Abs(angle);
        return angle <= tolerance;
    }

    private void RemoveChild()
    {
        print("REMOVE");

        child.gameObject.transform.parent = null;
        child.body.isKinematic = false;

        prevChild = child.body;

        child.gameObject = null;
        child.body = null;

        Destroy(clone);
    }

    private void OnDrawGizmos()
    {
        // Draw a sphere
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Draw an invisible sphere to allow selection in scene window
        Gizmos.color = Color.clear;
        Gizmos.DrawSphere(transform.position, radius);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw an arrow
        Gizmos.color = Color.red;
        float length = radius * 1.5f;
        float arms = length * 0.25f;
        Vector3 end = (transform.forward * length) + transform.position;
        Gizmos.DrawLine(transform.position, end);
        Gizmos.DrawLine(end, (transform.right - transform.forward) * arms + end);
        Gizmos.DrawLine(end, (-transform.right - transform.forward) * arms + end);
    }
}

[Serializable]
internal class SocketChild
{
    public GameObject gameObject = null;
    public Rigidbody body = null;
}