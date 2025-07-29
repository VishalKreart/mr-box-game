using UnityEngine;
using UnityEngine.EventSystems;

public class DragAnywhereToMoveBox : MonoBehaviour
{
    public BoxSpawner boxSpawner; // Assign in inspector
    public float dragThreshold = 0.2f; // Minimum drag (in world units) to count as drag

    private GameObject currentBox;
    private BoxState boxState;
    private Rigidbody2D rb;
    private bool isDragging = false;
    private Vector3 dragStartScreenPos;
    private float boxStartX;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (boxSpawner != null) boxSpawner.externalDropControl = true;
    }

    void Update()
    {
        // Get the current spawned box
        currentBox = boxSpawner != null ? boxSpawner.GetCurrentBox() : null;
        if (currentBox == null) return;
        boxState = currentBox.GetComponent<BoxState>();
        rb = currentBox.GetComponent<Rigidbody2D>();
        if (boxState == null || rb == null || boxState.state != BoxState.State.Spawned)
            return;

#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput();
#else
        HandleTouchInput();
#endif
    }

    // Check if pointer is over UI element
    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;
        
#if UNITY_EDITOR || UNITY_STANDALONE
        return EventSystem.current.IsPointerOverGameObject();
#else
        if (Input.touchCount > 0)
        {
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }
        return false;
#endif
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Don't start dragging if clicking on UI
            if (IsPointerOverUI()) return;
            
            isDragging = true;
            dragStartScreenPos = Input.mousePosition;
            boxStartX = currentBox.transform.position.x;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentScreenPos = Input.mousePosition;
            float dragDeltaWorld = mainCamera.ScreenToWorldPoint(new Vector3(currentScreenPos.x, 0, Mathf.Abs(mainCamera.transform.position.z))).x -
                                   mainCamera.ScreenToWorldPoint(new Vector3(dragStartScreenPos.x, 0, Mathf.Abs(mainCamera.transform.position.z))).x;
            currentBox.transform.position = new Vector3(boxStartX + dragDeltaWorld, currentBox.transform.position.y, currentBox.transform.position.z);
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            Vector3 currentScreenPos = Input.mousePosition;
            float dragDeltaWorld = Mathf.Abs(mainCamera.ScreenToWorldPoint(new Vector3(currentScreenPos.x, 0, Mathf.Abs(mainCamera.transform.position.z))).x -
                                         mainCamera.ScreenToWorldPoint(new Vector3(dragStartScreenPos.x, 0, Mathf.Abs(mainCamera.transform.position.z))).x);
            DropBox();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0) return;
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
            // Don't start dragging if touching UI
            if (IsPointerOverUI()) return;
            
            isDragging = true;
            dragStartScreenPos = touch.position;
            boxStartX = currentBox.transform.position.x;
        }
        else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging)
        {
            Vector3 currentScreenPos = touch.position;
            float dragDeltaWorld = mainCamera.ScreenToWorldPoint(new Vector3(currentScreenPos.x, 0, Mathf.Abs(mainCamera.transform.position.z))).x -
                                   mainCamera.ScreenToWorldPoint(new Vector3(dragStartScreenPos.x, 0, Mathf.Abs(mainCamera.transform.position.z))).x;
            currentBox.transform.position = new Vector3(boxStartX + dragDeltaWorld, currentBox.transform.position.y, currentBox.transform.position.z);
        }
        else if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && isDragging)
        {
            isDragging = false;
            DropBox();
        }
    }

    void DropBox()
    {
        if (boxSpawner != null) boxSpawner.ExternalDropBox();
    }
} 