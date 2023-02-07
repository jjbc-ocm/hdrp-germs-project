using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private Vector2 move;

    private Vector2 look;

    private bool isAim;

    private bool isSprint;

    private bool isAttack;

    private bool isShop;

    private bool isScoreBoard;

    private bool isChat;

    public Vector2 Move { get => move; }

    public Vector2 Look { get => look; }

    public bool IsAim { get => isAim; }

    public bool IsSprint { get => isSprint; }

    public bool IsAttack { get => isAttack; }

    public bool IsShop
    {
        get
        {
            var oldValue = isShop;

            isShop = false;

            return oldValue;
        }
    }

    public bool IsScoreBoard
    {
        get
        {
            var oldValue = isScoreBoard;

            isScoreBoard = false;

            return oldValue;
        }
    }

    public bool IsChat
    {
        get
        {
            var oldValue = isChat;

            isChat = false;

            return oldValue;
        }
    }

    #region Player Input Bindings

    public void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();
    }

    public void OnAim(InputValue value)
    {
        isAim = value.isPressed;
    }

    public void OnSprint(InputValue value)
    {
        isSprint = value.isPressed;
    }

    public void OnAttack(InputValue value)
    {
        isAttack = value.isPressed;
    }

    public void OnShop(InputValue value)
    {
        isShop = value.isPressed;
    }

    public void OnScoreBoard(InputValue value)
    {
        isScoreBoard = value.isPressed;
    }

    public void OnChat(InputValue value)
    {
        isChat = value.isPressed;
    }

    #endregion

    #region Bot Inputs

    public void OnMove(Vector2 value)
    {
        move = value;
    }

    public void OnLook(Vector2 value)
    {
        look = value;
    }

    public void OnAim(bool value)
    {
        isAim = value;
    }

    public void OnSprint(bool value)
    {
        isSprint = value;
    }

    public void OnAttack(bool value)
    {
        isAttack = value;
    }

    #endregion
}
