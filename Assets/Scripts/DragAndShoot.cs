using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndShoot : MonoBehaviour
{
    [SerializeField] InputAction _press = null;
    [SerializeField] InputAction _screenPosition = null;

    private Camera _cameraMain = null;
    private Vector3 _currentScreenPosition = Vector3.zero;
    private Rigidbody _rigidBody = null;
    private LineRenderer _lineRenderer = null;
    private bool _isDragging = false;
    private Vector3 _dragStartPos = Vector3.zero;
    private const float _power = 3f;
    private const float _maxDrag = 5f;
    private Vector3 _lineRendererDraggingPosition_Offset = new Vector3(0, 0.5f, 0);

    private Vector3 _worldPosition
    {
        get 
        {
            float z = _cameraMain.WorldToScreenPoint(transform.position).z;
            return _cameraMain.ScreenToWorldPoint(_currentScreenPosition + new Vector3(0, 0, z));
        }
    }

    private bool _isTransformHasChanged
    {
        get
        {
            if (transform.hasChanged == false)
            {
                transform.hasChanged = false;
                return false;
            }
            else
            {
                transform.hasChanged = false;
                return true;
            }
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
                if (hit.collider.gameObject == gameObject)
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }
        }
    }

    void Awake()
    {
        SetReference();
    }

    void OnEnable()
    {
        _press.Enable();
        _screenPosition.Enable();
        SubscribeEvents();
    }

    void OnDisable()
    {
        UnsubscribeEvents();
        _press.Disable();
        _screenPosition.Disable();
    }

    private void SetReference() 
    {
        _cameraMain = Camera.main;
        _rigidBody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    private void SubscribeEvents() 
    {
        _screenPosition.performed += context => { _currentScreenPosition = context.ReadValue<Vector2>(); };

        _press.started += _ =>
        {
            if (_isTransformHasChanged == false &&
                _isClickedOn == true)
            {
                StartCoroutine(UpdateDraggingPosition());
            }
        };

        _press.canceled += _ => Initialize_isDragging();
    }

    private void UnsubscribeEvents() 
    {
        _screenPosition.performed -= context => { _currentScreenPosition = context.ReadValue<Vector2>(); };
        _press.started -= _ => { StartCoroutine(UpdateDraggingPosition()); };
        _press.canceled -= _ => Initialize_isDragging();
    }

    private void Initialize_isDragging() 
    {
        _isDragging = false;
    }

    private IEnumerator UpdateDraggingPosition()
    {
        SetDragStartPosition();

        _isDragging = true;

        while (_isDragging)
        {
            Dragging();
            yield return null;
        }

        ShootBall();
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
        _lineRenderer.SetPosition(1, draggingPosition + _lineRendererDraggingPosition_Offset);
    }

    private void ShootBall()
    {
        _lineRenderer.positionCount = 0;

        Vector3 draggingPosition = ReturnDraggingPosition();
        Vector3 force = _dragStartPos - draggingPosition;
        Vector3 clampedForce = Vector3.ClampMagnitude(force, _maxDrag) * _power;
        _rigidBody.AddForce(clampedForce, ForceMode.Impulse);
    }
}
