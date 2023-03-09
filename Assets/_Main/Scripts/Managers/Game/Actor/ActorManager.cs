using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActorManager : GameEntityManager
{
    #region Serializables

    [SerializeField]
    protected bool isMonster;

    [Header("Events")]
    public UnityEvent<int> onDieEvent;

    #endregion

    #region Components

    private Rigidbody rigidBody;

    private Collider[] colliders;

    private AimManager aim;

    private ItemAimManager itemAim;

    private PlayerSoundVisualManager soundVisuals;

    private PlayerStatManager stat;

    private PlayerStatusManager status;

    private PlayerInventoryManager inventory;

    private InputManager input;

    private BotManager bot;

    #endregion

    #region Accessors

    public bool IsMonster { get => isMonster; }

    public Rigidbody RigidBody { get => GetComponent(rigidBody); }

    public Collider[] Colliders { get => GetComponents(colliders); }

    public AimManager Aim { get => GetComponent(aim); }

    public ItemAimManager ItemAim { get => GetComponent(itemAim); }

    public PlayerSoundVisualManager SoundVisuals { get => GetComponent(soundVisuals); }

    public PlayerStatManager Stat { get => GetComponent(stat); }

    public PlayerStatusManager Status { get => GetComponent(status); }

    public PlayerInventoryManager Inventory { get => GetComponent(inventory); }

    public InputManager Input
    {
        get
        {
            var attachedComponent = GetComponent(input);

            if (attachedComponent == InputManager.Instance)
            {
                Debug.LogError("InputManager singleton is assigned to " + gameObject.name);
            }
            
            if (attachedComponent != null)
            {
                return attachedComponent;
            }

            

            return InputManager.Instance;
        }
    }

    public BotManager Bot { get => GetComponent(bot); }

    public bool IsBot { get => GetComponent(bot) != null; }

    #endregion

    #region Public

    public string GetName()
    {
        return Bot?.Info.Name ?? photonView.Owner.GetName();
    }

    public int GetTeam()
    {
        return Bot?.Info.Team ?? photonView.Owner.GetTeam();
    }

    public bool HasChest()
    {
        return Bot?.Info.HasChest ?? photonView.Owner.HasChest();
    }

    public bool HasSurrendered()
    {
        return Bot?.Info.HasSurrendered ?? photonView.Owner.HasSurrendered();
    }

    public void HasChest(bool value)
    {
        if (Bot != null)
        {
            Bot.Info.HasChest = value;
        }
        else
        {
            photonView.Owner.HasChest(value);
        }
    }

    public void HasSurrendered(bool value)
    {
        if (Bot != null)
        {
            Bot.Info.HasSurrendered = value;
        }
        else
        {
            photonView.Owner.HasSurrendered(value);
        }
    }

    public T GetComponent<T>(T value)
    {
        if (value == null)
        {
            value = GetComponent<T>();
        }

        return value;
    }

    public T[] GetComponents<T>(T[] value)
    {
        if (value == null)
        {
            value = GetComponents<T>();
        }

        return value;
    }

    #endregion

    #region Override

    [PunRPC]
    public abstract void RpcDamageHealth(int amount, int attackerId);

    #endregion
}
