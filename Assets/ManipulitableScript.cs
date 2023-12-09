using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class ManipulitableScript : MonoBehaviour
{

    public GameObject worldLine;
    public float startPercent = 0;

    public float myWidth;

    public bool inInteractibleArea = true;

    public MyController rightController;
    public MyController leftController;

    private bool snapBack = false;

    private InputDevice rightControllerDevice;
    private InputDevice leftControllerDevice;

    private MyController currentController;

    public bool lGripPressed = false;
    public bool rGripPressed = false;

    public bool gripIsPressed = false;

    public GameObject duplicatePrefab;

    private List<UnityEngine.XR.InputDevice> gameControllers = new List<InputDevice>();
    private float currentOffsetAmount = 0;
    private float prevPosition = -1;

    private Vector3 currentRotation = Vector3.zero;
    private Vector3 prevRotation = Vector3.zero;

    private float worldLineWidth;
    private float leftPoint;
    private float rightPoint;

    private bool touchingLController = false;
    private bool touchingRController = false;

    private int prevYear = -1;
    private int year;
    private bool yearChanged = false;
    private bool setInitialTex = false;

    private Renderer m_Renderer;
    private Rigidbody m_Rigidbody;

    private TextMeshPro label;

    private bool createdDupe = false;


    // Start is called before the first frame update
    void Start()
    {
        worldLineWidth = worldLine.transform.localScale.y;
        leftPoint = worldLine.transform.position.x - worldLineWidth;
        rightPoint = worldLine.transform.position.x + worldLineWidth;

        m_Renderer = GetComponent<Renderer>();
        m_Rigidbody = GetComponent<Rigidbody>();

        label = GetComponentInChildren<TextMeshPro>();
    }

    private void SetControllers()
    {
        try
        {
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, gameControllers);
            rightControllerDevice = gameControllers[0];

            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, gameControllers);
            leftControllerDevice = gameControllers[0];
        }
        catch(System.Exception e)
        {

        }
    }

    private void GetControllerVals()
    {
        leftControllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out lGripPressed);
        rightControllerDevice.TryGetFeatureValue(CommonUsages.gripButton, out rGripPressed);

        gripIsPressed = (lGripPressed || rGripPressed);
        currentController = lGripPressed ? leftController : rightController;
    }

    private void HandleInteraction()
    {
        if (inInteractibleArea)
        {
            transform.position = new Vector3(
            Mathf.Clamp(leftPoint + currentOffsetAmount, leftPoint, rightPoint),
            worldLine.transform.position.y,
            worldLine.transform.position.z);

            //Debug.Log(gripIsPressed);
            if (gripIsPressed && !snapBack && (touchingLController || touchingRController))
            {
                if (prevPosition != -1)
                {
                    currentOffsetAmount += currentController.transform.position.x - prevPosition;
                }
                prevPosition = currentController.transform.position.x;

                if (yearChanged)
                {
                    SetTex();
                }
            }
            else
            {
                prevPosition = -1;
            }
            if (!gripIsPressed && snapBack)
            {
                snapBack = false;
            }
        }

        if ((lGripPressed && !leftController.controllerInWorldLine) || (rGripPressed && !rightController.controllerInWorldLine))
        {
            if (!createdDupe && (touchingLController || touchingRController))
            {
                if (!UniversalInputHandler.holdingADupe)
                {
                    var dupPos = lGripPressed ? leftController.transform.position : rightController.transform.position;
                    var dupedGlobe = GameObject.Instantiate(duplicatePrefab, dupPos, transform.rotation);
                    dupedGlobe.GetComponent<Renderer>().material.mainTexture = DataMaster.GetHeatMapTex(year);
                    dupedGlobe.GetComponentInChildren<TextMeshPro>().text = $"{label.text}";
                    dupedGlobe.transform.localScale = Vector3.one * UniversalInputHandler.currentScale;
                    createdDupe = true;
                }
            }
        }

        if(leftController.controllerInWorldLine || rightController.controllerInWorldLine && !UniversalInputHandler.holdingADupe)
        {
            createdDupe = false;
        }
    }

    private void SetYear()
    {
        prevYear = year;

        var currentPosition = transform.position.x;
        var mLeftPoint = leftPoint;
        var mRightPoint = rightPoint;

        if(mLeftPoint < 0)
        {
            mRightPoint += (mLeftPoint * -1);
            currentPosition += (mLeftPoint * -1);
        }
        if(mLeftPoint > 0)
        {
            mRightPoint -= mLeftPoint;
            currentPosition -= mLeftPoint;
        }
        mLeftPoint = 0;

        var percentage = currentPosition / mRightPoint;

        year = Mathf.RoundToInt(Mathf.Lerp(DataMaster.StartYear, DataMaster.EndYear, percentage));

        if(year != prevYear || prevYear == -1)
        {
            yearChanged = true;
        }
        else
        {
            yearChanged = false;
        }
    }

    private void SetTex()
    {
        Debug.Log($"Setting tex for year {year}");
        var tex = DataMaster.GetHeatMapTex(year);

        Debug.Log( tex );
        m_Renderer.material.mainTexture = tex;
        label.text = year.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        SetControllers();
        GetControllerVals();
        HandleInteraction();
        SetYear();
        if (!setInitialTex)
        {
            SetTex();
            setInitialTex = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
    }

    private void OnTriggerStay(Collider other)
    { 
        if(other.gameObject.name == "Left Controller")
        {
            touchingLController= true;
        }
        else if(other.gameObject.name == "Right Controller")
        {
            touchingRController = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Left Controller")
        {
            touchingLController = false;
        }
        else if (other.gameObject.name == "Right Controller")
        {
            touchingRController = false;
        }
    }
}
