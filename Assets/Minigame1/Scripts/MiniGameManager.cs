using System.Collections.Generic;
using UnityEngine;

public class MiniGameManager : MonoBehaviour
{
    public List<MiniGameBase> miniGamesPrefabs;
    private MiniGameBase currentMiniGame;

    void Start()
    {
        LoadNextMiniGame();
    }

    void LoadNextMiniGame()
    {
        if (currentMiniGame != null)
        {
            currentMiniGame.OnComplete -= HandleMiniGameComplete; // Unsubscribe from the previous mini-game's event
            Destroy(currentMiniGame.gameObject); // Clean up the previous mini-game
        }

        int index = Random.Range(0, miniGamesPrefabs.Count);
        MiniGameBase miniGamePrefab = miniGamesPrefabs[index];
        currentMiniGame = Instantiate(miniGamePrefab); // Instantiate the next mini-game
        currentMiniGame.OnComplete += HandleMiniGameComplete; // Subscribe to the completion event
    }

    void HandleMiniGameComplete()
    {
        LoadNextMiniGame(); // Load the next mini-game when the current one completes
    }
}
