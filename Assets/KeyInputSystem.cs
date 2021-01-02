using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInputSystem : InputSystem
{
    [SerializeField]
    public KeyCode upKey, downKey, leftKey, rightKey, jumpKey, grabKey, pauseKey;

    private void LateUpdate() // Should this be Update or LateUpdate?
    {
        int moveX = (Input.GetKey(rightKey) ? 1 : 0) - (Input.GetKey(leftKey) ? 1 : 0);
        int moveY = (Input.GetKey(upKey) ? 1 : 0) - (Input.GetKey(downKey) ? 1 : 0);
        
        //if (normalize && (moveX != 0 && moveY != 0)) // Both active at the same time?
        //    Move = new Vector2(moveX * 0.7071f, moveY * 0.7071f); // Normalized
        //else
        Move = new Vector2(moveX, moveY);

        Jump = Input.GetKey(jumpKey);
        Grab = Input.GetKey(grabKey);

        if (Input.GetKeyDown(pauseKey))
            Pause();
    }
}
