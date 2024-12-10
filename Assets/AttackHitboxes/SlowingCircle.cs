using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingCircle : TelegraphedHitbox
{
    private float damageRate = 1;
    private float timer = 0;
    private bool doDamageEffect = false;
    private float damageMod;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Tick());
        damageMod = transform.parent.GetComponent<EnemyInfo>().damageMod;
    }

    // Update is called once per frame
    // void Update()
    // {
    //     // The slowing circle is a hazard that stays on the floor and then disapears
    //     if(attackEnded)
    //     {
    //         Destroy(gameObject);
    //     }

    //     timer += 0;
    //     if(timer >= damageRate)
    //     {
    //         doDamageEffect = true;
    //     }

    // }


    public override void UpdateHitboxPosition()
    {
        //transform.position = transform.position;
        return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && doDamageEffect)
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            other.gameObject.GetComponent<Rigidbody2D>().drag *= 1.3f;
            playerMovement.baseMoveSpeed *= 0.5f;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        if(other.gameObject.tag == "Player" && doDamageEffect)
        {
            //Debug.Log("damage");

            // PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            // playerMovement.currSpeed -= 2;

             // increase damage based on difficulty
            int damage = (int) Math.Ceiling(Damage * damageMod);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);

            doDamageEffect = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            PlayerGrapple playerGrapple = other.gameObject.GetComponent<PlayerGrapple>();
            other.gameObject.GetComponent<Rigidbody2D>().drag = playerGrapple.initialDrag;
            playerMovement.baseMoveSpeed = playerMovement.initialBaseMoveSpeed;
        }
    
    }

    private IEnumerator Tick()
    {
        while(true)
        {
            // if(attackEnded)
            // {
            //     Destroy(gameObject);
            // }

            timer += Time.deltaTime;
            if(timer >= damageRate)
            {
                doDamageEffect = true;
                timer = 0;
            }

            yield return null;

        }

    }

    // public override void Setup()
    // {
    //     //throw new System.NotImplementedException();
    // }
}
