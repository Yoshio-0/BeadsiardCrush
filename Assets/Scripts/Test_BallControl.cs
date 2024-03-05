using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_BallControl : MonoBehaviour
{
    [SerializeField] float _power = 1f;
    [SerializeField] float _maxDrag = 5f;
    [SerializeField] Rigidbody _rigidBody;
    [SerializeField] LineRenderer _lineRenderer;

    [SerializeField] Camera _cameraMain;
    [SerializeField] Vector3 _dragStartPos;
    [SerializeField] Vector3 _touchPosVec3;
    private Touch _touch;

    void Awake()
    {
        _cameraMain = Camera.main;
        _rigidBody = GetComponent<Rigidbody>();
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.touchCount > 0) 
        {
            _touch = Input.GetTouch(0);

            if (_touch.phase == TouchPhase.Began) 
            {
                DragStart();
            }

            if (_touch.phase == TouchPhase.Moved)
            {
                Dragging();
            }

            if (_touch.phase == TouchPhase.Ended)
            {
                DragRelease();
            }
        }    
    }

    private void DragStart() 
    {
        Ray ray = _cameraMain.ScreenPointToRay(_touch.position);//
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.GetComponent<Test_BallControl>())
            {
                _dragStartPos = transform.position;
                Debug.Log(_dragStartPos);//test
                _lineRenderer.positionCount = 1;
                _lineRenderer.SetPosition(0, _dragStartPos);
            }
        }

        //_touchPosVec3 = new Vector3(_touch.position.x, transform.position.y, _touch.position.y);//test
        //_dragStartPos = Camera.main.ScreenToWorldPoint(_touchPosVec3/*_touch.position*/);
        //Debug.Log(_dragStartPos);//test
        //_dragStartPos.z = 0f;
        //_lineRenderer.positionCount = 1;
        //_lineRenderer.SetPosition(0, _dragStartPos);
    }

    private void Dragging()
    {
        Ray ray = _cameraMain.ScreenPointToRay(_touch.position);//
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "TableBed")
            {
                _touchPosVec3 = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.y);//test
                Vector3 draggingPos = Camera.main.ScreenToWorldPoint(/*_touch.position*/_touchPosVec3);
                Debug.Log(draggingPos);//test
                _lineRenderer.positionCount = 2;
                _lineRenderer.SetPosition(1, draggingPos);
            }
        }

        //_touchPosVec3 = new Vector3(_touch.position.x, transform.position.y, _touch.position.y);//test
        //Vector3 draggingPos = Camera.main.ScreenToWorldPoint(_touch.position);
        //draggingPos.z = 0f;
        //_lineRenderer.positionCount = 2;
        //_lineRenderer.SetPosition(1, draggingPos);
    }

    private void DragRelease()
    {
        _lineRenderer.positionCount = 0;

        Ray ray = _cameraMain.ScreenPointToRay(_touch.position);//
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag == "TableBed")
            {
                _touchPosVec3 = new Vector3(hit.transform.position.x, transform.position.y, hit.transform.position.y);//test
                Vector3 dragReleasePos = Camera.main.ScreenToWorldPoint(/*_touch.position*/_touchPosVec3);
                Debug.Log(dragReleasePos);
                Vector3 force = _dragStartPos - dragReleasePos;
                Vector3 clampedForce = Vector3.ClampMagnitude(force, _maxDrag) * _power;
                _rigidBody.AddForce(clampedForce, ForceMode.Impulse/*ForceMode2D.Impulse*/);
            }
        }


        //_touchPosVec3 = new Vector3(_touch.position.x, transform.position.y, _touch.position.y);//test
        //Vector3 dragReleasePos = Camera.main.ScreenToWorldPoint(_touchPosVec3/*_touch.position*/);
        //dragReleasePos.z = 0f;

        //Vector3 force = _dragStartPos - dragReleasePos;
        //Vector3 clampedForce = Vector3.ClampMagnitude(force, _maxDrag) * _power;

        //_rigidBody.AddForce(clampedForce, ForceMode.Impulse/*ForceMode2D.Impulse*/);
    }

}
