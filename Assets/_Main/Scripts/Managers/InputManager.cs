using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager>
{
    private Vector2 move;

    private Vector2 look;

    private Vector2 lookDelta;

    private Vector3 botLook;

    private bool isAim;

    private bool isAimCancel;

    private bool isSprint;

    private bool isAttack;

    private bool isShop;

    private bool isScoreBoard;

    private bool isChat;

    private bool isCamAimPrev;

    private bool isCamAimNext;

    private bool[] isDebugKey;


    public Vector2 Move { get => move; }

    public Vector2 Look { get => look; }

    public Vector2 LookDelta { get => lookDelta; }

    public Vector3 BotLook { get => botLook; }

    public bool IsAim { get => isAim; }

    

    public bool IsSprint { get => isSprint; }

    public bool IsAttack { get => isAttack; }

    public bool IsAimCancel
    {
        get
        {
            var oldValue = isAimCancel;

            isAimCancel = false;

            if (oldValue)
            {
                isAim = false;
            }
            
            return oldValue;
        }
    }

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

    public bool IsCamAimPrev
    {
        get
        {
            var oldValue = isCamAimPrev;

            isCamAimPrev = false;

            return oldValue;
        }
    }

    public bool IsCamAimNext
    {
        get
        {
            var oldValue = isCamAimNext;

            isCamAimNext = false;

            return oldValue;
        }
    }

    public bool IsDebugKey(int index)
    {
        var oldValue = isDebugKey[index];

        isDebugKey[index] = false;

        return oldValue;
    }

    #region Unity

    private void Awake()
    {
        // Automatically cache singleton class
        // Why? Because if not, random bot will take control of main player's control
        // Why? Because a singleton class means there's one script in scene, but this script is attached to all bots and player ship
        // Why? Lazy implementation
        var _ = Instance;

        isDebugKey = new bool[10];
    }

    #endregion

    #region Player Input Bindings

    public void OnMove(InputValue value) => move = value.Get<Vector2>();

    public void OnLook(InputValue value) => look = value.Get<Vector2>();

    public void OnLookDelta(InputValue value) => lookDelta = value.Get<Vector2>();

    public void OnAim(InputValue value) => isAim = value.isPressed;

    public void OnAimCancel(InputValue value) => isAimCancel = value.isPressed;

    public void OnSprint(InputValue value) => isSprint = value.isPressed;

    public void OnAttack(InputValue value) => isAttack = value.isPressed;

    public void OnShop(InputValue value) => isShop = value.isPressed;

    public void OnScoreBoard(InputValue value) => isScoreBoard = value.isPressed;

    public void OnChat(InputValue value) => isChat = value.isPressed;

    public void OnCamAimPrev(InputValue value) => isCamAimPrev = value.isPressed;

    public void OnCamAimNext(InputValue value) => isCamAimNext = value.isPressed;

    public void OnDebugKey0(InputValue value) => isDebugKey[0] = value.isPressed;

    public void OnDebugKey1(InputValue value) => isDebugKey[1] = value.isPressed;

    public void OnDebugKey2(InputValue value) => isDebugKey[2] = value.isPressed;

    public void OnDebugKey3(InputValue value) => isDebugKey[3] = value.isPressed;

    public void OnDebugKey4(InputValue value) => isDebugKey[4] = value.isPressed;

    public void OnDebugKey5(InputValue value) => isDebugKey[5] = value.isPressed;

    public void OnDebugKey6(InputValue value) => isDebugKey[6] = value.isPressed;

    public void OnDebugKey7(InputValue value) => isDebugKey[7] = value.isPressed;

    public void OnDebugKey8(InputValue value) => isDebugKey[8] = value.isPressed;

    public void OnDebugKey9(InputValue value) => isDebugKey[9] = value.isPressed;

    #endregion

    #region Bot Inputs

    public void OnMove(Vector2 value)
    {
        move = value;
    }

    public void OnLook(Vector3 value)
    {
        botLook = value;
    }

    public void OnLookDelta(Vector2 value)
    {
        lookDelta = value;
    }

    public void OnAim(bool value)
    {
        isAim = value;
    }

    public void OnAimCancel(bool value)
    {
        isAimCancel = value;
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
