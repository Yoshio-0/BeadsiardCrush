using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndShoot : MonoBehaviour
{
    [SerializeField] InputAction _press = null;
    [SerializeField] InputAction _screenPosition = null;

    [SerializeField] Vector3 _currentScreenPosition = Vector3.zero;
    [SerializeField] bool _isDragging = false;

    [SerializeField] Camera _cameraMain = null;
    //private bool _isGravityUsed = false;

    [SerializeField] float _power = 3f;
    [SerializeField] float _maxDrag = 5f;
    [SerializeField] Rigidbody _rigidBody;
    [SerializeField] LineRenderer _lineRenderer;
    [SerializeField] Vector3 _dragStartPos;

    private Vector3 _worldPosition 
    {
        get 
        {
            float z = _cameraMain.WorldToScreenPoint(transform.position).z;
            return _cameraMain.ScreenToWorldPoint(_currentScreenPosition + new Vector3(0, 0, z));
        }
    }

    private bool _isClickedOn
    {
        get
        {
            Ray ray = _cameraMain.ScreenPointToRay(_currentScreenPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                return hit.transform == transform;
            }
            else 
            {
                return false;
            }
        }
    }

    private bool _isTransformChanged 
    {
        get 
        {
            if (transform.hasChanged == false)
            {
                Debug.Log(transform.hasChanged);
                transform.hasChanged = false;
                return false;
            }
            else 
            {
                Debug.Log(transform.hasChanged);
                transform.hasChanged = false;
                return true;
            }
        }
    }



    void Awake()
    {
        _cameraMain = Camera.main;
        //_isGravityUsed = GetComponent<Rigidbody>().useGravity;
        _rigidBody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        _press.Enable();
        _screenPosition.Enable();

        _screenPosition.performed += context => { _currentScreenPosition = context.ReadValue<Vector2>(); };

        _press.performed += _ => 
        {
            if (_isClickedOn && 
                _isTransformChanged == false) 
            {
                StartCoroutine(UpdateDraggingPosition());
            }
        };

        _press.canceled += _ => { _isDragging = false; };
    }

    void OnDisable()
    {
        _screenPosition.performed -= context => { _currentScreenPosition = context.ReadValue<Vector2>(); };
        _press.performed -= _ => { StartCoroutine(UpdateDraggingPosition()); };
        _press.canceled -= _ => { _isDragging = false; };

        _press.Disable();
        _screenPosition.Disable();
    }

    private IEnumerator UpdateDraggingPosition()
    {
        SetDragStartPosition();//

        _isDragging = true;

        while (_isDragging)
        {
            Dragging();
            yield return null;
        }

        ShootBall();//
    }

    private void SetDragStartPosition()
    {
        _dragStartPos = transform.position;

        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, _dragStartPos);
    }

    private Vector3 ReturnDraggingPosition()
    {
        Vector3 draggingPosition = new Vector3(_worldPosition.x, transform.position.y, _worldPosition.z);
        return draggingPosition;
    }

    private void Dragging()
    {
        Vector3 draggingPosition = ReturnDraggingPosition();

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(1, draggingPosition);
    }

    private void ShootBall()
    {
        _lineRenderer.positionCount = 0;

        Vector3 draggingPosition = ReturnDraggingPosition();
        Vector3 force = _dragStartPos - draggingPosition;
        Vector3 clampedForce = Vector3.ClampMagnitude(force, _maxDrag) * _power;
        _rigidBody.AddForce(clampedForce, ForceMode.Impulse);
    }

    /*
    private IEnumerator DragAndReplaceBall()
    {
        _isGravityUsed = false;
        _isDragging = true;

        Vector3 offset = transform.position - _worldPosition;

        while (_isDragging)
        {
            transform.position = _worldPosition + offset;
            yield return null;
        }

        _isGravityUsed = true;
    }
    */
}
