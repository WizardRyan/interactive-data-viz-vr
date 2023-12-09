using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyController : MonoBehaviour
{

    public bool controllerInWorldLine = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "World Line")
        {
            controllerInWorldLine = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.name == "World Line")
        {
            controllerInWorldLine = false;
        }
    }
}
