using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Pinch : MonoBehaviour
{
    [SerializeField] float _cameraSpeed = 5f;//
    private Coroutine _zoomCoroutine;//
    private string _tagTofind = TypesOfTags.InputManager.ToString();

    [Header("Objects/Scripts References")]
    [SerializeField] Test_InputManager _inputManager_Ref;
    //[SerializeField] TouchControls _touchControls;
    private Transform _cameraTransform;//

    void Awake()
    {
        SetReferences();
    }

    void OnEnable()
    {
        SubscriveEvents();
    }

    void OnDisable()
    {
        UnsubscriveEvents();
    }

    private void SetReferences() 
    {
        _inputManager_Ref = GameObject.FindWithTag(_tagTofind).GetComponent<Test_InputManager>();
        //_touchControls = new TouchControls();
        _cameraTransform = Camera.main.transform;//
    }


    private void SubscriveEvents()
    {
        //_inputManager_Ref.OnZoomStart += ZoomStart;
        //_inputManager_Ref.OnZoomEnd += ZoomEnd;
    }

    private void UnsubscriveEvents()
    {
        //_inputManager_Ref.OnZoomStart -= ZoomStart;
        //_inputManager_Ref.OnZoomEnd -= ZoomEnd;
    }

    private void ZoomStart(Vector2 positionPrimaryFinger, Vector2 positionSecondaryFInger, float time)
    {
        _zoomCoroutine = StartCoroutine(ZoomDetection(positionPrimaryFinger, positionSecondaryFInger, time));
        Debug.Log("ZoomStart");//
    }

    private void ZoomEnd(Vector2 positionPrimaryFinger, Vector2 positionSecondaryFInger, float time)
    {
        StopCoroutine(ZoomDetection(positionPrimaryFinger, positionSecondaryFInger, time));
        Debug.Log("ZoomEnd");//
    }

    IEnumerator ZoomDetection(Vector2 positionPrimaryFinger, Vector2 positionSecondaryFInger, float time)
    {
        float previousDistance = 0f;
        float currentDistance = 0f;

        while (true)
        {
            currentDistance = Vector2.Distance(/*_touchControls.Touch.PrimaryFingerPosition.ReadValue<Vector2>()*/positionPrimaryFinger,
                                               /*_touchControls.Touch.SecondaryFingerPosition.ReadValue<Vector2>()*/positionSecondaryFInger);

            if (currentDistance > previousDistance)
            {
                Vector3 targetPosition = _cameraTransform.position;
                targetPosition.y -= 1;
                _cameraTransform.position = Vector3.Slerp(_cameraTransform.position,
                                                          targetPosition,
                                                          Time.deltaTime * _cameraSpeed);

                Debug.Log("ZoomDetection(IN)");//
            }
            else if (currentDistance < previousDistance)
            {
                Vector3 targetPosition = _cameraTransform.position;
                targetPosition.y += 1;
                _cameraTransform.position = Vector3.Slerp(_cameraTransform.position,
                                                          targetPosition,
                                                          Time.deltaTime * _cameraSpeed);
                Debug.Log("ZoomDetection(Out)");//
            }

            previousDistance = currentDistance;

            yield return null;

            //Debug.Log("ZoomDetection()");//
        }
    }
}
