using System.Collections;
using UnityEngine;

public class PotionBehavior : MonoBehaviour
{
    private bool isFading = false;
    
    // Add a field to store the potion color
    public string potionColor;

    // Reference to GameController to call SmashPotion
    public GameController gameController;

    private void OnMouseOver()
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndDestroy());
        }
    }

    private IEnumerator FadeAndDestroy()
    {
        isFading = true;
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = sprite.color;

        // Gradually fade out
        for (float t = 0; t < 1f; t += Time.deltaTime)
        {
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        // Call SmashPotion when the potion fades and is about to be destroyed
        if (gameController != null && !string.IsNullOrEmpty(potionColor))
        {
            gameController.SmashPotion(potionColor); // Pass the potion color
        }

        // Destroy the object after fading out
        Destroy(gameObject);
    }
}
