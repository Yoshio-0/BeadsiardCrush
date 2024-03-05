using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_DragAndDrop : MonoBehaviour
{
    [SerializeField] InputAction _mouseClick;//
    [SerializeField] float _mouseDragPhysicsSpeed = 10;
    [SerializeField] float _mouseDragSpeed = 0.1f;
    private Vector3 _velocity = Vector3.zero;
    private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();

    //private string _tagTofind = TypesOfTags.InputManager.ToString();//

    #region Scripts/Objects Reference
    [Header("Scripts/Objects Reference")]
    //[SerializeField] InputManager _inputManager_Ref;//
    [SerializeField] Camera _cameraMain;
    #endregion

    void Awake()
    {
        _cameraMain = Camera.main;    
    }

    void OnEnable()
    {
        _mouseClick.Enable();
        _mouseClick.performed += MousePressed;
    }

    void OnDisable()
    {
        _mouseClick.performed -= MousePressed;
        _mouseClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context) 
    {
        Ray ray = _cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue());//
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) 
        {
            if (hit.collider != null) 
            {
                StartCoroutine(DragUpdate(hit.collider.gameObject));
            }
        }
    }

    private IEnumerator DragUpdate(GameObject clickedObject) 
    {
        float initialDistance = Vector3.Distance(clickedObject.transform.position, _cameraMain.transform.position);
        clickedObject.TryGetComponent<Rigidbody>(out var rb);
        while (_mouseClick.ReadValue<float>() != 0) 
        {
            Ray ray = _cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (rb != null)
            {
                Vector3 direction = ray.GetPoint(initialDistance) - clickedObject.transform.position;
                rb.velocity = direction * _mouseDragPhysicsSpeed;
                yield return _waitForFixedUpdate;
            }
            else 
            {
                clickedObject.transform.position = Vector3.SmoothDamp(
                    clickedObject.transform.position, ray.GetPoint(initialDistance), ref _velocity, _mouseDragSpeed);
                yield return null;
            }
        }
    }
}
