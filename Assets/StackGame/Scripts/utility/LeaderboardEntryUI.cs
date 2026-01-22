using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntryUI : MonoBehaviour
{
    [Header("UI")]
    public Image bgImage;

    [Header("Background Sprites")]
    public Sprite rank1Sprite;
    public Sprite rank2Sprite;
    public Sprite rank3Sprite;
    public Sprite normalSprite;
    public Sprite currentUserSprite;

    public void Setup(int rank, bool isCurrentUser)
    {
        if (isCurrentUser)
        {
            bgImage.sprite = currentUserSprite;
            return;
        }

        switch (rank)
        {
            case 1:
                bgImage.sprite = rank1Sprite;
                break;
            case 2:
                bgImage.sprite = rank2Sprite;
                break;
            case 3:
                bgImage.sprite = rank3Sprite;
                break;
            default:
                bgImage.sprite = normalSprite;
                break;
        }
    }
}
