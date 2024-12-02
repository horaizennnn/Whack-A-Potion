using System.Collections;
using UnityEngine;

public class PotionBehavior : MonoBehaviour
{
    private bool isFading = false;
    
    // Add a field to store the potion color
    public string potionColor;

    // Reference to GameController to call SmashPotion
    public GameController gameController;

    public void DestroyInstantly()
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndDestroyInstantly());
        }
    }

    public IEnumerator FadeAndDestroyInstantly()
    {
        isFading = true;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        // Set the alpha to 0 immediately
        Color currentColor = sprite.color;
        sprite.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0f);

        // Call SmashPotion instantly
        if (gameController != null && !string.IsNullOrEmpty(potionColor))
        {
            gameController.SmashPotion(potionColor); // Pass the potion color
        }

        // Destroy the object immediately after fading
        Destroy(gameObject);
        yield return null;
    }
}
