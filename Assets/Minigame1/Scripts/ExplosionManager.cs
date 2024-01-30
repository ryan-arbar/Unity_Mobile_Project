using UnityEngine;

// For purpose of testing explosion
public class ExplosionManager : MonoBehaviour
{
    public GameObject explosionPrefab;
    public float explosionRadius = 50f;
    public float explosionPower = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject explosion = Instantiate(explosionPrefab, hit.point, Quaternion.identity);
                explosion.GetComponent<Shape>().Explode(explosionRadius, explosionPower);
            }
        }
    }
}
