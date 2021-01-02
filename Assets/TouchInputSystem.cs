using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputSystem : InputSystem
{
    [SerializeField]
    Joystick moveInput;
    [SerializeField]
    SimpleButton jumpInput, grabInput, pauseButton;

    private void SetJump(bool value) => Jump = value;
    private void SetGrab(bool value) => Grab = value;

    private void OnEnable()
    {
        jumpInput.OnChangeEvent += SetJump;
        grabInput.OnChangeEvent += SetGrab;
        pauseButton.OnClickEvent += Pause;
    }

    private void OnDisable()
    {
        jumpInput.OnChangeEvent -= SetJump;
        grabInput.OnChangeEvent -= SetGrab;
        pauseButton.OnClickEvent -= Pause;
    }

    private void LateUpdate() // Should this be Update or LateUpdate?
    {
        Vector2 move = moveInput.RawDirection;
        float dist = Mathf.Max(Mathf.Abs(move.x), Mathf.Abs(move.y));
        if (dist > 1f)
            move /= dist;
        Move = move;
    }
}
