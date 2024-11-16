using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(!GetComponent<Shake>().shaking)
        {
            float posX = player.transform.position.x;
            float posY = player.transform.position.y;
            transform.position = new Vector3(posX, posY, transform.position.z);
        }
        
    }
}
