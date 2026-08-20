using UnityEngine;

public class DampedFollower : MonoBehaviour
{
    public bool following = true;
    [SerializeField] private Transform target;
    [SerializeField, Range(0.0f, 3.0f)] private float smoothTime;
    private Vector3 currentVelocity = Vector3.zero;


    public void SetTarget(Transform newTarget){
        target = newTarget;
    }

    private void Update(){
        if(following){
            DampedMovement();
        }
    }

    private void DampedMovement(){
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref currentVelocity, smoothTime);
    }
}
