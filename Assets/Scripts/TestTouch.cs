using UnityEngine;
using UnityEngine.InputSystem;

public class TestTouch : MonoBehaviour
{
    private string _tagTofind = TypesOfTags.InputManager.ToString();

    #region Scripts/Objects Reference
    [Header("Scripts/Objects Reference")]
    [SerializeField] InputManager _inputManager_Ref;
    [SerializeField] Camera _cameraMain;
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
        _inputManager_Ref = GameObject.FindWithTag(_tagTofind).GetComponent<InputManager>();
        _cameraMain = Camera.main;
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
        Vector3 screenCoordinates = new Vector3(screenPosition.x, screenPosition.y, _cameraMain.nearClipPlane);
        Vector3 worldCoordinates = _cameraMain.ScreenToWorldPoint(screenCoordinates);
        worldCoordinates.y = 0;

        transform.position = worldCoordinates;
    }
}
