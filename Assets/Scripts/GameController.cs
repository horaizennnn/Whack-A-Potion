using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // The potion prefabs for different colors
    public GameObject[] potionPrefabs;  // Array for potion prefabs (each color has its own prefab)
    
    // The spawn points for the potions
    public Transform[] spawnPoints;  // Spawn points for potions
    
    // The UI text for the target color and result
    public TMP_Text targetColorText;
    public TMP_Text resultText;

    // Add a reference to the Image component for displaying the target color
    public Image targetColorImage;  // The UI Image where the target color will be shown

    // Add a list of images to represent each target color
    public Sprite[] targetColorImages;  // Array to hold images for each target color

    // Timer for the game
    public float gameTime = 20f;
    public TMP_Text gameText;

    // List to store the remaining potions that the player did not hit
    private List<string> potionsRemaining = new List<string>();
    
    // Dictionary of target colors and their corresponding potion combinations (colors that should NOT be hit)
    private Dictionary<string, List<string[]>> colorCombinations = new Dictionary<string, List<string[]>>()
    {
        { "Fresh Eggplant", new List<string[]>{ new string[]{ "Bright Red", "Bright Blue" } } },  // Fresh Eggplant
        { "Chartreuse", new List<string[]>{ new string[]{ "Bright Green", "Yellow" } } },  // Chartreuse
        { "Medium Red Violet", new List<string[]>{ new string[]{ "Hot Pink", "Dark Purple" } } },  // Medium Red Violet
        { "Sycamore", new List<string[]>{ new string[]{ "Orange", "Teal" } } },  // Sycamore
        { "Pastel Green", new List<string[]>{ new string[]{ "Dark Turquoise", "Gold" } } },  // Pastel Green
        { "Cannon Pink", new List<string[]>{ new string[]{ "Steel Blue", "Crimson Red" } } },  // Cannon Pink
    };

    // Other colors that are not part of the target/result combinations
    private List<string> otherColors = new List<string>
    {
        "Bright Red", "Bright Blue", "Bright Green", "Yellow", "Hot Pink", "Dark Purple",
        "Orange", "Teal", "Dark Turquoise", "Gold", "Steel Blue", "Crimson Red"
    };

    private List<string> smashedPotions = new List<string>();  // Track smashed potions
    private List<string> unsmashedPotions = new List<string>();  // Track unsmashed potions

    private string targetColor;

    private void Start()
    {
        // Randomly choose a target color at the start of the game
        targetColor = ChooseRandomTargetColor();

        // Display the target color name (not the hex code)
        targetColorText.text = $"Target Color: {targetColor}";

        // Set the target color image (based on the target color)
        SetTargetColorImage(targetColor);

        // Spawn potions at specific positions (one potion per position)
        SpawnPotions();

        // Initialize unsmashed potions at the start of the game
        unsmashedPotions = new List<string>(potionsRemaining);
    }

    // Method to set the target color image based on the selected color
    private void SetTargetColorImage(string color)
    {
        Sprite targetSprite = null;

        // Check the color and assign the appropriate sprite from the array
        switch (color)
        {
            case "Fresh Eggplant":
                targetSprite = targetColorImages[0];  // First image in the array
                break;
            case "Chartreuse":
                targetSprite = targetColorImages[1];  // Second image in the array
                break;
            case "Medium Red Violet":
                targetSprite = targetColorImages[2];  // Third image in the array
                break;
            case "Sycamore":
                targetSprite = targetColorImages[3];  // Fourth image in the array
                break;
            case "Pastel Green":
                targetSprite = targetColorImages[4];  // Fifth image in the array
                break;
            case "Cannon Pink":
                targetSprite = targetColorImages[5];  // Sixth image in the array
                break;
            default:
                Debug.LogWarning($"No sprite found for {color}");
                break;
        }

        // If a sprite is found, set it to the target color image
        if (targetSprite != null)
        {
            targetColorImage.sprite = targetSprite;
        }
    }



    private void Update()
    {
        // Decrease the game timer and update the timer text
        gameTime -= Time.deltaTime;

        if (gameTime <= 0)
        {
            gameTime = 0;
            EndGame();
        }

        gameText.text = Mathf.CeilToInt(gameTime).ToString();
    }

    // Choose a random target color from the list of target colors
    private string ChooseRandomTargetColor()
    {
        List<string> targetColors = new List<string>(colorCombinations.Keys);
        return targetColors[Random.Range(0, targetColors.Count)];
    }

    // Spawn potions at specific spawn points (one potion color per position)
    public void SpawnPotions()
    {
        // Create a list of all available colors to spawn (including the target color's components)
        List<string> colorsToSpawn = new List<string>(otherColors);
        Debug.Log($"other colors: {string.Join(", ", colorsToSpawn)}");

        // Get the correct potion combinations for the target color
        List<string> correctColors = GetCorrectPotionCombinations(targetColor);
        
        // Add the correct colors to the list before shuffling
        //colorsToSpawn.AddRange(correctColors);
        
        // Ensure that the correct colors are in the list before shuffling
        bool correctColorsPresent = false;
        
        // Repeat until both correct colors are in the list of 9 potions
        while (!correctColorsPresent)
        {
            // Shuffle the list of colors
            colorsToSpawn = colorsToSpawn.OrderBy(x => Random.value).ToList();
            
            // Ensure we have 9 unique colors by taking distinct ones
            colorsToSpawn = colorsToSpawn.Distinct().Take(9).ToList();
            
            // Check if both correct colors are in the shuffled list
            correctColorsPresent = correctColors.All(color => colorsToSpawn.Contains(color));
            Debug.Log($"Shuffled and Distinct Colors: {string.Join(", ", colorsToSpawn)}");
            Debug.Log($"correctColors: {string.Join(", ", correctColors)}");

            // Log the check result
            if (correctColorsPresent)
            {
                Debug.Log("Both correct colors are present in the list.");
            }
            else
            {
                Debug.Log("One or both correct colors are missing, reshuffling...");
            }

            // If both correct colors are not found, repeat the process
            if (!correctColorsPresent)
            {
                // Reset the list and add the correct colors again
                colorsToSpawn.Clear();
                colorsToSpawn.AddRange(otherColors);
                //colorsToSpawn.AddRange(correctColors);
                //ChooseRandomTargetColor();
            }
        }

        // Now spawn the potions at the spawn points (ensure one potion per position)
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            // Pick a color from the shuffled list of available unique colors
            string color = colorsToSpawn[i];

            // Add the color to the list of remaining potions
            potionsRemaining.Add(color);

            // Instantiate the corresponding potion prefab at the correct spawn point
            GameObject potionPrefab = GetPotionPrefabByColor(color);
            GameObject potionInstance = Instantiate(potionPrefab, spawnPoints[i].position, Quaternion.identity);

            // Set the potion color in the potion behavior
            PotionBehavior potionBehavior = potionInstance.GetComponent<PotionBehavior>();
            if (potionBehavior != null)
            {
                potionBehavior.potionColor = color; // Set the color of the potion
                potionBehavior.gameController = this; // Assign the game controller to the potion behavior
            }
        }
}



    // Get the appropriate potion prefab based on the color
    private GameObject GetPotionPrefabByColor(string color)
    {
        switch (color)
        {
            case "Bright Red": return potionPrefabs[0];  // Bright Red
            case "Bright Blue": return potionPrefabs[1];  // Bright Blue
            case "Bright Green": return potionPrefabs[2];  // Bright Green
            case "Yellow": return potionPrefabs[3];  // Yellow
            case "Hot Pink": return potionPrefabs[4];  // Hot Pink
            case "Dark Purple": return potionPrefabs[5];  // Dark Purple
            case "Orange": return potionPrefabs[6];  // Orange
            case "Teal": return potionPrefabs[7];  // Teal
            case "Dark Turquoise": return potionPrefabs[8];  // Dark Turquoise
            case "Gold": return potionPrefabs[9];  // Gold
            case "Steel Blue": return potionPrefabs[10]; // Steel Blue
            case "Crimson Red": return potionPrefabs[11]; // Crimson Red
            default: return null; // Default case, should never hit
        }
    }

        // Method to be called when a potion is smashed
    public void SmashPotion(string potionColor)
    {
        // Check if the potion is in the unsmashed list
        if (unsmashedPotions.Contains(potionColor))
        {
            // Remove from unsmashed and add to smashed
            unsmashedPotions.Remove(potionColor);
            smashedPotions.Add(potionColor);
        }
    }

    
// Method to calculate and display the success rate when the game ends
    private void EndGame()
{
    //Debug.Log($"EndGame called. Target Color: {targetColor}");

    // Get the correct colors that are part of the target color combination
    List<string> correctColors = GetCorrectPotionCombinations(targetColor);
    //Debug.Log($"Correct Colors for Target ({targetColor}): {string.Join(", ", correctColors)}");

    // Count how many of the correct potions are left (i.e., still in the unsmashed potions list)
    int correctLeft = unsmashedPotions.Count(potion => correctColors.Contains(potion));
    //Debug.Log($"Correct Potions Remaining (unsmashed): {correctLeft}");

    // Track which colors are considered "incorrect" potions
    List<string> incorrectColors = otherColors.Except(correctColors).ToList();
    //Debug.Log($"Incorrect Colors (should be avoided): {string.Join(", ", incorrectColors)}");

    // Count how many of the incorrect potions are left (i.e., still in the unsmashed potions list)
    int incorrectLeft = unsmashedPotions.Count(potion => incorrectColors.Contains(potion));
    //Debug.Log($"Incorrect Potions Remaining (unsmashed): {incorrectLeft}");

    // Calculate total remaining potions
    int totalRemaining = correctLeft + incorrectLeft;
    //Debug.Log($"Total Potions Remaining (unsmashed): {totalRemaining}");

    // Ensure the totalRemaining is 9 or less
    if (totalRemaining != 9)
    {
        //Debug.LogError($"Error: Total remaining potions count is not 9. Actual count: {totalRemaining}");
    }

    // Calculate the success rate: if there are no correct potions left, the score is 0%
    if (correctLeft == 2 && incorrectLeft == 0)
    {
        resultText.text = "Success Rate: 100%! You got it! Go to the next round!";
        //Debug.Log("Success Rate: 100%! You got it! Go to the next round!");
    } else if (correctLeft == 1 && incorrectLeft == 0)
    {
        resultText.text = "Success Rate: 50%! You're half close :o try again!";
        //Debug.Log("Success Rate: 50%! You're half close :o try again!");
    } else if (correctLeft == 0 || (correctLeft == 2 && incorrectLeft == 7))
    {
        resultText.text = "Success Rate: 0%! Don't give up, Try Again!";
        //Debug.Log("Success Rate: 0%! Don't give up, Try Again!");
    } else 
    {
        // Calculate success rate as the percentage of correct potions out of total remaining potions
        int successRate = (correctLeft * 100) / totalRemaining;

        if (successRate > 50)
        {
            resultText.text = $"Success Rate: {successRate}%!";
            //Debug.Log($"Success Rate Calculated: {successRate}%");
        } else {
            resultText.text = $"Success Rate: {successRate}%! Better Luck Next Time!";
            //Debug.Log($"Success Rate Calculated: {successRate}%");
        }
    }

    // Debug the smashed potions
    //Debug.Log($"Smashed Potions: {string.Join(", ", smashedPotions)}");
    //Debug.Log($"Unsmashed Potions: {string.Join(", ", unsmashedPotions)}");
}



    // Get the correct potions for each target color combination
    private List<string> GetCorrectPotionCombinations(string targetColor)
    {
        switch (targetColor)
        {
            case "Fresh Eggplant":
                return new List<string> { "Bright Red", "Bright Blue" };
            case "Chartreuse":
                return new List<string> { "Bright Green", "Yellow" };
            case "Medium Red Violet":
                return new List<string> { "Hot Pink", "Dark Purple" };
            case "Sycamore":
                return new List<string> { "Orange", "Teal" };
            case "Pastel Green":
                return new List<string> { "Dark Turquoise", "Gold" };
            case "Cannon Pink":
                return new List<string> { "Steel Blue", "Crimson Red" };
            default:
                return new List<string>();
        }
    }
}
