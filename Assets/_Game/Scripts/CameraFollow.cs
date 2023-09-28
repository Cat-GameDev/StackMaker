using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target; 
    [SerializeField] private float smoothSpeed = 0.125f; 
    [SerializeField] private Vector3 offset; 
    //[SerializeField] private GameObject anim;
    private void Awake() 
    {
        LoadTargetTest();
    }
    private void LateUpdate()
    {
        if (target == null)
            return;

        PlayerFollow();
    }

    private void LoadTargetTest()
    {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        this.SetTarget(target.transform);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    

    private void PlayerFollow()
    {
        if(Player.instance.IsLifted)
        {
            offset.y += 0.3f;
            Player.instance.IsLifted = false;
        } 
        else if(Player.instance.IsDowned)
        {
            offset.y += -0.3f;
            Player.instance.IsDowned = false;
        }
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(target); 
    }
}