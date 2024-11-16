using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowCone : MonoBehaviour
{
    Animation anim;
    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animation>();
    }

    public void Finished()
    {
        Destroy(gameObject);
    }
}
