using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Touch : MonoBehaviour
{
    private string _tagTofind = TypesOfTags.InputManager.ToString();

    #region Scripts/Objects Reference
    [Header("Scripts/Objects Reference")]
    [SerializeField] Test_InputManager _inputManager_Ref;
    [SerializeField] Camera _cameraMain_Ref;
    #endregion

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

    #region Breakup
    private void SetReferences() 
    {
        _inputManager_Ref = GameObject.FindWithTag(_tagTofind).GetComponent<Test_InputManager>();
        _cameraMain_Ref = Camera.main;
    }

    private void SubscriveEvents() 
    {
        _inputManager_Ref.OnStartTouch += Move;
    }

    private void UnsubscriveEvents()
    {
        _inputManager_Ref.OnStartTouch -= Move;
    }
    #endregion

    private void Move(Vector2 screenPosition, float time)
    {
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, _cameraMain_Ref.nearClipPlane);
        Vector3 worldCoordinates = _cameraMain_Ref.ScreenToWorldPoint(screenCoordinates);
        worldCoordinates.y = 0;
        transform.position = worldCoordinates;

        Debug.Log($"testTouch()_{transform.position}");//
    }
}
