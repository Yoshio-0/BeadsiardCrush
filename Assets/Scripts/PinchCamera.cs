using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class PinchCamera : MonoBehaviour
{
    [SerializeField] InputAction _pinchCamera = null;

    private Transform _cameraMainTransform;
    private const float _cameraPosY_Min = 5f;
    private const float _cameraPosY_Max = 29f;
    private const float _cameraPosY_Default = 18f;
    private const float _offset = 0.01f;

    void Awake()
    {
        _cameraMainTransform = GetComponent<Transform>();
    }

    void OnEnable()
    {
        EnhancedTouchSupport.Enable();
        _pinchCamera.Enable();
        _pinchCamera.performed += context => CalculateDistanceBetweenFingers(context);
    }

    void OnDisable()
    {
        _pinchCamera.performed -= context => CalculateDistanceBetweenFingers(context);
        EnhancedTouchSupport.Disable();
        _pinchCamera.Disable();
    }

    private void CalculateDistanceBetweenFingers(InputAction.CallbackContext context) 
    {
        if (Touch.activeTouches.Count < 2)
        {
            return;
        }

        Touch primary = Touch.activeTouches[0];
        Touch secondary = Touch.activeTouches[1];

        if (primary.phase == TouchPhase.Moved ||
            secondary.phase == TouchPhase.Moved)
        {
            if (primary.history.Count == 0 ||
                secondary.history.Count == 0)
            {
                return;
            }

            float currentDistance = Vector2.Distance(primary.screenPosition, secondary.screenPosition);
            float previousDistance = Vector2.Distance(primary.history[0].screenPosition, secondary.history[0].screenPosition);
            float fingerDistance = currentDistance - previousDistance;

            ChangeCameraDistance(fingerDistance);
        }
    }

    private void ChangeCameraDistance(float fingerDistance) 
    {
        float cameraPosY_offset = (fingerDistance * _offset) * -1;

        if (cameraPosY_offset + _cameraMainTransform.position.y > _cameraPosY_Min &&
            cameraPosY_offset + _cameraMainTransform.position.y < _cameraPosY_Max)
        {
            _cameraMainTransform.position += new Vector3(0, cameraPosY_offset, 0);
        }
        else 
        {
            cameraPosY_offset = _cameraPosY_Default;
            _cameraMainTransform.position = new Vector3(0, cameraPosY_offset, 0);
        }
    }
}
