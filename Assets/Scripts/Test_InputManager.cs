using System;
using System.Collections;//
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

[DefaultExecutionOrder(-1)]
public class Test_InputManager : MonoBehaviour
{
    public delegate void StartTouchEvent(Vector2 position, float time);
    public delegate void EndTouchEvent(Vector2 position, float time);

    //public delegate void ZoomStartEvent(Vector2 positionPrimaryFinger, Vector2 positionSecondaryFInger, float time);//test
    //public delegate void ZoomEndEvent(Vector2 positionPrimaryFinger, Vector2 positionSecondaryFInger, float time);//test

    [SerializeField] float _cameraSpeed = 5f;//
    private Coroutine _zoomCoroutine;//
    private Transform _cameraTransform;//

    #region Events
    public event StartTouchEvent OnStartTouch;
    public event EndTouchEvent OnEndTouch;

    //public event ZoomStartEvent OnZoomStart;//test
    //public event ZoomEndEvent OnZoomEnd;//test
    #endregion

    [Header("Objects/Scripts References")]
    [SerializeField] TouchControls _touchControls_Ref;

    void Awake()
    {
        _touchControls_Ref = new TouchControls();
        _cameraTransform = Camera.main.transform;//
    }

    void OnEnable()
    {
        _touchControls_Ref.Enable();
        TouchSimulation.Enable();
        //EnhancedTouchSupport.Enable();
        //UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    void OnDisable()
    {
        _touchControls_Ref.Disable();
        //UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
        TouchSimulation.Disable();
        //EnhancedTouchSupport.Disable();
    }

    void Start()
    {
        _touchControls_Ref.Touch.TouchPress.started += context => StartTouch(context);
        _touchControls_Ref.Touch.TouchPress.canceled += context => EndTouch(context);

        //_touchControls_Ref.Touch.SecondaryFingerPosition.started += _ => ZoomStart();//
        //_touchControls_Ref.Touch.SecondaryFingerPosition.canceled += _ => ZoomEnd();//
        //_touchControls.Touch.SecondaryTouchContact.started += context => ZoomStart(context);//test
        //_touchControls.Touch.SecondaryTouchContact.canceled += context => ZoomEnd(context);//test
    }

    /*
    void Update()
    {
        //Debug.Log(UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches);

        foreach (UnityEngine.InputSystem.EnhancedTouch.Touch touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches) 
        {
            //Debug.Log(touch.phase == UnityEngine.InputSystem.TouchPhase.Began);
        }
    }
    */

    private void StartTouch(InputAction.CallbackContext context) 
    {
        //Debug.Log($"StartTouch_{_touchControls.Touch.TouchPosition.ReadValue<Vector2>()}");

        if (OnStartTouch != null) 
        {
            OnStartTouch(_touchControls_Ref.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.startTime);
        }
    }

    private void EndTouch(InputAction.CallbackContext context)
    {
        //Debug.Log("EndTouch");

        if (OnEndTouch != null)
        {
            OnEndTouch(_touchControls_Ref.Touch.TouchPosition.ReadValue<Vector2>(), (float)context.time);
        }
    }

    /*
    private void ZoomStart(InputAction.CallbackContext context) 
    {
        if (OnZoomStart != null) 
        {
            OnZoomStart(_touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(), 
                        _touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>(), 
                        (float)context.startTime);
        }
    }

    private void ZoomEnd(InputAction.CallbackContext context)
    {
        if (OnZoomEnd != null)
        {
            OnZoomEnd(_touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>(),
                      _touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>(), 
                      (float)context.time);
        }
    }
    */

    /*
    private void FingerDown(Finger finger)
    {
        if (OnStartTouch != null)
        {
            OnStartTouch(finger.screenPosition, Time.time);
        }
    }
    */

    //
    private void ZoomStart() 
    {
        _zoomCoroutine = StartCoroutine(ZoomDetection()) ;
    }

    //
    private void ZoomEnd() 
    {
        StopCoroutine(ZoomDetection());
    }

    //
    IEnumerator ZoomDetection() 
    {
        float previousDistance = 0f;
        float currentDistance = 0f;

        while (true) 
        {
            currentDistance = Vector2.Distance(_touchControls_Ref.Touch.PrimaryFingerPosition.ReadValue<Vector2>(), 
                _touchControls_Ref.Touch.SecondaryFingerPosition.ReadValue<Vector2>());

            if (currentDistance > previousDistance)
            {
                Vector3 targetPosition = _cameraTransform.position;
                targetPosition.y -= 1;
                _cameraTransform.position = Vector3.Slerp(_cameraTransform.position, 
                                                          targetPosition, 
                                                          Time.deltaTime * _cameraSpeed);
            }
            else if (currentDistance < previousDistance)
            {
                Vector3 targetPosition = _cameraTransform.position;
                targetPosition.y += 1;
                _cameraTransform.position = Vector3.Slerp(_cameraTransform.position,
                                                          targetPosition,
                                                          Time.deltaTime * _cameraSpeed);
            }

            previousDistance = currentDistance;

            yield return null;

            Debug.Log("ZoomDetection()");//
        }
    }
}
