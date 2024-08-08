using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
    public Transform target; // The target to look at

    [SerializeField] private bool useXAxis = true;
    [SerializeField] private bool useYAxis = true;
    [SerializeField] private bool useZAxis = true;

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;

            if (!useXAxis) direction.x = 0;
            if (!useYAxis) direction.y = 0;
            if (!useZAxis) direction.z = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation;
            }
        }
    }
}
