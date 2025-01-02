using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.UI;

public class SwitchUIInput : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] VirtualMouseInput virtualMouseControl;
    [SerializeField] GameObject virtualMouse;
    [SerializeField] RectTransform canvas;
    [SerializeField] RectTransform parent;
    // Start is called before the first frame update
    // void Start()
    // {
    //     virtualMouseControl.cursorTransform.localPosition = virtualMouseControl.virtualMouse.position;
    // }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Mouse.current.position);
        
        if(playerInput.currentControlScheme == "Controller")
        {
            //Debug.Log("controller");
            // virtualMouseControl.enabled = true;
            virtualMouse.SetActive(true);
            // Vector2 localPosition;
            // RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, parent.localPosition, Camera.main, out localPosition);
            // parent.localPosition = localPosition;
            //InputState.Change(virtualMouseControl.virtualMouse.position, Mouse.current.position);
        }
        else
        {
            //Debug.Log("keyboard");
            // virtualMouseControl.enabled = false;
            virtualMouse.SetActive(false);
        }
    }
}
