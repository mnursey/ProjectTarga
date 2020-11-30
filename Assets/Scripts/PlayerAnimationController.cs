using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    public Animator anim;
    public CharacterController controller;
    public float velocityAnimationScaleFactor = 0.4f;

    void Awake()
    {
        if(anim == null) anim = GetComponent<Animator>();
        if(controller == null) controller = GetComponent<CharacterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(controller.velocity);
        anim.SetFloat("VelocityX", localVelocity.x * velocityAnimationScaleFactor);
        anim.SetFloat("VelocityZ", localVelocity.z * velocityAnimationScaleFactor);
    }
}
