using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TanksMP;
using UnityEngine;

public class PlayerStatusManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private Material materialNormal;

    [SerializeField]
    private Material materialInvisible;

    private string itemId0;
    private string itemId1;
    private string itemId2;
    private string itemId3;
    private string itemId4;
    //private string itemId5;
    //private string itemId6;
    //private string itemId7;
    //private string itemId8;
    //private string itemId9;

    private float duration0;
    private float duration1;
    private float duration2;
    private float duration3;
    private float duration4;
    //private float duration5;
    //private float duration6;
    //private float duration7;
    //private float duration8;
    //private float duration9;

    private PlayerSoundVisualManager visuals;


    void Awake()
    {
        visuals = GetComponent<PlayerSoundVisualManager>();
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        duration0 = Mathf.Max(0, duration0 - Time.deltaTime);
        duration1 = Mathf.Max(0, duration1 - Time.deltaTime);
        duration2 = Mathf.Max(0, duration2 - Time.deltaTime);
        duration3 = Mathf.Max(0, duration3 - Time.deltaTime);
        duration4 = Mathf.Max(0, duration4 - Time.deltaTime);
        //duration5 = Mathf.Max(0, duration5 - Time.deltaTime);
        //duration6 = Mathf.Max(0, duration6 - Time.deltaTime);
        //duration7 = Mathf.Max(0, duration7 - Time.deltaTime);
        //duration8 = Mathf.Max(0, duration8 - Time.deltaTime);
        //duration9 = Mathf.Max(0, duration9 - Time.deltaTime);

        if (duration0 <= 0) itemId0 = "";
        if (duration1 <= 0) itemId1 = "";
        if (duration2 <= 0) itemId2 = "";
        if (duration3 <= 0) itemId3 = "";
        if (duration4 <= 0) itemId4 = "";
        //if (duration5 <= 0) itemId5 = "";
        //if (duration6 <= 0) itemId6 = "";
        //if (duration7 <= 0) itemId7 = "";
        //if (duration8 <= 0) itemId8 = "";
        //if (duration9 <= 0) itemId9 = "";
    }

    public bool TryApplyItem(ItemData data)
    {
        if (string.IsNullOrEmpty(itemId0)) { itemId0 = data.ID; duration0 += data.Duration; return true; }
        if (string.IsNullOrEmpty(itemId1)) { itemId1 = data.ID; duration1 += data.Duration; return true; }
        if (string.IsNullOrEmpty(itemId2)) { itemId2 = data.ID; duration2 += data.Duration; return true; }
        if (string.IsNullOrEmpty(itemId3)) { itemId3 = data.ID; duration3 += data.Duration; return true; }
        if (string.IsNullOrEmpty(itemId4)) { itemId4 = data.ID; duration4 += data.Duration; return true; }
        //if (string.IsNullOrEmpty(itemId5)) { itemId5 = data.ID; duration5 += data.Duration; return true; }
        //if (string.IsNullOrEmpty(itemId6)) { itemId6 = data.ID; duration6 += data.Duration; return true; }
        //if (string.IsNullOrEmpty(itemId7)) { itemId7 = data.ID; duration7 += data.Duration; return true; }
        //if (string.IsNullOrEmpty(itemId8)) { itemId8 = data.ID; duration8 += data.Duration; return true; }
        //if (string.IsNullOrEmpty(itemId9)) { itemId9 = data.ID; duration9 += data.Duration; return true; }

        return false;
    }

    /*public bool TryRemoveItem(ItemData data)
    {
        if (itemId0 == data.ID) { itemId0 = ""; return true; }
        if (itemId1 == data.ID) { itemId1 = ""; return true; }
        if (itemId2 == data.ID) { itemId2 = ""; return true; }
        if (itemId3 == data.ID) { itemId3 = ""; return true; }
        if (itemId4 == data.ID) { itemId4 = ""; return true; }
        if (itemId5 == data.ID) { itemId5 = ""; return true; }

        return false;
    }*/

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(itemId0);
            stream.SendNext(itemId1);
            stream.SendNext(itemId2);
            stream.SendNext(itemId3);
            stream.SendNext(itemId4);
            //stream.SendNext(itemId5);
            //stream.SendNext(itemId6);
            //stream.SendNext(itemId7);
            //stream.SendNext(itemId8);
            //stream.SendNext(itemId9);
            stream.SendNext(duration0);
            stream.SendNext(duration1);
            stream.SendNext(duration2);
            stream.SendNext(duration3);
            stream.SendNext(duration4);
            //stream.SendNext(duration5);
            //stream.SendNext(duration6);
            //stream.SendNext(duration7);
            //stream.SendNext(duration8);
            //stream.SendNext(duration9);
        }
        else
        {
            itemId0 = (string)stream.ReceiveNext();
            itemId1 = (string)stream.ReceiveNext();
            itemId2 = (string)stream.ReceiveNext();
            itemId3 = (string)stream.ReceiveNext();
            itemId4 = (string)stream.ReceiveNext();
            //itemId5 = (string)stream.ReceiveNext();
            //itemId6 = (string)stream.ReceiveNext();
            //itemId7 = (string)stream.ReceiveNext();
            //itemId8 = (string)stream.ReceiveNext();
            //itemId9 = (string)stream.ReceiveNext();
            duration0 = (float)stream.ReceiveNext();
            duration1 = (float)stream.ReceiveNext();
            duration2 = (float)stream.ReceiveNext();
            duration3 = (float)stream.ReceiveNext();
            duration4 = (float)stream.ReceiveNext();
            //duration5 = (float)stream.ReceiveNext();
            //duration6 = (float)stream.ReceiveNext();
            //duration7 = (float)stream.ReceiveNext();
            //duration8 = (float)stream.ReceiveNext();
            //duration9 = (float)stream.ReceiveNext();
        }
    }
}

public class Status
{

}