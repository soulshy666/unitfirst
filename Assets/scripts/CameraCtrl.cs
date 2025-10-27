using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCtrl : MonoBehaviour
{
    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public CinemachineImpulseSource Source;
    public So cameraShakeEvent;
    public So shakeEvent;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }
    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
        shakeEvent.OnEventRaised += OnShakeEvent;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        shakeEvent.OnEventRaised -= OnShakeEvent;
    }

    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }
    private void OnShakeEvent()
    {
        Source.GenerateImpulse();
    }
    //TODO:场景切换后更改
    private void Start()
    {
        GetNewCameraBounds();
    }

    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bound");
        if (obj == null)
            return;

        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();

        confiner2D.InvalidateCache();
    }
}
