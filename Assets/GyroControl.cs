using UnityEngine;

public class GyroControl : MonoBehaviour
{
    public GameObject wallTop, wallBottom, wallLeft, wallRight;
    public float gravityScale = 20.0f; // Increased gravity scale for faster response

    void Start()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
        }
        else
        {
            Debug.LogError("Gyroscope not supported on this device.");
        }

        SetUpWalls();
    }

    void Update()
    {
        if (SystemInfo.supportsGyroscope)
        {
            Vector3 targetGravity = Input.acceleration * gravityScale;
            Physics.gravity = targetGravity; // Apply gravity change
        }
    }

    void SetUpWalls()
    {
        Camera cam = Camera.main;
        float screenAspect = Screen.width / (float)Screen.height;
        float camHeight = cam.orthographicSize * 2;
        Vector2 camSize = new Vector2(camHeight * screenAspect, camHeight);
        Vector3 camPosition = cam.transform.position;

        // Position walls just outside the camera's view
        wallTop.transform.position = new Vector3(camPosition.x, camPosition.y + (camSize.y / 2) + (wallTop.transform.localScale.y / 2), 0);
        wallBottom.transform.position = new Vector3(camPosition.x, camPosition.y - (camSize.y / 2) - (wallBottom.transform.localScale.y / 2), 0);
        wallLeft.transform.position = new Vector3(camPosition.x - (camSize.x / 2) - (wallLeft.transform.localScale.x / 2), camPosition.y, 0);
        wallRight.transform.position = new Vector3(camPosition.x + (camSize.x / 2) + (wallRight.transform.localScale.x / 2), camPosition.y, 0);

        // Scale walls to cover the screen edges
        wallTop.transform.localScale = new Vector3(camSize.x + 1, 1, 1);
        wallBottom.transform.localScale = new Vector3(camSize.x + 1, 1, 1);
        wallLeft.transform.localScale = new Vector3(1, camSize.y + 1, 1);
        wallRight.transform.localScale = new Vector3(1, camSize.y + 1, 1);
    }
}
