using UnityEngine;
using System.Collections;

public class CameraStackFollow : MonoBehaviour
{
        
    public Transform boxSpawner; // Assign in inspector
    public BoxSpawner boxSpawnerScript; // Assign in inspector
    public float cameraMoveSpeed = 2f; // Smooth movement speed
    public float margin = 10.2f; // Extra margin to ensure full box visibility
    public int boxesPerStep = 5; // Number of boxes to drop before moving camera/spawner
    public int boxesVisibleAfterStep = 2; // Number of last stacked boxes to keep fully visible after each move
    public float zoomOutDuration = 1f; // Duration of zoom out in seconds
    public float zoomOutExtraMargin = 1.5f; // Extra margin for zoom out to show base

    private float minCameraY; // The lowest Y position the camera can have (starting position)
    private float maxBoxHeight = 1f;
    private int stackedCountAtLastMove = 0;
    private bool initialized = false;
    private float targetCameraY;
    private bool isZoomingOut = false;

    void Start()
    {
        minCameraY = transform.position.y;
        targetCameraY = minCameraY;
        initialized = true;
        // Calculate the tallest box height from prefabs
        if (boxSpawnerScript != null && boxSpawnerScript.boxPrefabs != null && boxSpawnerScript.boxPrefabs.Length > 0)
        {
            foreach (GameObject prefab in boxSpawnerScript.boxPrefabs)
            {
                Collider2D col = prefab.GetComponent<Collider2D>();
                if (col != null && col.bounds.size.y > maxBoxHeight)
                {
                    maxBoxHeight = col.bounds.size.y;
                }
            }
        }
    }

    void LateUpdate()
    {
        if (!initialized || boxSpawner == null || isZoomingOut) return;

        // Find the highest stacked (sleep) box and count stacked boxes
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
        if (boxes.Length == 0) return;

        float highestBoxY = float.MinValue;
        int stackedCount = 0;
        foreach (GameObject box in boxes)
        {
            BoxState boxState = box.GetComponent<BoxState>();
            if (boxState != null && boxState.state == BoxState.State.Sleep)
            {
                float y = box.transform.position.y;
                if (y > highestBoxY)
                    highestBoxY = y;
                stackedCount++;
            }
        }
        if (highestBoxY == float.MinValue) return;


        // Always place the spawner at a fixed offset below the top of the camera view
        float cameraTopY = targetCameraY + Camera.main.orthographicSize;
        float spawnerOffset = maxBoxHeight / 2f + margin;
        float spawnerY = cameraTopY - spawnerOffset;

        boxSpawner.position = new Vector3(0, spawnerY, boxSpawner.position.z);

        // Stepwise camera/spawner movement after every boxesPerStep stacked boxes
        if (stackedCount - stackedCountAtLastMove >= boxesPerStep)
        {
            // Move camera so the last N stacked boxes are fully visible at the bottom
            float cameraBottomY = highestBoxY - (maxBoxHeight * (boxesVisibleAfterStep - 1)) - maxBoxHeight / 2f - margin;
            targetCameraY = cameraBottomY + Camera.main.orthographicSize;
            // Place the spawner at the top (with margin for full visibility)
            cameraTopY = targetCameraY + Camera.main.orthographicSize;
            spawnerY = cameraTopY - spawnerOffset;

            boxSpawner.position = new Vector3(0, spawnerY, boxSpawner.position.z);
            // Reset counter
            stackedCountAtLastMove = stackedCount;
        }

        // Smoothly move the camera toward the target Y every frame
        Vector3 camPos = transform.position;
        camPos.y = Mathf.Lerp(camPos.y, targetCameraY, Time.deltaTime * cameraMoveSpeed);
        transform.position = camPos;

        // Never move camera below starting Y
        if (transform.position.y < minCameraY)
        {
            transform.position = new Vector3(transform.position.x, minCameraY, transform.position.z);
        }
    }


    

    public void ZoomOutOnGameOver()
    {
        if (!isZoomingOut)
            StartCoroutine(ZoomOutToShowBaseCoroutine());
    }

    private IEnumerator ZoomOutToShowBaseCoroutine()
    {
        isZoomingOut = true;
        // Find the lowest Y (base) and highest Y (top box)
        float lowestY = float.MaxValue;
        float highestY = float.MinValue;
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
        foreach (GameObject box in boxes)
        {
            Collider2D col = box.GetComponent<Collider2D>();
            if (col != null)
            {
                float boxBottom = col.bounds.min.y;
                float boxTop = col.bounds.max.y;
                if (boxBottom < lowestY) lowestY = boxBottom;
                if (boxTop > highestY) highestY = boxTop;
            }
        }
        // Optionally, include the base platform if it has a collider and a tag (e.g., "Platform")
        GameObject basePlatform = GameObject.FindGameObjectWithTag("Platform");
        if (basePlatform != null)
        {
            Collider2D col = basePlatform.GetComponent<Collider2D>();
            if (col != null && col.bounds.min.y < lowestY)
                lowestY = col.bounds.min.y;
        }
        // Calculate required orthographic size
        float centerY = (lowestY + highestY) / 2f;
        float requiredHalfHeight = (highestY - lowestY) / 2f + zoomOutExtraMargin;
        float startSize = Camera.main.orthographicSize;
        float targetSize = Mathf.Max(startSize, requiredHalfHeight);
        float elapsed = 0f;
        Vector3 camPos = transform.position;
        camPos.y = centerY;
        while (elapsed < zoomOutDuration)
        {
            Camera.main.orthographicSize = Mathf.Lerp(startSize, targetSize, elapsed / zoomOutDuration);
            transform.position = Vector3.Lerp(transform.position, camPos, elapsed / zoomOutDuration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        

        Camera.main.orthographicSize = targetSize;
        transform.position = camPos;
        isZoomingOut = false;
    }
}