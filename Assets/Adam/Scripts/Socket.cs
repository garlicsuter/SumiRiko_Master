using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

//
// Written by Adam Calvelage -> adamjasoncalvelage@gmail.com
//

public class Socket : MonoBehaviour
{
    private XRRayInteractor rightController, leftController;

    private GenericObject child, clone;

    private GameObject heldObject;

    private float radius;
    private float speed;


    [Tooltip("Up/Down, Left/Right, Tilt")]
    public Vector3 angles;

    [Tooltip("Scale of the Trigger")]
    public float size = 1.0f;

    [Range(0.0f, 1.0f)]
    [Tooltip("Slow - Near-Instant")]
    public float speedRatio = 0.5f;

    [Tooltip("Colorless Translucent Material")]
    public Material material;


    private void Start()
    {
        rightController = GameObject.Find("LeftHand Controller").GetComponent<XRRayInteractor>();
        leftController = GameObject.Find("RightHand Controller").GetComponent<XRRayInteractor>();

        radius = size * 0.5f;
        speed = speedRatio > 0 ? speedRatio * 10 : 0.1f;

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
                // Lerp position to the center of the socket
                float distance = Vector3.Distance(child.transform.position, transform.position);
                child.transform.position = distance <= 0.001f ? transform.position : Vector3.Lerp(child.transform.position, transform.position, speed * Time.deltaTime);

                // Lerp rotation to the forward direction of the socket
                float angle = Quaternion.Angle(child.transform.rotation, transform.rotation);
                child.transform.rotation = angle <= 0.5f ? transform.rotation : Quaternion.Lerp(child.transform.rotation, transform.rotation, speed * Time.deltaTime);
            }
            else
            {
                StartCoroutine(RemoveChild());
            }
        }
    }
    private void OnTriggerStay(Collider collider)
    {
        if(child.gameObject == null)
        {
            // [BUG REPORT][Visual] - AC
            // Position two or more alternative invalid meshes, remove cloned object,
            // cloned mesh is not altered until the current heldObject is removed.
            // Requires some thought to fix but will never get noticed so...

            // Any interactable that contacts this trigger
            heldObject = collider.gameObject;

            // Setup a clone of the held object
            if(clone.gameObject == null)
            {
                // Create object and components
                clone.gameObject = new GameObject("Clone");
                clone.filter = clone.gameObject.AddComponent<MeshFilter>();
                clone.renderer = clone.gameObject.AddComponent<MeshRenderer>();

                // Copy values to the cloned object
                clone.filter.mesh = heldObject.GetComponent<MeshFilter>().sharedMesh;
                clone.renderer.material = material;
                clone.transform.SetParent(transform);
                clone.transform.SetPositionAndRotation(transform.position, transform.rotation);
                clone.transform.localScale = heldObject.transform.localScale;
            }

            // Check for a valid orientation
            if(IsValid(heldObject))
            {
                // Valid color
                clone.color = Color.green;

                // Release the object to confirm selection
                if(!IsGrabbed(heldObject))
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
                // Invalid color
                clone.color = Color.red;
            }
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        Vector3 point = collider.ClosestPoint(transform.position);
        float distance = Vector3.Distance(point, transform.position);

        // Remove visual
        if(collider.gameObject == heldObject && distance >= radius)
        {
            heldObject = null;
            clone.Destroy();
        }
    }
    private void OnDrawGizmos()
    {
        // Draw a sphere
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, radius);

        // Draw an invisible sphere to allow selection in the scene window
        Gizmos.color = Color.clear;
        Gizmos.DrawSphere(transform.position, radius);
    }
    private void OnDrawGizmosSelected()
    {
        // Draw an arrow
        Gizmos.color = Color.red;
        float length = radius * 1.5f;
        float arms = length * 0.25f;
        Vector3 end = transform.forward * length + transform.position;
        Gizmos.DrawLine(transform.position, end);
        Gizmos.DrawLine(end, (transform.right - transform.forward) * arms + end);
        Gizmos.DrawLine(end, (-transform.right - transform.forward) * arms + end);
    }
    private void OnValidate()
    {
        radius = size * 0.5f;
        speed = speedRatio > 0 ? speedRatio * 10 : 0.1f;

        if(gameObject.TryGetComponent<SphereCollider>(out SphereCollider collider))
        {
            collider.radius = radius;
        }
    }


    /// <summary>
    /// Does the user have this object in their hand?
    /// </summary>
    private bool IsGrabbed(GameObject obj)
    {
        if(rightController.hasSelection)
        {
            if(rightController.interactablesSelected[0].transform.gameObject == obj) // How can one controller select multiple interactables anyway?
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
    /// Does this object fit within the tolerances?
    /// </summary>
    private bool IsValid(GameObject obj)
    {
        float x = Quaternion.Angle(Quaternion.Euler(obj.transform.eulerAngles.x, 0, 0), Quaternion.Euler(transform.eulerAngles.x, 0, 0));
        float y = Quaternion.Angle(Quaternion.Euler(0, obj.transform.eulerAngles.y, 0), Quaternion.Euler(0, transform.eulerAngles.y, 0));
        float z = Quaternion.Angle(Quaternion.Euler(0, 0, obj.transform.eulerAngles.z), Quaternion.Euler(0, 0, transform.eulerAngles.z));
        return x <= angles.x && y <= angles.y && z <= angles.z;
    }
    /// <summary>
    /// Dereference and reset the child
    /// </summary>
    private IEnumerator RemoveChild()
    {
        var obj = child.gameObject;

        child.gameObject = null;
        child.body = null;

        // Wait until the user has dropped the child because
        // the interactable component does not like this
        while(IsGrabbed(obj))
        {
            yield return null;
        }

        obj.GetComponent<Rigidbody>().isKinematic = false;
        obj.transform.parent = null;
    }
}


/// <summary>
/// An organized and modular collection of references
/// </summary>
[Serializable]
internal struct GenericObject
{
    internal GameObject gameObject;
    internal Rigidbody body;
    internal MeshFilter filter;
    internal MeshRenderer renderer;

    internal Color color
    {
        get => renderer.material.color;
        set => renderer.material.color = value;
    }

    internal Transform transform
    {
        get => gameObject.transform;
    }

    internal void Destroy()
    {
        UnityEngine.Object.Destroy(gameObject);

        gameObject = null;
        body = null;
        filter = null;
        renderer = null;
    }
}