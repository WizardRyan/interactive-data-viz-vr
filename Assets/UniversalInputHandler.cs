using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Utilities.Tweenables;

public class UniversalInputHandler : MonoBehaviour
{

    public static bool holdingADupe = false;

    public XRRayInteractor lInteractor;
    public XRRayInteractor rInteractor;

    public GameObject precipPrefab;

    private InputDevice rightControllerDevice;
    private InputDevice leftControllerDevice;
    private List<UnityEngine.XR.InputDevice> gameControllers = new List<InputDevice>();
    private Vector2 primaryAxis = Vector2.zero;

    private bool aPressed = false;
    private bool bPressed = false;
    private bool shownAltGlobes = false;
    private bool globesAreCurrentlyShown = false;
    private bool sortedGlobes = false;

    public static float currentScale = 100.0f;

    private ManipulitableScript mScript;

    // Start is called before the first frame update
    void Start()
    {
        mScript = GameObject.FindFirstObjectByType<ManipulitableScript>();
        lInteractor.enabled = false;
        rInteractor.enabled = false;
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
        catch (System.Exception e)
        {

        }
    }

    // Update is called once per frame
    void Update()
    {
        SetControllers();
        HandleRotationScaling();
        CheckDupes();
        HandleSorting();
        HandleDeletion();
        HandleShowPrecipitation();
    }

    private void HandleShowPrecipitation()
    {
        rightControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bPressed);
        if (!bPressed)
        {
            leftControllerDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bPressed);
        }

        if (!bPressed)
        {
            shownAltGlobes = false;
        }

        if (bPressed && !shownAltGlobes)
        {
            if (globesAreCurrentlyShown)
            {
                foreach(var precipGlobe in GameObject.FindGameObjectsWithTag("precipGlobe"))
                {
                    Destroy(precipGlobe);
                }
                globesAreCurrentlyShown = false;
            }
            else
            {
                foreach(var globe in FindObjectsByType<DuplicateScript>(FindObjectsSortMode.None))
                {
                    var year = Int32.Parse(globe.gameObject.GetComponentInChildren<TextMeshPro>().text);

                    if (DataMaster.RainfallDataExists(year))
                    {
                        var precipGlobe = GameObject.Instantiate(precipPrefab);
                        precipGlobe.transform.parent = globe.transform;
                        precipGlobe.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
                        precipGlobe.transform.localPosition = new Vector3(0, 0, -0.0021f);
                        precipGlobe.GetComponent<Renderer>().material.mainTexture = DataMaster.GetRainfallTex(year);
                    }
                }
                globesAreCurrentlyShown = true;
            }
            shownAltGlobes = true;
        }

    }

    private void HandleDeletion()
    {
        if(mScript.gripIsPressed && !holdingADupe)
        {
            if (mScript.lGripPressed)
            {
                lInteractor.enabled = true;
            }
            else if (mScript.rGripPressed)
            {
                rInteractor.enabled = true;
            }
        }
        else
        {
            lInteractor.enabled = false;
            rInteractor.enabled = false;
        }
    }

    private void HandleSorting()
    {
        rightControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out aPressed);
        if (!aPressed)
        {
            leftControllerDevice.TryGetFeatureValue(CommonUsages.primaryButton, out aPressed);
        }

        if (!aPressed)
        {
            sortedGlobes = false;
        }

        if (aPressed && !sortedGlobes)
        {
            var globeWidth = (currentScale / 500);
            var margin = 0.03f;
            var z = 0.0f;
            var x = 0.7f;

            var rowSize = 2.0f;
            var currentRowSize = 0f;
            var y = mScript.worldLine.transform.position.y;


            List<DuplicateScript> allDupes = new List<DuplicateScript>();
            var dupeArr = FindObjectsByType<DuplicateScript>(FindObjectsSortMode.None);
            allDupes.AddRange(dupeArr);

            allDupes.Sort((a, b) => Int32.Parse(a.GetComponentInChildren<TextMeshPro>().text).CompareTo(Int32.Parse(b.GetComponentInChildren<TextMeshPro>().text)));
            //sort globes

            var dupeLocations = new List<Vector3>();
            foreach(var globe in allDupes)
            {
                dupeLocations.Add(new Vector3(x, y, z));
                z -= (globeWidth + margin);
                currentRowSize += (globeWidth + margin);
            }

            currentRowSize -= (globeWidth + margin);

            var i = 0;
            Debug.Log($"Current row size: {currentRowSize} Current z: {z}");
            foreach(var globe in allDupes)
            {
                dupeLocations[i] = new Vector3(
                    dupeLocations[i].x,
                    dupeLocations[i].y,
                    dupeLocations[i].z + (currentRowSize / 2) - 0.1f
                    );
                globe.transform.DOMove(dupeLocations[i], 0.8f);
                i++;
            }
            sortedGlobes = true;
        }
    }

    private void CheckDupes()
    {
        var allDupes = GameObject.FindObjectsByType<DuplicateScript>(FindObjectsSortMode.None);
        holdingADupe = false;

        foreach (var dupe in allDupes)
        {
            if (dupe.holdingDuplicate)
            {
                holdingADupe = true;
                break;
            }
        }
    }

    private void HandleRotationScaling()
    {
        leftControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out primaryAxis);
        rightControllerDevice.TryGetFeatureValue(CommonUsages.primary2DAxis, out primaryAxis);
        float rotSpeed = 200.0f * Time.deltaTime;
        float sizeSpeed = 50f * Time.deltaTime;
        foreach(var globe in GameObject.FindGameObjectsWithTag("globe"))
        {
            globe.transform.Rotate(Vector3.up, primaryAxis[0] * rotSpeed * -1, Space.World);

            var sizeMovement = primaryAxis[1];
            if(Mathf.Abs(sizeMovement) < 0.8)
            {
                sizeMovement = 0;
            }

            if(globe.name != "EarthUnwrap")
            {
                currentScale += (sizeMovement / sizeSpeed);
                globe.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
            }
        }
    }
}
