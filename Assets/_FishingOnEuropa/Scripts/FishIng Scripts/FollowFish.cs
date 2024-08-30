using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowFish : MonoBehaviour
{
    
    public float speed = 5f; // The speed at which the object moves
    bool follow = false;

    private Rigidbody rb;

    [SerializeField] GameObject caughtFish;
    Transform fish;
    [SerializeField] FishNavigationScript fishNavScipt;
    [SerializeField] ClosestObjectsFinder closestObjectsFinder;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    void Update()
    {
        if (fish != null)
        {
            if (follow == true)
            {
                MoveTowardsTarget();
            }

        }
    }

    void MoveTowardsTarget()
    {
        // Calculate the direction to the target
        Vector3 direction = (fish.position - transform.position).normalized;
        direction.y = 0;
        // Apply force to move towards the target
        rb.AddForce(direction * speed);
    }

    public void UpdateLureFollowFish(bool doFollow)
    {
        follow = doFollow;
        //caughtFish = closestObjectsFinder.caughtFish; //REMOVED SINCE ClosestObjectsFinder IS BEING REMOVED
        fish = caughtFish.transform;
    }
}
