using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;

// TODO: this code is buggy as hell, fix later
public class PlayerStatusManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private Material materialNormal;

    [SerializeField]
    private Material materialInvisible;

    private List<int> statusGroupIds;

    private List<StatModifier> statusGroups;

    private PlayerSoundVisualManager visuals;

    public bool IsInvisible { get => statusGroups.Any(i => i.IsInvisible); }

    public float BuffMaxHealth { get => 1 + statusGroups.Select(i => i.BuffMaxHealth).Sum(); }

    public float BuffMaxMana { get => 1 + statusGroups.Select(i => i.BuffMaxMana).Sum(); }

    public float BuffAttackDamage { get => 1 + statusGroups.Select(i => i.BuffAttackDamage).Sum(); }

    public float BuffAbilityPower { get => 1 + statusGroups.Select(i => i.BuffAbilityPower).Sum(); }

    public float BuffArmor { get => 1 + statusGroups.Select(i => i.BuffArmor).Sum(); }

    public float BuffResist { get => 1 + statusGroups.Select(i => i.BuffResist).Sum(); }

    public float BuffAttackSpeed { get => 1 + statusGroups.Select(i => i.BuffAttackSpeed).Sum(); }

    public float BuffMoveSpeed { get => 1 + statusGroups.Select(i => i.BuffMoveSpeed).Sum(); }

    public void AddStatusGroup(StatModifier statusGroup)
    {
        statusGroupIds.Add(statusGroup.Id);

        statusGroups.Add(statusGroup);
    }

    void Awake()
    {
        visuals = GetComponent<PlayerSoundVisualManager>();

        statusGroupIds = new List<int>();

        statusGroups = new List<StatModifier>();
    }

    void Update()
    {
        /*foreach (var statusGroup in statusGroups) // TODO: add check, if photon view is mine
        {
            statusGroup.Duration -= Time.deltaTime;
        }*/

        visuals.RendererShip.material = IsInvisible ? materialInvisible : materialNormal;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(statusGroupIds);
        }
        else
        {
            //statusGroupIds = (List<int>)stream.ReceiveNext();
        }
    }
}
