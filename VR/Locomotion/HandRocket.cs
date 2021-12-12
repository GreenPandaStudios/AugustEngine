using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandRocket : MonoBehaviour
{
    [SerializeField] ActionBasedController handController;
    [SerializeField] Transform hand;
    [SerializeField] Rigidbody body;
    [SerializeField] ParticleSystem rocketParticles;


    private bool pressing;
    private bool Pressing
    {
        get => pressing;
        set
        {
            if (pressing = value)
                rocketParticles.Play();
            else
                rocketParticles.Stop();
        }
    }
    private void OnEnable()
    {
        handController.uiPressAction.action.started += _ => { Pressing = true; };
        handController.uiPressAction.action.canceled += _ => { Pressing = false; };
    }
    private void OnDisable()
    {
        handController.uiPressAction.action.started -= _ => { Pressing = true; };
        handController.uiPressAction.action.canceled -= _ => { Pressing = false; };
    }
    private void FixedUpdate()
    {
        if (pressing)
        {
            body.AddForce(-hand.forward * 15f);
        }

    }


}
