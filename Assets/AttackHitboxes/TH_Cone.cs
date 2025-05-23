using System;
using System.Collections;
using UnityEngine;

public class TH_Cone : TelegraphedHitbox
{
    private float damageMod;
    void Start()
    {
        damageMod = transform.parent.GetComponent<EnemyInfo>().damageMod;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();

            // cancel grapple and movement
            playerRb.GetComponent<PlayerGrapple>().EndGrapple();
            StartCoroutine(TempStopMovement(other.gameObject));

             // increase damage based on difficulty
            int damage = (int) Math.Ceiling(Damage * damageMod);
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage);
            
            Vector3 pushDir = (other.gameObject.transform.position - transform.parent.position).normalized;
            playerRb.AddForce(pushDir * 4f, ForceMode2D.Impulse);

            SetAllCollidersStatus(false);

        }
    }

    IEnumerator TempStopMovement(GameObject player)
    {
        player.GetComponent<PlayerMovement>().CanMove = false;
        yield return new WaitForSeconds(ActiveTime);
        player.GetComponent<PlayerMovement>().CanMove = true;
    }

    // public override void Setup()
    // {
    // }
}
