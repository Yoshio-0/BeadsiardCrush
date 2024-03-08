using UnityEngine;
using UnityEngine.InputSystem;

public class PinchCameraForZoom : MonoBehaviour
{
    private Transform _cameraMainTransform;

    [SerializeField] InputAction _touch0Contact;
    [SerializeField] InputAction _touch1Contact;
    [SerializeField] InputAction _touch0Position;
    [SerializeField] InputAction _touch1Position;

    private float _previousMagnitude = 0;
    private int _touchCount = 0;

    private const float _cameraPosY_Default = 18f;
    private const float _cameraPosY_Min = 5f;
    private const float _cameraPosY_Max = 29f;
    private const float _speed = 0.01f;

    void Awake()
    {
        _cameraMainTransform = GetComponent<Transform>();
    }

    void OnEnable()
    {
        _touch0Contact.Enable();
        _touch1Contact.Enable();
        _touch0Position.Enable();
        _touch1Position.Enable();

        _touch0Contact.performed += _ => _touchCount++;
        _touch1Contact.performed += _ => _touchCount++;
        
        _touch0Contact.canceled += _ =>
        {
            _touchCount = 0;
            _previousMagnitude = 0;
        };

        _touch1Contact.canceled += _ =>
        {
            _touchCount = 0;
            _previousMagnitude = 0;
        };

        _touch1Position.performed += _ =>
        {
            if (_touchCount < 2)
            {
                return;
            }

            var magnitude = (_touch0Position.ReadValue<Vector2>() - _touch1Position.ReadValue<Vector2>()).magnitude;
            if (_previousMagnitude == 0)
            {
                _previousMagnitude = magnitude;
            }
            var difference = magnitude - _previousMagnitude;
            _previousMagnitude = magnitude;

            ChangeCameraDistance(difference);
        };
    }

    private void ChangeCameraDistance(float difference)
    {
        float cameraPosY_offset = difference * _speed;

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
