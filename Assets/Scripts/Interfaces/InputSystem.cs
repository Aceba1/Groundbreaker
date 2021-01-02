using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSystem : MonoBehaviour
{
    private bool jump;
    private bool grab;
    private Vector2 move;

    public Action<bool> OnJumpEvent;
    public Action<bool> OnGrabEvent;
    public Action<Vector2> OnMoveEvent;
    public Action OnPauseEvent;

    public virtual bool Jump
    {
        get => jump;
        set
        {
            if (jump != value)
                OnJumpEvent?.Invoke(value);
            jump = value;
        }
    }

    public virtual bool Grab
    {
        get => grab;
        set
        {
            if (grab != value)
                OnGrabEvent?.Invoke(value);
            grab = value;
        }
    }

    public virtual Vector2 Move
    {
        get => move;
        set
        {
            if (move != value)
                OnMoveEvent?.Invoke(value);
            move = value;
        }
    }

    protected void Pause() =>
        OnPauseEvent?.Invoke();
}
