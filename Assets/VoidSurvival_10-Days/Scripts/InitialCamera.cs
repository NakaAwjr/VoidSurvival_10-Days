using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class InitialCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    // Start is called before the first frame update
    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _virtualCamera.Follow = FindAnyObjectByType<PlayerController>().transform;
    }
}
