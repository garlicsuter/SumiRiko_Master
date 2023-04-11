using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;
//using UnityEngine.XR.OpenXR.Input;

public class TestActionInput : MonoBehaviour
{
    public ActionBasedController controller;
    public TextMeshPro text;
    public float isPressed;

    void Start()
    {
        //controller = GetComponent<ActionBasedController>();

        controller.selectAction.action.performed += Action_performed;
    }

    private void Action_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        Debug.Log("some Button is pressed");
    }

    // Update is called once per frame
    void Update()
    {
        isPressed = controller.selectAction.action.ReadValue<float>();
        text.text = "TMProText Here + ispressed?" + isPressed;
        //Debug.Log(controller.selectAction.action.ReadValue<float>());
    }
}