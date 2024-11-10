using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TH_Cone : TelegraphedHitbox
{
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(Damage);
            Rigidbody2D playerRb = other.gameObject.GetComponent<Rigidbody2D>();

            Vector3 pushDir = (other.gameObject.transform.position - transform.parent.position).normalized;
            playerRb.AddForce(pushDir * 70f, ForceMode2D.Impulse);

            StartCoroutine(TempStopMovement(other.gameObject));
        }
    }

    IEnumerator TempStopMovement(GameObject player)
    {
        player.GetComponent<PlayerMovement>().SetMovementAbility(false);
        yield return new WaitForSeconds(ActiveTime);
        player.GetComponent<PlayerMovement>().SetMovementAbility(true);
    }

    public override void Setup()
    {
    }
}
