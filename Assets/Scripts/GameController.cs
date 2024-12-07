using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // The potion prefabs for different colors
    public GameObject[] potionPrefabs;  // Array for potion prefabs (each color has its own prefab)

    // The pipe prefabs array (3 pipes, one for each spawn point)
    public GameObject[] pipePrefabs;  // Array of pipe prefabs

    // The spawn points for the potions (these positions will remain unchanged)
    public Transform[] potionSpawnPoints;  // Spawn points for potions

    // The spawn points for the pipes (this is for the pipes only)
    public Transform[] pipeSpawnPoints;  // Separate spawn points for pipes

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
        { "Purple", new List<string[]>{ new string[]{ "Red", "Blue" } } },  // Purple
        { "Orange", new List<string[]>{ new string[]{ "Red", "Yellow" } } },  // Orange
        { "Green", new List<string[]>{ new string[]{ "Blue", "Yellow" } } },  // Green
    };

    // Other colors that are not part of the target/result combinations
    private List<string> otherColors = new List<string>
    {
        "Red", "Blue", "Yellow"
    };

    public List<string> smashedPotions = new List<string>();  // Track smashed potions
    public List<string> unsmashedPotions = new List<string>();  // Track unsmashed potions

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

        // Instantiate pipes to match the order of potion colors
        SpawnPipes();

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
            case "Purple":
                targetSprite = targetColorImages[0];  // First image in the array
                break;
            case "Orange":
                targetSprite = targetColorImages[1];  // Second image in the array
                break;
            case "Green":
                targetSprite = targetColorImages[2];  // Third image in the array
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

        // Get the correct potion combinations for the target color
        List<string> correctColors = GetCorrectPotionCombinations(targetColor);

        // Add the correct colors to the list before shuffling
        bool correctColorsPresent = false;

        // Repeat until both correct colors are in the list of 3 potions
        while (!correctColorsPresent)
        {
            // Shuffle the list of colors
            colorsToSpawn = colorsToSpawn.OrderBy(x => Random.value).ToList();

            // Ensure we have 3 unique colors by taking distinct ones
            colorsToSpawn = colorsToSpawn.Distinct().Take(3).ToList();

            // Check if both correct colors are in the shuffled list
            correctColorsPresent = correctColors.All(color => colorsToSpawn.Contains(color));

            if (!correctColorsPresent)
            {
                colorsToSpawn.Clear();
                colorsToSpawn.AddRange(otherColors);
            }
        }

        // Now spawn the potions at the spawn points (ensure one potion per position)
        for (int i = 0; i < potionSpawnPoints.Length; i++)
        {
            string color = colorsToSpawn[i];

            // Add the color to the list of remaining potions
            potionsRemaining.Add(color);

            // Instantiate the corresponding potion prefab at the correct spawn point
            GameObject potionPrefab = GetPotionPrefabByColor(color);
            GameObject potionInstance = Instantiate(potionPrefab, potionSpawnPoints[i].position, Quaternion.identity);

            // Set the potion color in the potion behavior
            PotionBehavior potionBehavior = potionInstance.GetComponent<PotionBehavior>();
            if (potionBehavior != null)
            {
                potionBehavior.potionColor = color; // Set the color of the potion
                potionBehavior.gameController = this; // Assign the game controller to the potion behavior
            }
        }
    }

    // Spawn pipes based on the order of potions
    private void SpawnPipes()
    {
        for (int i = 0; i < pipeSpawnPoints.Length; i++)
        {
            // Ensure we have matching pipes and potions
            if (i < potionsRemaining.Count && i < pipePrefabs.Length)
            {
                string potionColor = potionsRemaining[i];

                // Get the corresponding pipe prefab based on potion color
                GameObject pipePrefab = GetPipePrefabByColor(potionColor);

                if (pipePrefab != null)
                {
                    Instantiate(pipePrefab, pipeSpawnPoints[i].position, Quaternion.identity);
                }
            }
        }
    }

    // Get the appropriate potion prefab based on the color
    private GameObject GetPotionPrefabByColor(string color)
    {
        switch (color)
        {
            case "Red": return potionPrefabs[0];  // Red
            case "Blue": return potionPrefabs[1];  // Blue
            case "Yellow": return potionPrefabs[2];  // Yellow
            default: return null; // Default case, should never hit
        }
    }

    // Get the appropriate pipe prefab based on the potion color
    private GameObject GetPipePrefabByColor(string color)
    {
        switch (color)
        {
            case "Red": return pipePrefabs[0];  // Pipe for Red
            case "Blue": return pipePrefabs[1];  // Pipe for Blue
            case "Yellow": return pipePrefabs[2];  // Pipe for Yellow
            default: return null; // Default case, should never hit
        }
    }

    // Method to be called when a potion is smashed
    public void SmashPotion(string potionColor)
    {
        if (unsmashedPotions.Contains(potionColor))
        {
            unsmashedPotions.Remove(potionColor);
            smashedPotions.Add(potionColor);
        }
    }

    // Method to calculate and display the success rate when the game ends
    private void EndGame()
    {
        List<string> correctColors = GetCorrectPotionCombinations(targetColor);
        int correctLeft = unsmashedPotions.Count(potion => correctColors.Contains(potion));
        List<string> incorrectColors = otherColors.Except(correctColors).ToList();
        int incorrectLeft = unsmashedPotions.Count(potion => incorrectColors.Contains(potion));
        int totalRemaining = correctLeft + incorrectLeft;

        if (correctLeft == 2 && incorrectLeft == 0)
        {
            resultText.text = "Success Rate: 100%! You got it! Go to the next round!";
        }
        else if (correctLeft == 1 && incorrectLeft == 0)
        {
            resultText.text = "Success Rate: 50%! You're half close :o try again!";
        }
        else if (correctLeft == 0 || (correctLeft == 2 && incorrectLeft == 1))
        {
            resultText.text = "Success Rate: 0%! C'mon whack a potion, Try Again!";
        }
        else
        {
            int successRate = (correctLeft * 100) / totalRemaining;
            resultText.text = $"Success Rate: {successRate}%!";

        }
    }

    // Get the correct potions for the target color (color combinations that should not be hit)
    private List<string> GetCorrectPotionCombinations(string targetColor)
    {
        switch (targetColor)
        {
            case "Purple":
                return new List<string> { "Red", "Blue" };
            case "Orange":
                return new List<string> { "Red", "Yellow" };
            case "Green":
                return new List<string> { "Blue", "Yellow" };
            default:
                return new List<string>();
        }
    }
}
