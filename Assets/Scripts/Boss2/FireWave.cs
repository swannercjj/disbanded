using UnityEngine;

public class FireWave : MonoBehaviour
{
    public GameObject prefab; // Prefab to spawn
    public Transform spawnPoint; // The point from which to spawn the prefabs
    public int numberOfPrefabs = 10; // Total number of prefabs to spawn
    public float spacing = 2f; // Distance between each spawned prefab
    public float height = 100f; // Height above the spawn point where the prefabs should be placed

    public void Initiate()
    {
        SpawnPrefabs();
    }

    void SpawnPrefabs()
    {
        // Calculate the offset to ensure the prefabs are centered
        float halfSpacing = (numberOfPrefabs - 1) * spacing / 2f;

        // Loop to spawn each prefab in a line (symmetrically placed)
        for (int i = 0; i < numberOfPrefabs; i++)
        {
            // Calculate the position for each prefab, symmetrically around the spawn point
            float offset = i * spacing - halfSpacing;
            Vector3 spawnPosition = spawnPoint.position + new Vector3(offset, height, 0f);

            // Instantiate the prefab at the calculated position
            GameObject prefabInstance = Instantiate(prefab, spawnPosition, Quaternion.identity);

            // Get the EnemyLaserLerp script from the instantiated prefab
            EnemyLaserLerp laserScript = prefabInstance.GetComponent<EnemyLaserLerp>();

            if (laserScript != null)
            {
                Vector3 obj_vec = gameObject.transform.position;
                // Call the Initial function with the offset-adjusted target position
                Vector3 target_pos = new Vector3(spawnPosition.x - 2f * obj_vec.x, spawnPosition.y, spawnPosition.z - 2f * obj_vec.z);

                laserScript.Initial(target_pos);
            }
            else
            {
                Debug.LogError("EnemyLaserLerp script is not attached to the prefab.");
            }
        }
    }
}
