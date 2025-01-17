using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickIdleState : MonoBehaviour
{
    // Start is called before the first frame update
    //[SerializeField] Animator animator;
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void ChooseIdleState()
    {
        animator.SetInteger("IdleToSwitch", Random.Range(0,2));
    }
}
