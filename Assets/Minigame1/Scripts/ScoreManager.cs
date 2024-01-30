using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;
    public TextMeshProUGUI scoreText;
    public int score;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddScoreByTier(int tier)
    {
        int pointsToAdd = CalculatePoints(tier);
        score += pointsToAdd;
        UpdateScoreUI();
    }

    private int CalculatePoints(int tier)
    {
        return tier * 10;
    }

    private void UpdateScoreUI()
    {
        scoreText.text = score.ToString();
    }
}
