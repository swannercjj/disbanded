using UnityEngine;
using System.Collections.Generic; // Include this to use List

public class DisableDistance : MonoBehaviour
{
    public float disableDistance = 10f; // Distance at which to disable the objects
    public List<GameObject> objectsToManage; // List of objects to enable/disable

    // Update is called once per frame
    // void Update()
    // {
    //     // Check the distance to the player
    //     float distance = Vector3.Distance(transform.position, PlayerController.player.transform.position);

    //     // Loop through all objects in the list
    //     foreach (GameObject obj in objectsToManage)
    //     {
    //         if (obj != null) // Ensure the object isn't null
    //         {
    //             // If the distance is greater than the specified threshold, disable the object
    //             if (distance > disableDistance)
    //             {
    //                 obj.SetActive(false);
    //             }
    //             else
    //             {
    //                 obj.SetActive(true);
    //             }
    //         }
    //     }
    // }

    public void SetAwakeness(bool active) {
        foreach (GameObject obj in objectsToManage)
        {
            if (obj != null) // Ensure the object isn't null
            {
                obj.SetActive(active);
            }
        }
    }
}
