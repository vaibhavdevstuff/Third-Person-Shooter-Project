using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;

public class GunRecoil : MonoBehaviour
{
    [Header("Virtual Cameras")]
    public CinemachineVirtualCamera followCamera;
    public CinemachineVirtualCamera aimCamera;

    [Header("Recoil Data")]
    public float verticalRecoil = 0.5f;
    public float recoilFrequency;

    private CinemachineVirtualCamera currentCamera;
    private StarterAssetsInputs _input;

    private float followCameraFrequency;
    private float aimCameraFrequency;

    private void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();

        followCameraFrequency = followCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain;
        aimCameraFrequency = aimCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain;
    }

    private void Update()
    {
        if (_input.aim)
            currentCamera = aimCamera;
        else
            currentCamera = followCamera;
    }

    public void GenerateRecoil()
    {
        currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = recoilFrequency;
    }

    public void ResetFrequency()
    {
        followCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = followCameraFrequency;
        aimCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = aimCameraFrequency;
    }












}//class
