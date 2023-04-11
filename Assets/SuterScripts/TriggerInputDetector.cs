using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;
using TMPro;

public class TriggerInputDetector : MonoBehaviour
{
    public TextMeshProUGUI leftScoreDisplay;
    public TextMeshProUGUI rightScoreDisplay;

    private InputData _inputData;
    private float _leftMaxScore = 0f;
    private float _rightMaxScore = 0f;
    private float triggerValue;

    public GameObject UI;
    public GameObject UIAnchor;
    private bool UIActive;

    private void Start()
    {
        //should start as false
        UI.SetActive(true);
        UIActive = false;

        _inputData = GetComponent<InputData>();
        //Debug.Log("Started inputData: " + _inputData);
    }
  

    void Update()
    {
        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            
            leftScoreDisplay.text = triggerValue.ToString("#.00");
            //Debug.Log("triggerValue: " + triggerValue);
        }

        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool Abutton))
        {
            //When primary button pressed on right controller, activate the Debug UI
            UIActive = !UIActive;
            //UI.SetActive(UIActive);

            
            rightScoreDisplay.text = Abutton.ToString();
            //Debug.Log("A button: " + Abutton);
        }

        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool Bbutton))
        {
            Debug.Log("B button: " + Bbutton);
        }

        //If the UI is active (from button press above) set position to the canvas anchor
        if (UIActive)
        {
            UI.transform.position = UIAnchor.transform.position;
            UI.transform.eulerAngles = new Vector3(UIAnchor.transform.eulerAngles.x, UIAnchor.transform.eulerAngles.y, 0);
        }
        //triggerValue = ((float)_inputData._leftController.characteristics);
        //Debug.Log("triggerValue is: " + triggerValue);

        //if (_inputData._leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity))
        //{
        //    _leftMaxScore = Mathf.Max(leftVelocity.magnitude, _leftMaxScore);
        //    leftScoreDisplay.text = _leftMaxScore.ToString("F2");
        //}
        //if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVelocity))
        //{
        //    _rightMaxScore = Mathf.Max(rightVelocity.magnitude, _rightMaxScore);
        //    rightScoreDisplay.text = _rightMaxScore.ToString("F2");
        //}
    }
}
