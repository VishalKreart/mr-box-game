using UnityEngine;

public class HorizontalMover : MonoBehaviour
{
    public float speed = 2f;
    public float leftLimit = -3f;
    public float rightLimit = 3f;

    private int direction = 1;
    private bool allowMovement = true;

    void LateUpdate()
    {
        //if (!allowMovement) return;

        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        if (transform.position.x >= rightLimit)
            direction = -1;
        else if (transform.position.x <= leftLimit)
            direction = 1;
    }

    public void SetMovement(bool value)
    {
        allowMovement = value;
    }
}
