using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] GameObject shadow;
    private Vector3 shadowInitialPosition;
    // Start is called before the first frame update
    void Start()
    {
        shadowInitialPosition = shadow.transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void LateUpdate ()
    {
        // keep the shadow at the bottom of the player
        shadow.transform.rotation = Quaternion.identity;
        shadow.transform.position = transform.position + shadowInitialPosition;
    }
}
