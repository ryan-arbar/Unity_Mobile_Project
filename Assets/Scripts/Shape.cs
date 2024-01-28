using UnityEngine;

public class Shape : MonoBehaviour
{
    public int tier; // The current tier of the shape
    public GameObject nextTierPrefab; // Prefab of the next tier shape
    public AudioClip combineSound; // Combine sound effect

    private bool isCombining = false;

    private void OnCollisionEnter(Collision collision)
    {
        Shape otherShape = collision.gameObject.GetComponent<Shape>();

        if (otherShape != null && otherShape.tier == this.tier && !isCombining && !otherShape.isCombining)
        {
            isCombining = true;
            otherShape.isCombining = true;
            CombineShapes(otherShape);
        }
    }

    private void CombineShapes(Shape otherShape)
    {
        AudioManager.Instance.PlaySound(combineSound);

        if (tier == 6)
        {
            Explode();
        }
        else if (nextTierPrefab != null)
        {
            Instantiate(nextTierPrefab, transform.position, Quaternion.identity);
        }

        Destroy(otherShape.gameObject);
        Destroy(gameObject);
    }

    public void Explode(float radius = 100f, float power = 1000f)
    {
        Debug.Log("Explode called, Radius: " + radius + ", Power: " + power);

        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);

        foreach (Collider hit in colliders)
        {
            Debug.DrawRay(explosionPos, hit.transform.position - explosionPos, Color.red, 0f);

            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(power, explosionPos, radius, 0f);
                Debug.Log("Explosion force applied to " + hit.gameObject.name);
            }
        }

        Destroy(gameObject, 0.1f); // Delay before destroying the object (probably don't need)
    }
}
