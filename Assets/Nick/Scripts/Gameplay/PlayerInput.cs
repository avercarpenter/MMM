using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : Singleton<PlayerInput>
{
    private Vector2 screenBounds;
    private float playerWidth;
    private float playerHeight;
    private float playerXPos;
    private float playerYPos;
    private float lastYPos;

    private Animator _playerAnim;

    [Range(-1,1)][SerializeField] float xPosition;

    public void OnMoveMouse(InputAction.CallbackContext context)
    {
        Move(context.ReadValue<Vector2>());
    }

    public void OnMouseClick(InputAction.CallbackContext context)
    {
        if (context.performed)
            Slash();
    }

    public void OnMouseClickAndHold(InputAction.CallbackContext context)
    {
        if (context.performed)
            SpinSlash();
        if (context.canceled)
        {
            _playerAnim.SetBool("Spinning", false);
            _playerAnim.ResetTrigger("SpinSlash");
        }
    }

    private void Start()
    {
        _playerAnim = GetComponent<Animator>();

        // Calculate the boundaries of the screen, height, width of player sprite
        screenBounds = ScreenManager.Instance.ScreenBounds;
        playerHeight = transform.GetComponent<SpriteRenderer>().bounds.size.y / 2;
        playerWidth = transform.GetComponent<SpriteRenderer>().bounds.size.x / 2;

        // Use the screen boundaries to clamp the x position of the player sprite
        playerXPos = screenBounds.x * -xPosition;
        playerXPos = Mathf.Clamp(playerXPos, screenBounds.x + playerWidth, -screenBounds.x - playerWidth);
    }

    // Have the camera and player constantly move right while staying in bounds
    private void Update()
    {
        transform.position = playerYPos * Vector3.up + Vector3.right * playerXPos;
    }

    // Move the player up and down the screen using the mouse while clamping the player in the screen boundaries
    private void Move(Vector2 mousePos)
    {
        float newMouseYPos = Mathf.Lerp(-1f, 1f, (mousePos.y - ScreenManager.Instance.BottomLeftScreenPos.y) / ScreenManager.Instance.InsideScreenSize.y);
        playerYPos = newMouseYPos * -screenBounds.y;
        playerYPos = Mathf.Clamp(playerYPos, screenBounds.y + playerHeight, -screenBounds.y - playerHeight);

        if (lastYPos < newMouseYPos)
            _playerAnim.SetFloat("mouseYPos", 1);
        else if (lastYPos > newMouseYPos)
            _playerAnim.SetFloat("mouseYPos", -1);
        else
            _playerAnim.SetFloat("mouseYPos", 0);

        lastYPos = newMouseYPos;
    }

    private void Slash()
    {
        _playerAnim.SetTrigger("Slash");
        print("slash");
    }

     private void SpinSlash()
    {
        _playerAnim.ResetTrigger("Slash");
        _playerAnim.SetBool("Spinning", true);
        _playerAnim.SetTrigger("SpinSlash");
        print("spinslash");
    }

}