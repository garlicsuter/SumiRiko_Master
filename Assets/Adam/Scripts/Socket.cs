using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Written by Adam Calvelage | adamjasoncalvelage@gmail.com
/// </summary>
public class Socket : MonoBehaviour
{
    private XRRayInteractor rightController, leftController;

    private GenericObject child = new GenericObject();
    private GenericObject clone = new GenericObject();

    private GameObject heldObject;

    public Material material;

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
        if (child.gameObject != null)
        {
            if (!IsGrabbed(child.gameObject))
            {
                // Lerp child position to the center of the socket
                float distance = Vector3.Distance(child.transform.position, transform.position);
                child.transform.position = distance <= 0.001f ? transform.position : Vector3.Lerp(child.transform.position, transform.position, lerpSpeed * Time.deltaTime);

                // Lerp child rotation to the forward direction of the socket
                float angle = Quaternion.Angle(child.transform.rotation, transform.rotation);
                child.transform.rotation = angle <= 1 ? transform.rotation : Quaternion.Lerp(child.transform.rotation, transform.rotation, lerpSpeed * Time.deltaTime);
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
            heldObject = other.gameObject;

            // Create a clone of the held object as a visual aid
            if (clone.gameObject == null)
            {
                // Fetch values
                clone.gameObject = new GameObject("Clone");
                clone.filter = clone.gameObject.AddComponent<MeshFilter>();
                clone.renderer = clone.gameObject.AddComponent<MeshRenderer>();

                // Copy values
                clone.filter.mesh = heldObject.GetComponent<MeshFilter>().sharedMesh;
                clone.renderer.material = material;
                clone.transform.SetParent(transform);
                clone.transform.position = transform.position;
                clone.transform.rotation = transform.rotation;
                clone.transform.localScale = heldObject.transform.localScale;
            }

            // Check for a valid orientation
            if (IsValid(heldObject))
            {
                clone.color = Color.green;

                // Release the object to confirm selection
                if (!IsGrabbed(heldObject))
                {
                    child.gameObject = heldObject;
                    child.body = heldObject.GetComponent<Rigidbody>();

                    child.transform.SetParent(transform);
                    child.body.isKinematic = true;

                    // Remove visual on confirmation
                    clone.Destroy();
                }
            }
            else
            {
                clone.color = Color.red;
            }
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        Vector3 point = collider.ClosestPoint(transform.position);
        float distance = Vector3.Distance(point, transform.position);

        // Remove visual
        if (collider.gameObject == heldObject && distance >= radius)
        {
            heldObject = null;
            clone.Destroy();
        }

        if (collider.gameObject == child.gameObject && distance >= radius)
        {
            RemoveChild();
        }
    }

    /// <summary>
    /// Does the user have this object in their hand?
    /// </summary>
    public bool IsGrabbed(GameObject obj)
    {
        if (rightController.hasSelection)
        {
            if (rightController.interactablesSelected[0].transform.gameObject == obj)
            {
                return true;
            }
        }
        if (leftController.hasSelection)
        {
            if (leftController.interactablesSelected[0].transform.gameObject == obj)
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

    /// <summary>
    /// Remove and dereference the child
    /// </summary>
    private void RemoveChild()
    {
        // Wait until the user has dropped the child before resetting it
        if (child.gameObject.GetComponent<SocketChildReset>() == null)
        {
            var comp = child.gameObject.AddComponent<SocketChildReset>();
            var func = comp.Wait(this, child.gameObject);
            StartCoroutine(func);
        }

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
internal struct GenericObject
{
    public GameObject gameObject;
    public Rigidbody body;
    public MeshFilter filter;
    public MeshRenderer renderer;

    public Color color
    {
        get => renderer.material.color;
        set => renderer.material.color = value;
    }
    public Transform transform
    {
        get => gameObject.transform;
    }

    public void Destroy()
    {
        UnityEngine.Object.Destroy(gameObject);

        gameObject = null;
        body = null;
        filter = null;
        renderer = null;
    }
}