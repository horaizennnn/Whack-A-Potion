using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // Added to use LINQ methods like Except

public class ContainerAnimation : MonoBehaviour
{
    private Animator mAnimator;
    private GameController gameController;
    private bool hasAnimationTriggered = false; // To ensure animation triggers only once

    void Start()
    {
        // Get the Animator component attached to this GameObject
        mAnimator = GetComponent<Animator>();

        // Find the first GameController in the scene
        gameController = Object.FindFirstObjectByType<GameController>();

        if (gameController == null)
        {
            Debug.LogError("GameController not found in the scene!");
        }
    }

    void Update()
    {
        if (gameController != null && mAnimator != null)
        {
            // Check if the game timer is 0 and animation has not been triggered
            if (gameController.gameTime <= 0 && !hasAnimationTriggered)
            {
                hasAnimationTriggered = true; // Mark animation as triggered
                StartCoroutine(TriggerAnimationWithCondition()); // Start animation with condition
            }
        }
    }

    private IEnumerator TriggerAnimationWithCondition()
    {
        yield return new WaitForSeconds(6f); // Wait for 6 seconds

        // Check if the potion combination is correct
        if (IsCorrectCombination())
        {
            // Trigger the animation based on the target color if correct
            Debug.Log("Correct combination. Triggering target color animation.");
            TriggerAnimationBasedOnTargetColor(gameController.targetColor);
        }
        else
        {
            // Trigger the machine error animation if combination is incorrect
            Debug.Log("Incorrect combination. Triggering machine error animation.");
            TriggerMachineErrorAnimation();
        }
    }

    private bool IsCorrectCombination()
    {
        // Get the correct color combination for the target color
        List<string> correctColors = gameController.GetCorrectPotionCombinations(gameController.targetColor);

        // Log the correct and smashed potions for debugging
        Debug.Log("Correct Colors: " + string.Join(", ", correctColors));
        Debug.Log("Smashed Potions: " + string.Join(", ", gameController.unsmashedPotions));

        // Check if the smashed potions contain all the correct colors and no extra colors
        bool isCorrect = gameController.unsmashedPotions.Count == correctColors.Count &&
                         !correctColors.Except(gameController.unsmashedPotions).Any();

        // Debug log the result of the combination check
        Debug.Log("Is correct combination: " + isCorrect);

        return isCorrect;
    }

    private void TriggerAnimationBasedOnTargetColor(string targetColor)
    {
        // Trigger the corresponding animation
        switch (targetColor.ToLower())
        {
            case "purple":
                mAnimator.SetTrigger("TrPurple");
                Debug.Log("Triggered Purple animation.");
                break;

            case "orange":
                mAnimator.SetTrigger("TrOrange");
                Debug.Log("Triggered Orange animation.");
                break;

            case "green":
                mAnimator.SetTrigger("TrGreen");
                Debug.Log("Triggered Green animation.");
                break;

            default:
                Debug.LogWarning("Unknown target color: " + targetColor);
                break;
        }
    }

    private void TriggerMachineErrorAnimation()
    {
        if (mAnimator != null)
        {
            mAnimator.SetTrigger("TrMachineError");
            Debug.Log("Triggered Machine Error animation.");
        }
        else
        {
            Debug.LogWarning("Animator not found on this GameObject!");
        }
    }
}
