using UnityEngine;

public class FallDetector : MonoBehaviour
{
    public GameManager gameManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Box"))
        {
            Destroy(other.gameObject);
            gameManager.GameOver();
        }
    }
}
