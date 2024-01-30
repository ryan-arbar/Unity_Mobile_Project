using System.Collections;
using UnityEngine;

[System.Serializable]
public class ShapeTier
{
    public GameObject[] shapes; // Array of shape prefabs for this tier
    public int spawnProbability; // Probability of this tier being selected (out of 100)
}

public class ShapeSpawner : MonoBehaviour
{
    public ShapeTier[] tiers; // Array of tiers
    public float initialSpawnInterval = 2f; // Initial time interval between spawns
    public float minSpawnInterval = 0.5f; // Minimum time interval between spawns
    public float intervalDecreaseRate = 0.01f; // Rate at which spawn interval decreases
    public float shapeSpeed = 10f;

    private Camera cam;
    private float timer;
    private float currentSpawnInterval;

    void Start()
    {
        cam = Camera.main;
        currentSpawnInterval = initialSpawnInterval;
        timer = currentSpawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnShape();
            timer = currentSpawnInterval;
            DecreaseSpawnInterval();
        }
    }

    void SpawnShape()
    {
        ShapeTier selectedTier = SelectTier();
        GameObject shapeToSpawn = selectedTier.shapes[Random.Range(0, selectedTier.shapes.Length)];
        Vector3 spawnPosition = GetRandomPositionOutsideScreen();
        GameObject spawnedShape = Instantiate(shapeToSpawn, spawnPosition, Quaternion.identity);

        spawnedShape.layer = LayerMask.NameToLayer("Spawning");

        Rigidbody rb = spawnedShape.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            Vector3 targetPoint = GetRandomPointInsideScreen();
            Vector3 moveDirection = (targetPoint - spawnPosition).normalized;
            rb.velocity = moveDirection * shapeSpeed;

            StartCoroutine(ResetGravityAndLayer(spawnedShape, 1f));
        }
        else
        {
            Debug.LogError("Rigidbody not found on the spawned shape");
        }
    }

    IEnumerator ResetGravityAndLayer(GameObject shape, float delay)
    {
        yield return new WaitForSeconds(delay);

        shape.layer = LayerMask.NameToLayer("Gameplay");
        Rigidbody rb = shape.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }

    ShapeTier SelectTier()
    {
        int totalProbability = 0;
        foreach (var tier in tiers)
        {
            totalProbability += tier.spawnProbability;
        }

        int randomPoint = Random.Range(0, totalProbability);
        int currentSum = 0;
        foreach (var tier in tiers)
        {
            currentSum += tier.spawnProbability;
            if (randomPoint <= currentSum)
            {
                return tier;
            }
        }

        return tiers[0];
    }

    Vector3 GetRandomPointInsideScreen()
    {
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        return new Vector3(Random.Range(-camWidth / 2, camWidth / 2), Random.Range(-camHeight / 2, camHeight / 2), 0) + cam.transform.position;
    }

    Vector3 GetRandomPositionOutsideScreen()
    {
        float screenAspect = Screen.width / (float)Screen.height;
        float camHeight = cam.orthographicSize * 2;
        Vector2 camSize = new Vector2(camHeight * screenAspect, camHeight);
        Vector3 camPosition = cam.transform.position;

        int side = Random.Range(0, 4);
        Vector3 spawnPos = Vector3.zero;

        switch (side)
        {
            case 0:
                spawnPos = new Vector3(Random.Range(camPosition.x - (camSize.x / 2), camPosition.x + (camSize.x / 2)), camPosition.y + (camSize.y / 2) + 1, 0);
                break;
            case 1:
                spawnPos = new Vector3(Random.Range(camPosition.x - (camSize.x / 2), camPosition.x + (camSize.x / 2)), camPosition.y - (camSize.y / 2) - 1, 0);
                break;
            case 2:
                spawnPos = new Vector3(camPosition.x - (camSize.x / 2) - 1, Random.Range(camPosition.y - (camSize.y / 2), camPosition.y + (camSize.y / 2)), 0);
                break;
            case 3:
                spawnPos = new Vector3(camPosition.x + (camSize.x / 2) + 1, Random.Range(camPosition.y - (camSize.y / 2), camPosition.y + (camSize.y / 2)), 0);
                break;
        }

        return spawnPos;
    }

    void DecreaseSpawnInterval()
    {
        if (currentSpawnInterval > minSpawnInterval)
        {
            currentSpawnInterval -= intervalDecreaseRate * Time.deltaTime;
        }
    }
}
