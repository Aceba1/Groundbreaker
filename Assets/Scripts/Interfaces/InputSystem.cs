using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputSystem : MonoBehaviour
{
    protected bool jump;
    protected bool grab;
    protected Vector2 move;

    public Action<bool> OnJumpEvent;
    public Action<bool> OnGrabEvent;
    public Action<Vector2> OnMoveEvent;
    public Action OnPauseEvent;
    public Action<InputSystem> OnUpdate;

    public virtual bool Jump
    {
        get => jump;
        set
        {
            if (jump != value)
            {
                jump = value;
                OnUpdate?.Invoke(this);
                OnJumpEvent?.Invoke(value);
            }
        }
    }

    public virtual bool Grab
    {
        get => grab;
        set
        {
            if (grab != value)
            {
                grab = value;
                OnUpdate?.Invoke(this);
                OnGrabEvent?.Invoke(value);
            }
        }
    }

    public virtual Vector2 Move
    {
        get => move;
        set
        {
            if (move != value)
            {
                move = value;
                OnUpdate?.Invoke(this);
                OnMoveEvent?.Invoke(value);
            }
        }
    }

    protected virtual void Pause() =>
        OnPauseEvent?.Invoke();
}
