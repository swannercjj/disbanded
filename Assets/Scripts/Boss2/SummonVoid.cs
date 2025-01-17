using UnityEngine;

public class SummonVoid : MonoBehaviour
{
    public GameObject voidPrefab;    // The prefab of the void to summon
    public Transform firepoint;     // The point from where the void is summoned

    // This method is called to summon a void at the firepoint
    public void Initiate()
    {
        if (voidPrefab != null && firepoint != null)
        {
            // Instantiate the void prefab at the firepoint's position and rotation
            Instantiate(voidPrefab, firepoint.position, firepoint.rotation);
        }
        else
        {
            Debug.LogWarning("Void Prefab or Firepoint is not assigned.");
        }
    }
}
