using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{

    public GameObject lookAtTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        FaceTextMeshToCamera();
    }

    void FaceTextMeshToCamera()
    {
        Vector3 origRot = transform.eulerAngles;
        transform.LookAt(Camera.main.transform);
        origRot.y = transform.eulerAngles.y + 180;
        transform.eulerAngles = origRot;
    }
}
