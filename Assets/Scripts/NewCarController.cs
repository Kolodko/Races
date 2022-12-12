using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class NewCarController : MonoBehaviour
{
    [SerializeField] private InputController _inputController;
    [SerializeField] private AnimationCurve _steeringCurve;
    [SerializeField] private WheelColliders _colliders;
    [SerializeField] private WheelMeshes _wheelMeshes;

    private bool _startAcceleration, _startBrake, _stopAcceleration;
    private float _acceleration = 500;
    private float _motorPower = 500;
    private float _brakePower = 80000;
    private float _speed;

    private Rigidbody _playerRB;
    private Sequence _mySequence, _mySequenceBrake;

    public float GasInput;
    public float BrakeInput;
    public float SteeringInput;
    public float SlipAngle;
    
    void Awake()
    {
        _playerRB = gameObject.GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        _speed = _playerRB.velocity.magnitude;
        _inputController.CheckInput();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        ApplyWheelPositions();
    }

    //Tормоз
    void ApplyBrake()
    {
        _colliders.FRWheel.brakeTorque = BrakeInput * _brakePower * 1.8f;
        _colliders.FLWheel.brakeTorque = BrakeInput * _brakePower * 1.8f;

        _colliders.RRWheel.brakeTorque = BrakeInput * _brakePower * 0.8f;
        _colliders.RLWheel.brakeTorque = BrakeInput * _brakePower * 1.8f;

        if (GasInput == 1)
        {
            _mySequence.Kill();
            _startAcceleration = false;
            _mySequenceBrake = DOTween.Sequence();
            _mySequenceBrake.AppendCallback(() => _startBrake = true);
            _mySequenceBrake.AppendInterval(1f);
            _mySequenceBrake.AppendCallback(() => _playerRB.drag = 10);
            _mySequenceBrake.AppendCallback(() => _playerRB.drag = 25);
            _mySequenceBrake.AppendCallback(() => _startBrake = false);
            _mySequenceBrake.Play();
        }
    }

    //Мотор
    void ApplyMotor()
    {
        _colliders.RRWheel.motorTorque = _motorPower * GasInput;
        _colliders.RLWheel.motorTorque = _motorPower * GasInput;
        if (GasInput == -1)
        {
            if (!_stopAcceleration)
            {
                _stopAcceleration = true;
                _playerRB.drag = 0;
                _mySequence = DOTween.Sequence();
                _mySequence.AppendCallback(() => _stopAcceleration = false);
                _mySequence.AppendInterval(3f);
                _mySequence.AppendCallback(() => Acceleration());
            }
        }
        else
        {
            _motorPower = 500;
            _acceleration = 500;
        }
    }

    //Ускорение
    private void Acceleration()
    {
        if (!_startAcceleration)
        {
            Debug.Log("den");
            _mySequence = DOTween.Sequence();
            _mySequence.AppendCallback(() => _startAcceleration = true);
            _mySequence.AppendInterval(1f);
            _mySequence.AppendCallback(() => _acceleration += _acceleration / 100 * 10);
            _mySequence.AppendCallback(() => Debug.Log(_acceleration));
            _mySequence.AppendCallback(() => _playerRB.AddRelativeForce(new Vector3(0, 0, -_acceleration), ForceMode.Impulse));
            _mySequence.AppendCallback(() => _startAcceleration = false);
            _mySequence.Play();
        }
    }

    //Общие повороты
    void ApplySteering()
    {
        float steeringAngle = SteeringInput * _steeringCurve.Evaluate(_speed);

        if (SlipAngle < 120f) steeringAngle += Vector3.SignedAngle(transform.forward, _playerRB.velocity + transform.forward, Vector3.up);
        steeringAngle = Mathf.Clamp(steeringAngle, -90f, 90f);
        _colliders.FRWheel.steerAngle = steeringAngle;
        _colliders.FLWheel.steerAngle = steeringAngle;
    }

    //Повороты колес
    void ApplyWheelPositions()
    {
        UpdateWheel(_colliders.FRWheel, _wheelMeshes.FRWheel);
        UpdateWheel(_colliders.FLWheel, _wheelMeshes.FLWheel);
        UpdateWheel(_colliders.RRWheel, _wheelMeshes.RRWheel);
        UpdateWheel(_colliders.RLWheel, _wheelMeshes.RLWheel);
    }

    //Выравниваем колеса
    void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }

    //Проверка на вьезд на стрелку
    private void OnTriggerEnter(Collider coll)
    {
        if (coll.GetComponent<ArrowController>() != null)
        {
            if (GasInput == -1)
            {
                if (coll.GetComponent<ArrowController>().Car == ArrowController.Arrow.car)
                {
                    if (transform.rotation.eulerAngles.y > -30 && transform.rotation.eulerAngles.y < 30)
                        SpeedArrow(true);
                    else if (transform.rotation.eulerAngles.y > 330 && transform.rotation.eulerAngles.y < 390)
                        SpeedArrow(true);
                    if (transform.rotation.eulerAngles.y > 180 && transform.rotation.eulerAngles.y < 220)
                        SpeedArrow(false);
                    else if (transform.rotation.eulerAngles.y > 110 && transform.rotation.eulerAngles.y <= 180)
                        SpeedArrow(false);
                }
            }
        }
    }

    //Ускорение/замедление от стрелки
    private void SpeedArrow(bool rotation)
    {
        if(rotation)
        {
            Debug.Log("test1");
            _playerRB.AddRelativeForce(new Vector3(0, 0, -40) * (_motorPower / 100 * 15), ForceMode.Impulse);
            _motorPower += _acceleration;
        }
        else
        {
            Debug.Log("test2");
            _playerRB.AddRelativeForce(new Vector3(0, 0, 40) * (_motorPower / 100 * 15), ForceMode.Impulse);
            //_motorPower -= _acceleration;
        }
    }
}

//Колайдеры колес
[System.Serializable]
public class WheelColliders
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;
}

//Меши колес
[System.Serializable]
public class WheelMeshes
{
    public MeshRenderer FRWheel;
    public MeshRenderer FLWheel;
    public MeshRenderer RRWheel;
    public MeshRenderer RLWheel;
}