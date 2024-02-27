using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour
{
    public delegate void StartTouchEvent(Vector2 position, float time);
    public delegate void EndTouchEvent(Vector2 position, float time);

    #region Events
    public event StartTouchEvent OnStartTouch;
    public event EndTouchEvent OnEndTouch;
    #endregion

    [Header("Objects/Scripts References")]
    [SerializeField] TouchControls _touchControls;

    void Awake()
    {
        _touchControls = new TouchControls();
    }

    void OnEnable()
    {
        _touchControls.Enable();
        //TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    void OnDisable()
    {
        _touchControls.Disable();
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        //TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        _touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        _touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
    }

    void Update()
    {
        Debug.Log(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches);

        foreach (UnityEngine.InputSystem.EnhancedTouch.Touch touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches) 
        {
            Debug.Log(touch.phase == UnityEngine.InputSystem.TouchPhase.Began);
        }
    }

    private void StartTouch(InputAction.CallbackContext context) 
    {
        //Debug.Log($"StartTouch_{_touchControls.Touch.TouchPosition.ReadValue<Vector2>()}");

        if (OnStartTouch != null) 
        {
            OnStartTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        //Debug.Log("EndTouch");

        if (OnEndTouch != null)
        {
            OnEndTouch(_touchControls.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
        }
    }

    private void FingerDown(Finger finger)
    {
        if (OnStartTouch != null)
        {
            OnStartTouch(finger.screenPosition, Time.time);
        }
    }
}
