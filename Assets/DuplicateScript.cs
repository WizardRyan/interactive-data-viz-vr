using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DuplicateScript : MonoBehaviour
{

    public bool holdingDuplicate = false;

    private bool touchingLController = false;
    private bool touchingRController = false;

    private ManipulitableScript mscript;


    // Start is called before the first frame update
    void Start()
    {
        mscript = FindFirstObjectByType<ManipulitableScript>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInteraction();
    }

    private void HandleInteraction()
    { 
        if((touchingLController || touchingRController) && mscript.gripIsPressed) 
        {
            if(!(UniversalInputHandler.holdingADupe && holdingDuplicate == false))
            {
                transform.position = mscript.lGripPressed ? mscript.leftController.transform.position : mscript.rightController.transform.position;
                holdingDuplicate = true;
            }
        }
        else
        {
            holdingDuplicate = false;
        }
    }

    public void DeleteGlobe(ActivateEventArgs args)
    {
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.name == "Left Controller")
        {
            touchingLController = true;
        }
        else if (other.gameObject.name == "Right Controller")
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
