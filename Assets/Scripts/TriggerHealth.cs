using UnityEngine;

public class TriggerAnimationOnHit : Health
{
    public GameObject targetObject; // The GameObject whose animation will play
    string triggerName = "PlayAnimation"; // The trigger in the Animator to activate

    private Animator targetAnimator; // Animator on the target GameObject

    void Start()
    {
        // Ensure the targetObject has an Animator
        if (targetObject != null)
        {
            targetAnimator = targetObject.GetComponent<Animator>();
            if (targetAnimator == null)
            {
                Debug.LogError("No Animator component found on the target object.");
            }
        }
        else
        {
            Debug.LogError("Target object is not assigned.");
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage); // Call the base class's TakeDamage to reduce health

        if (health <= 0 && targetAnimator != null)
        {
            // Trigger the animation on the target object
            targetAnimator.SetTrigger(triggerName);
        }
    }
}
