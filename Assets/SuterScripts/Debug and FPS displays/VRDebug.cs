using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VRDebug : MonoBehaviour
{
    public GameObject UI;
    public GameObject UIAnchor;
    private bool UIActive;
    
    
    void Start()
    {
        UI.SetActive(false);
        UIActive = false;
    }


    //void Update()
    //{
    //    if (OVRInput.GetDown(OVRInput.Button.Four))
    //    {
    //        UIActive = !UIActive;
    //        UI.SetActive(UIActive);
    //    }

    //    if (UIActive)
    //    {
    //        UI.transform.position = UIAnchor.transform.position;
    //        UI.transform.eulerAngles = new Vector3(UIAnchor.transform.eulerAngles.x, UIAnchor.transform.eulerAngles.y, 0);
    //    }

    //    if (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger))
    //    {
    //        Debug.Log("Right trigger pressed.");
    //    }

    //    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger))
    //    {
    //        Debug.Log("Left trigger pressed.");
    //    }
    //}
}
