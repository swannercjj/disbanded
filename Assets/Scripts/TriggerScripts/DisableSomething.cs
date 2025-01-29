using UnityEngine;

public class DisableSomething : MonoBehaviour
{

    public GameObject something;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        something.SetActive(false);
        Destroy(this);
    }
}
