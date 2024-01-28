using System.Collections;
using UnityEngine;

public abstract class MiniGameBase : MonoBehaviour
{
    public float timeLimit = 5.0f; // Default time limit
    public delegate void MiniGameEvent();
    public event MiniGameEvent OnComplete; // Event to signal the completion of a mini-game

    protected virtual void Start()
    {
        StartCoroutine(GameTimer());
    }

    protected virtual void CompleteGame()
    {
        OnComplete?.Invoke(); // Invoke the completion event
    }

    IEnumerator GameTimer()
    {
        yield return new WaitForSeconds(timeLimit);
        CompleteGame();
    }

    protected abstract void GameLogic();
}
