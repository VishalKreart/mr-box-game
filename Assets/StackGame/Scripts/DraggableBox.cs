using UnityEngine;

public class DraggableBox : MonoBehaviour
{
    private bool isDragging = false;
    private bool dragStarted = false;
    private Vector3 offset;
    private Camera mainCamera;
    private BoxState boxState;
    private Rigidbody2D rb;
    public float dragThreshold = 0.1f; // Minimum movement to count as drag
    private Vector3 dragStartPos;

    void Start()
    {
        mainCamera = Camera.main;
        boxState = GetComponent<BoxState>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnMouseDown()
    {
        if (boxState != null && boxState.state == BoxState.State.Spawned)
        {
            isDragging = true;
            dragStarted = false;
            dragStartPos = GetMouseWorldPos();
            offset = transform.position - dragStartPos;
        }
    }

    void OnMouseDrag()
    {
        if (isDragging && boxState != null && boxState.state == BoxState.State.Spawned)
        {
            Vector3 mousePos = GetMouseWorldPos();
            if (!dragStarted && Vector3.Distance(mousePos, dragStartPos) > dragThreshold)
            {
                dragStarted = true;
            }
            if (dragStarted)
            {
                Vector3 newPos = mousePos + offset;
                transform.position = new Vector3(newPos.x, transform.position.y, transform.position.z);
            }
        }
    }

    void OnMouseUp()
    {
        if (isDragging && boxState != null && boxState.state == BoxState.State.Spawned)
        {
            isDragging = false;
            if (rb != null) rb.gravityScale = 1f;
            boxState.state = BoxState.State.Falling;
        }
    }

    Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Mathf.Abs(mainCamera.transform.position.z);
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    // Touch support for mobile
    void Update()
    {
        if (boxState == null || boxState.state != BoxState.State.Spawned)
            return;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, Mathf.Abs(mainCamera.transform.position.z)));
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (IsTouchingThisBox(touchWorldPos))
                    {
                        isDragging = true;
                        dragStarted = false;
                        dragStartPos = touchWorldPos;
                        offset = transform.position - touchWorldPos;
                    }
                    break;
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    if (isDragging)
                    {
                        if (!dragStarted && Vector3.Distance(touchWorldPos, dragStartPos) > dragThreshold)
                        {
                            dragStarted = true;
                        }
                        if (dragStarted)
                        {
                            transform.position = new Vector3(touchWorldPos.x + offset.x, transform.position.y, transform.position.z);
                        }
                    }
                    break;
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (isDragging)
                    {
                        isDragging = false;
                        if (rb != null) rb.gravityScale = 1f;
                        boxState.state = BoxState.State.Falling;
                    }
                    break;
            }
        }
    }

    bool IsTouchingThisBox(Vector3 worldPos)
    {
        Collider2D col = GetComponent<Collider2D>();
        return col != null && col.OverlapPoint(new Vector2(worldPos.x, worldPos.y));
    }
} 