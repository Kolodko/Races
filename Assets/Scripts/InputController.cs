using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private NewCarController _carController;
    private Rigidbody _playerRB;

    void Awake()
    {
        _playerRB = gameObject.GetComponent<Rigidbody>();
    }

    //”правление
    public void CheckInput()
    {
        _carController.GasInput = Input.GetAxis("Vertical");
        _carController.SteeringInput = Input.GetAxis("Horizontal");
        _carController.SlipAngle = Vector3.Angle(transform.forward, _playerRB.velocity - transform.forward);

        float movingDirection = Vector3.Dot(transform.forward, _playerRB.velocity);

        if (movingDirection < -0.5f && _carController.GasInput > 0)
            _carController.BrakeInput = Mathf.Abs(_carController.GasInput);
        else if (movingDirection > 0.5f && _carController.GasInput < 0)
            _carController.BrakeInput = Mathf.Abs(_carController.GasInput);
        else 
            _carController.BrakeInput = 0;
    }
}