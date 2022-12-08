using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;


public class CarController : MonoBehaviour
{
    private Controls _controls;

    void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }


    void Update()
    {
        
    }
}
