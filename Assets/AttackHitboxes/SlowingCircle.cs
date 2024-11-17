using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingCircle : TelegraphedHitbox
{
    private float damageRate = 1;
    private float timer = 0;
    private bool doDamageEffect = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Tick());
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
            playerMovement.linearDrag = 1;
            playerMovement.baseMoveSpeed = 1;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {

        if(other.gameObject.tag == "Player" && doDamageEffect)
        {
            //Debug.Log("damage");

            // PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            // playerMovement.currSpeed -= 2;

            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(Damage);

            doDamageEffect = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player" && doDamageEffect)
        {
            PlayerMovement playerMovement = other.gameObject.GetComponent<PlayerMovement>();
            playerMovement.linearDrag = 0.4f;
            playerMovement.baseMoveSpeed = 2;
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
