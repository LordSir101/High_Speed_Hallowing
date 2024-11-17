using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NerdEnemyBehaviour : MonoBehaviour
{
    private float speed;
    [SerializeField] private float minSpeed, maxSpeed, patrolTime;

    private float moveAngle;
    private float radius;
    //private float reactiontime;

    Rigidbody2D rb;
    GameObject player;
    Vector2 startingPoint;
    private bool beginPatrol = false;
    Vector2 targetPoint;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        radius = 5;

        speed = UnityEngine.Random.Range(minSpeed, maxSpeed);

        GetMovementCircle();
        StartCoroutine(ChangePatrol());
    }

    // The nerd ghost moves in a circle around a point close to the player
    private void GetMovementCircle()
    {
        float range = 3f;
        float posX = UnityEngine.Random.Range(-range, range);
        float posY = UnityEngine.Random.Range(-range, range);

        targetPoint = new Vector2(player.transform.position.x + posX, player.transform.position.y + posY);

        Debug.Log(targetPoint);

        float startingAngle = UnityEngine.Random.Range(0, 360);
        moveAngle = startingAngle;
        startingPoint = targetPoint + new Vector2(Mathf.Cos(startingAngle), Mathf.Sin(startingAngle)) * radius;

        Debug.Log(startingPoint);

        rb.velocity = startingPoint - rb.position;
        beginPatrol = false;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(targetPoint, 0.5f);
    // }

    // Update is called once per frame
    void Update()
    {
        if((startingPoint - rb.position).magnitude < 0.5)
        {
            beginPatrol = true;
        }
        if(beginPatrol)
        {
            moveAngle += speed / radius * Mathf.PI * Time.deltaTime;
            //rb.transform.position = targetPoint  +  new Vector2(Mathf.Cos(moveAngle), Mathf.Sin(moveAngle)) * radius;
            Vector2 nextPos = targetPoint  +  new Vector2(Mathf.Cos(moveAngle), Mathf.Sin(moveAngle)) * radius;
            rb.velocity = (nextPos - rb.position) * speed;
            
        }
        
    }

    IEnumerator ChangePatrol()
    {       
        while(true)
        {
            yield return new WaitForSeconds(patrolTime);
            GetMovementCircle();
        }
    }
}
