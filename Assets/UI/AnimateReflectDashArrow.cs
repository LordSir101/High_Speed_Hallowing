using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateReflectDashArrow : MonoBehaviour
{
    [SerializeField] private InputActionReference movement;

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = movement.action.ReadValue<Vector2>();
        if(dir != Vector2.zero)
        {
            float radvalue = Mathf.Atan2(dir.y, dir.x);
            float angle= radvalue * (180/Mathf.PI);

            transform.localRotation = Quaternion.Euler(0,0,angle -90);
            transform.position = transform.position;
        }

        
        
    }
}
