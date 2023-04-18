using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectOrientationController : MonoBehaviour
{
    //ATTACH THIS SCRIPT TO THE OBJECT, NOT THE SOCKET
    //INSERT SOCKET INTO PUBLIC GAME OBJECT IN THE INSPECTOR

    public float minXAngle = 0f;
    public float maxXAngle = 90f;

    public float minYAngle = 0f;
    public float maxYAngle = 90f;

    public float minZAngle = 0f;
    public float maxZAngle = 90f;

    //For ranges that are only postive (example 0-90)

    public float minXAngle2 = 0f;
    public float maxXAngle2 = 90f;

    public float minYAngle2 = 0f;
    public float maxYAngle2 = 90f;

    public float minZAngle2 = 0f;
    public float maxZAngle2 = 90f;

    //Enable only if your range goes from negative and postive (example 270-90)
    //For the example above, enable second set of variables and set one to (270-360) and another to (0-90)
    //Otherwise Comment Out Sections to Avoid Unnessary Variables

    public Material materialInRange;
    public Material materialNotInRange;

    //[SerializeField] XRBaseController controller; 
    //Haptic Feedback (In Progress)

    public GameObject socket;

    private void Update()
    {
        //Debug.Log("X Rotation is" + transform.eulerAngles.x);
        //Debug.Log("Y Rotation is" + transform.eulerAngles.y);
        //Debug.Log("Z Rotation is" + transform.eulerAngles.z);
        
        //Enable Each One at a Time and Look at the Console to Figure Out Ranges

        Vector3 objectRotation = transform.rotation.eulerAngles;

        if (((objectRotation.x >= minXAngle && objectRotation.x <= maxXAngle) || (objectRotation.x >= minXAngle2 && objectRotation.x <= maxXAngle2))
            &&
            ((objectRotation.y >= minYAngle && objectRotation.y <= maxYAngle) || (objectRotation.y >= minYAngle2 && objectRotation.y <= maxYAngle2))
            &&
            ((objectRotation.z >= minZAngle && objectRotation.z <= maxZAngle) || (objectRotation.z >= minZAngle2 && objectRotation.z <= maxZAngle2)))
        {
            socket.SetActive(true);
            Debug.Log("Object is in correct orientation");
            //Sets Socket to True, Thus Object is Insertable

            GetComponent<Renderer>().material = materialInRange;
            //Changes Color of Object to Inserted Color in Inspector for Material In Range

            //controller.SendHapticImpulse(0.7f, 0.2f); (Not Working)
            //Haptic Feedback When In Range

            // Things that Happen When It is In Range
        }
        else
        {
            socket.SetActive(false);
            Debug.Log("Object is not in correct orientation");
            //Sets Socket to False, Thus Object in Uninsertable

            GetComponent<Renderer>().material = materialNotInRange;
            //Changes Color of Object to Inserted Color in Inspector for Material Not In Range

            // Things that Happen When It is Not In Range
        }
    }
}

