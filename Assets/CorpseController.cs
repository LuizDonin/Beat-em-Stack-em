using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Transform ragdollRoot;
    [SerializeField] private bool startRagdoll = false;

    public Rigidbody[] rigidBodies;
    private CharacterJoint[] joints;
    private Collider[] colliders;

    private void Awake()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        joints = GetComponentsInChildren<CharacterJoint>();
        colliders = GetComponentsInChildren<Collider>();

        if (startRagdoll)
        {
            EnableRagdoll();
        }
        else
        {
            EnableAnimator();
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) 
        {
            EnableRagdoll();
        }
    }

    public void EnableRagdoll()
    {
        Destroy(this.GetComponent<CapsuleCollider>());
        animator.enabled = false;
        foreach(CharacterJoint joint in joints)
        {
            joint.enableCollision = true;
        }
        foreach(Collider collider in colliders)
        {
            collider.enabled = true;
        }
        foreach(Rigidbody rigidbody in rigidBodies)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.detectCollisions = true;
            rigidbody.useGravity = true;
            rigidbody.isKinematic = false;
        }
    }

    public void EnableAnimator()
    {
        animator.enabled = true;
        foreach (CharacterJoint joint in joints)
        {
            joint.enableCollision = false;
        }
        foreach (Collider collider in colliders)
        {
            collider.enabled = false;
        }
        foreach (Rigidbody rigidbody in rigidBodies)
        {
            rigidbody.detectCollisions = false;
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
        }
    }
}
