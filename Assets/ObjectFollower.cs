using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFollower : MonoBehaviour
{

    public GameObject objectToFollow;
    public float yOffset = 0.0f;

    public bool followX;
    public bool followY;
    public bool followZ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var followVec = new Vector3(
            followX ? objectToFollow.transform.position.x : transform.position.x,
            followY ? objectToFollow.transform.position.y + yOffset : transform.position.y,
            followZ ? objectToFollow.transform.position.z : transform.position.z);
        transform.position = followVec;
    }
}
