using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowCone : MonoBehaviour
{
    Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animation>();
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public void Finished()
    {
        // float time = transform.parent.gameObject.GetComponent<EnemyTelegraphAttack>().activeTime;
        // anim["start"].time = time;
        Destroy(gameObject);
    }
}
