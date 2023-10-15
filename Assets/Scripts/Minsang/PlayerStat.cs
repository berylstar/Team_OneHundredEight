using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerStat : MonoBehaviour, IPunObservable
{
    [SerializeField] private PlayerStatSO initialStat;

    [field: SerializeField] public int HP { get; private set; }
    public int MaxHp { get; private set; }
    public float MoveSpeed { get; private set; }
    public float JumpForce { get; private set; }

    public int MaxMagazine { get; private set; }
    public float Shootingdelay { get; private set; }
    public float ReloadSpeed { get; private set; }

    private void Start()
    {
        HP = initialStat.MaxHp;
        MaxHp = initialStat.MaxHp;
        MoveSpeed = initialStat.MoveSpeed;
        JumpForce = initialStat.JumpForce;
        MaxMagazine = initialStat.MaxMagazine;
        Shootingdelay = initialStat.Shootingdelay;
        ReloadSpeed = initialStat.ReloadSpeed;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(HP);
            stream.SendNext(MaxHp);
        }
        else
        {
            HP = (int)stream.ReceiveNext();
            MaxHp = (int)stream.ReceiveNext();
        }
    }
}
