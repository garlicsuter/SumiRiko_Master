using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Written by Adam Calvelage | adamjasoncalvelage@gmail.com
/// </summary>
public class Socket : MonoBehaviour
{
    // References
    private XRRayInteractor rightController, leftController;
    private SocketChild child = new SocketChild();
    private MeshRenderer meshRenderer;

    public Color OverlayColor
    {
        get => meshRenderer.material.color;
        set => meshRenderer.material.color = value;
    }

    // TODO: Make it ratio between 0 to 1 where radius impacts speed?
    public float lerpSpeed = 5.0f;

    public float radius = 0.5f;

    // TODO: Tolerance for each axis
    public float tolerance;

    private void Start()
    {
        // Controllers
        rightController = GameObject.Find("LeftHand Controller").GetComponent<XRRayInteractor>();
        leftController = GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>();

        meshRenderer = gameObject.GetComponent<MeshRenderer>();

        // Trigger
        var collider = gameObject.AddComponent<SphereCollider>();
        collider.hideFlags = HideFlags.HideInInspector;
        collider.isTrigger = true;
        collider.radius = radius;
    }

    private void Update()
    {
        if(child.gameObject != null)
        {
            if(!IsGrabbed(child.gameObject))
            {
                // Lerp child to the center of the socket
                float distance = Vector3.Distance(child.gameObject.transform.position, transform.position);
                if(distance <= 0.001f)
                {
                    child.gameObject.transform.position = transform.position;
                }
                else
                {
                    child.gameObject.transform.position = Vector3.Lerp(child.gameObject.transform.position, transform.position, lerpSpeed * Time.deltaTime);
                }
            }
            else
            {
                RemoveChild();
            }
        }
        else
        {
            // Ensure that children get detached from the parent
            if(transform.childCount > 0)
            {
                transform.DetachChildren();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(child.gameObject == null)
        {
            var obj = other.gameObject;

            // Modify color of the socket based off
            // of the transform of the held object
            if(IsValid(obj))
            {
                OverlayColor = Color.green;

                // Releasing the object will confirm the selection
                if(!IsGrabbed(obj))
                {
                    child.gameObject = obj;
                    child.body = obj.GetComponent<Rigidbody>();

                    child.gameObject.transform.SetParent(transform);
                    child.body.isKinematic = true;

                    // Preserve horizontal rotation
                    child.gameObject.transform.rotation = Quaternion.Euler(0, child.gameObject.transform.rotation.eulerAngles.y, 0);

                    OverlayColor = Color.clear;
                }
            }
            else
            {
                OverlayColor = Color.red;
            }
        }
    }

    // NOTE: I have no idea if this even does anything
    private void OnTriggerExit(Collider collider)
    {
        Vector3 point = collider.ClosestPoint(transform.position);
        float distance = Vector3.Distance(point, transform.position);

        if(collider.gameObject == child.gameObject && distance > radius)
        {
            RemoveChild();
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
        child.body.isKinematic = false;

        child.gameObject = null;
        child.body = null;
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