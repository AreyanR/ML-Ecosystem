using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Camera cameraToFollow;

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraToFollow.transform.position);
    }
}
