using UnityEngine;
using UnityEngine.InputSystem;

public class PinchCameraForZoom : MonoBehaviour
{
    [SerializeField] InputAction _touch0Contact;
    [SerializeField] InputAction _touch1Contact;
    [SerializeField] InputAction _touch0Position;
    [SerializeField] InputAction _touch1Position;

    private Transform _cameraMainTransform = null;
    private float _previousMagnitude = 0;
    private int _touchCount = 0;
    private const float _cameraPosY_Default = 18f;
    private const float _cameraPosY_Min = 5f;
    private const float _cameraPosY_Max = 29f;
    private const float _speed = 0.01f;

    void Awake()
    {
        SetReference();
    }

    void OnEnable()
    {
        _touch0Contact.Enable();
        _touch1Contact.Enable();
        _touch0Position.Enable();
        _touch1Position.Enable();
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
        _touch0Contact.Disable();
        _touch1Contact.Disable();
        _touch0Position.Disable();
        _touch1Position.Disable();
    }

    private void SetReference() 
    {
        _cameraMainTransform = GetComponent<Transform>();
    }

    private void SubscribeEvents() 
    {
        _touch0Contact.performed += _ => CountingTouch();
        _touch1Contact.performed += _ => CountingTouch();
        _touch0Contact.canceled += _ => { Initialize(); };
        _touch1Contact.canceled += _ => { Initialize(); };

        _touch1Position.performed += _ =>
        {
            if (_touchCount < 2)
            {
                return;
            }

            ChangeCameraDistance(CalculateMagnitudeDifference());
        };
    }

    private void UnsubscribeEvents() 
    {
        _touch0Contact.performed -= _ => CountingTouch();
        _touch1Contact.performed -= _ => CountingTouch();
        _touch0Contact.canceled -= _ => { Initialize(); };
        _touch1Contact.canceled -= _ => { Initialize(); };
        _touch1Position.performed -= _ => { ChangeCameraDistance(CalculateMagnitudeDifference()); };
    }

    private void CountingTouch() 
    {
        _touchCount++;
    }

    private void Initialize()
    {
        _touchCount = 0;
        _previousMagnitude = 0;
    }

    private float CalculateMagnitudeDifference()
    {
        float magnitude = (_touch0Position.ReadValue<Vector2>() - _touch1Position.ReadValue<Vector2>()).magnitude;

        if (_previousMagnitude == 0)
        {
            _previousMagnitude = magnitude;
        }

        float magnitudeDifference = magnitude - _previousMagnitude;
        _previousMagnitude = magnitude;

        return magnitudeDifference;
    }

    private void ChangeCameraDistance(float magnitudeDifference)
    {
        float cameraPosY_offset = magnitudeDifference * _speed;

        if (cameraPosY_offset + _cameraMainTransform.position.y > _cameraPosY_Min &&
            cameraPosY_offset + _cameraMainTransform.position.y < _cameraPosY_Max)
        {
            _cameraMainTransform.position -= new Vector3(0, cameraPosY_offset, 0);
        }
        else
        {
            cameraPosY_offset = _cameraPosY_Default;
            _cameraMainTransform.position = new Vector3(0, cameraPosY_offset, 0);
        }
    }
}
