using System.Collections;
using UnityEngine;

public class OnTrigger2dScript : MonoBehaviour
{
    [SerializeField]
    private GameObject explosion;
    public GameController gameController;
    public Sprite[] hammerSprites; // Array to hold the hammer sprites (1-stand, 2-slight bend, 3-right bend, 4-right bend with smash effect)
    private SpriteRenderer hammerRenderer;
    private bool isTriggering = false;

    void Start()
    {
        // Get the SpriteRenderer component from the hammer object (you can set this reference in the Inspector too)
        hammerRenderer = GetComponent<SpriteRenderer>(); 
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the PotionBehavior component from the collided object
        PotionBehavior potionBehavior = collision.GetComponent<PotionBehavior>();

        if (potionBehavior != null)
        {
            potionBehavior.DestroyInstantly(); // Call instant destroy method
            Instantiate(explosion, transform.position, Quaternion.identity); // Instantiate explosion effect
            Debug.Log("Potion triggered");

            // Start the hammer animation to simulate the smashing effect (1 -> 4)
            isTriggering = true;
            StartCoroutine(AnimateHammer(true));
        }
    }

    void OnTriggerExit2D()
    {
        // When exiting the trigger, stop triggering and animate the hammer back (4 -> 1)
        Debug.Log("Trigger exited");

        isTriggering = false;
        StartCoroutine(AnimateHammer(false));
    }

    // Coroutine to animate the hammer
    private IEnumerator AnimateHammer(bool isSmashing)
    {
        if (isSmashing)
        {
            // Animate from sprite 1 to 4
            for (int i = 0; i < hammerSprites.Length; i++)
            {
                hammerRenderer.sprite = hammerSprites[i];
                yield return new WaitForSeconds(0.05f); // Adjust time between frames
            }
        }
        else
        {
            // Animate from sprite 4 to 1
            for (int i = hammerSprites.Length - 1; i >= 0; i--)
            {
                hammerRenderer.sprite = hammerSprites[i];
                yield return new WaitForSeconds(0.05f); // Adjust time between frames
            }
        }
    }
}
