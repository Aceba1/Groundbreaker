using UnityEngine;

class InputMapper : InputSystem
{
    public InputSystem target;

    public void AddToWatch(InputSystem system) =>
        system.OnUpdate += SwitchTarget;

    public void RemoveFromWatch(InputSystem system) =>
        system.OnUpdate -= SwitchTarget;

    private void SwitchTarget(InputSystem newTarget)
    {
        if (newTarget == target) return;

        if (target)
            Unsubscribe();

        target = newTarget;
        Subscribe();
    }

    private void UpdateGrab(bool value) => Grab = value;
    private void UpdateJump(bool value) => Jump = value;
    private void UpdateMove(Vector2 value) => Move = value;
    private void UpdatePause() => Pause();

    private void Subscribe()
    {
        target.OnGrabEvent += UpdateGrab;
        target.OnJumpEvent += UpdateJump;
        target.OnMoveEvent += UpdateMove;
        target.OnPauseEvent += UpdatePause;
    }

    private void Unsubscribe()
    {
        target.OnGrabEvent -= UpdateGrab;
        target.OnJumpEvent -= UpdateJump;
        target.OnMoveEvent -= UpdateMove;
        target.OnPauseEvent -= UpdatePause;
    }
}