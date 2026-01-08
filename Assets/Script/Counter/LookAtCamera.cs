using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private enum Mode { 
        LookAt,
        LookAtInverted,
        CamareForward,
        CamareForwardInverted
    }

    [SerializeField] private Mode mode;

    private void LateUpdate()
    {
        switch (mode)
        {
            case Mode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case Mode.LookAtInverted:
                Vector3 directionToCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + directionToCamera);
                break;
            case Mode.CamareForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case Mode.CamareForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
        }
        
    }
}
