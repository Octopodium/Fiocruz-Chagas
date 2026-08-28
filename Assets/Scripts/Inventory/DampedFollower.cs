using UnityEngine;

public class DampedFollower : MonoBehaviour
{
    public bool following = true;
    [SerializeField] private Transform target;
    [SerializeField, Range(0.0f, 3.0f)] private float smoothTime;
    private Vector3 currentVelocity = Vector3.zero;


    /// <summary>
    /// Sets the target for the follower to follow.
    /// </summary>
    /// <param name="newTarget"></param>
    public void SetTarget(Transform newTarget){
        target = newTarget;
    }

    private void Update(){
        if(following){
            DampedMovement();
        }
    }

    /// <summary>
    /// Moves the object towards the target position using a damped movement. The movement is smoothed over time based on the smoothTime value.
    /// </summary>
    private void DampedMovement(){
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref currentVelocity, smoothTime);
    }
}
