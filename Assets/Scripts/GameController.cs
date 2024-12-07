using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // The potion prefabs for different colors
    public GameObject[] potionPrefabs;
    public GameObject[] pipePrefabs;
    public Transform[] potionSpawnPoints;
    public Transform[] pipeSpawnPoints;

    // New: Light prefabs and spawn points
    public GameObject[] lightPrefabs;  // Light prefabs
    public Transform[] lightSpawnPoints;  // Light spawn points

    // UI elements
    public TMP_Text targetColorText;
    public TMP_Text resultText;
    public Image targetColorImage;
    public Sprite[] targetColorImages;

    // Timer
    public float gameTime = 20f;
    public TMP_Text gameText;

    private List<string> potionsRemaining = new List<string>();
    private Dictionary<string, List<string[]>> colorCombinations = new Dictionary<string, List<string[]>>
    {
        { "Purple", new List<string[]> { new string[] { "Red", "Blue" } } },
        { "Orange", new List<string[]> { new string[] { "Red", "Yellow" } } },
        { "Green", new List<string[]> { new string[] { "Blue", "Yellow" } } }
    };
    private List<string> otherColors = new List<string> { "Red", "Blue", "Yellow" };

    public List<string> smashedPotions = new List<string>();
    public List<string> unsmashedPotions = new List<string>();

    private string targetColor;

    private void Start()
    {
        targetColor = ChooseRandomTargetColor();
        targetColorText.text = $"Target Color: {targetColor}";
        SetTargetColorImage(targetColor);

        SpawnPotions();
        SpawnPipes();
        SpawnLights();

        unsmashedPotions = new List<string>(potionsRemaining);
    }

    private void Update()
    {
        gameTime -= Time.deltaTime;
        if (gameTime <= 0)
        {
            gameTime = 0;
            EndGame();
        }

        gameText.text = Mathf.CeilToInt(gameTime).ToString();
    }

    private string ChooseRandomTargetColor()
    {
        List<string> targetColors = new List<string>(colorCombinations.Keys);
        return targetColors[Random.Range(0, targetColors.Count)];
    }

    private void SetTargetColorImage(string color)
    {
        Sprite targetSprite = color switch
        {
            "Purple" => targetColorImages[0],
            "Orange" => targetColorImages[1],
            "Green" => targetColorImages[2],
            _ => null
        };

        if (targetSprite != null)
            targetColorImage.sprite = targetSprite;
        else
            Debug.LogWarning($"No sprite found for {color}");
    }

    private void SpawnPotions()
    {
        List<string> colorsToSpawn = new List<string>(otherColors);
        List<string> correctColors = GetCorrectPotionCombinations(targetColor);

        while (!correctColors.All(color => colorsToSpawn.Contains(color)))
        {
            colorsToSpawn = colorsToSpawn.OrderBy(x => Random.value).Distinct().Take(3).ToList();
            if (!correctColors.All(color => colorsToSpawn.Contains(color)))
                colorsToSpawn.Clear();
        }

        for (int i = 0; i < potionSpawnPoints.Length; i++)
        {
            string color = colorsToSpawn[i];
            potionsRemaining.Add(color);

            GameObject potionPrefab = GetPotionPrefabByColor(color);
            GameObject potionInstance = Instantiate(potionPrefab, potionSpawnPoints[i].position, Quaternion.identity);

            if (potionInstance.TryGetComponent(out PotionBehavior potionBehavior))
            {
                potionBehavior.potionColor = color;
                potionBehavior.gameController = this;
            }
        }
    }

    private void SpawnPipes()
    {
        for (int i = 0; i < pipeSpawnPoints.Length; i++)
        {
            if (i < potionsRemaining.Count && i < pipePrefabs.Length)
            {
                string potionColor = potionsRemaining[i];
                GameObject pipePrefab = GetPipePrefabByColor(potionColor);

                if (pipePrefab != null)
                    Instantiate(pipePrefab, pipeSpawnPoints[i].position, Quaternion.identity);
            }
        }
    }

    private void SpawnLights()
    {
        for (int i = 0; i < lightSpawnPoints.Length; i++)
        {
            if (i < lightPrefabs.Length)
            {
                Instantiate(lightPrefabs[i], lightSpawnPoints[i].position, Quaternion.identity);
            }
        }
    }

    private GameObject GetPotionPrefabByColor(string color) => color switch
    {
        "Red" => potionPrefabs[0],
        "Blue" => potionPrefabs[1],
        "Yellow" => potionPrefabs[2],
        _ => null
    };

    private GameObject GetPipePrefabByColor(string color) => color switch
    {
        "Red" => pipePrefabs[0],
        "Blue" => pipePrefabs[1],
        "Yellow" => pipePrefabs[2],
        _ => null
    };

    public void SmashPotion(string potionColor)
    {
        if (unsmashedPotions.Contains(potionColor))
        {
            unsmashedPotions.Remove(potionColor);
            smashedPotions.Add(potionColor);
        }
    }

    private void EndGame()
    {
        List<string> correctColors = GetCorrectPotionCombinations(targetColor);
        int correctLeft = unsmashedPotions.Count(potion => correctColors.Contains(potion));
        int incorrectLeft = unsmashedPotions.Count(potion => otherColors.Except(correctColors).Contains(potion));

        if (correctLeft == 2 && incorrectLeft == 0)
            resultText.text = "Success Rate: 100%! You got it! Go to the next round!";
        else if (correctLeft == 1 && incorrectLeft == 0)
            resultText.text = "Success Rate: 50%! You're half close :o try again!";
        else if (correctLeft == 0 || (correctLeft == 2 && incorrectLeft == 1))
            resultText.text = "Success Rate: 0%! C'mon whack a potion, Try Again!";
        else
        {
            int successRate = (correctLeft * 100) / (correctLeft + incorrectLeft);
            resultText.text = $"Success Rate: {successRate}%!";
        }
    }

    private List<string> GetCorrectPotionCombinations(string targetColor) => targetColor switch
    {
        "Purple" => new List<string> { "Red", "Blue" },
        "Orange" => new List<string> { "Red", "Yellow" },
        "Green" => new List<string> { "Blue", "Yellow" },
        _ => new List<string>()
    };
}
