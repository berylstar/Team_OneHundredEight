using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerStatController : MonoBehaviour, IPunObservable
{
    [SerializeField] private PlayerStatSO initialStat;

    public int HP { get; private set; }
    public int MaxHp { get; private set; }
    public float MoveSpeed { get; private set; }
    public float JumpForce { get; private set; }

    public GameObject Bullet { get; private set; }
    public int MaxMagazine { get; private set; }
    public float Shootingdelay { get; private set; }
    public float ReloadSpeed { get; private set; }

    private void Start()
    {
        HP = initialStat.MaxHp;
        MaxHp = initialStat.MaxHp;
        MoveSpeed = initialStat.MoveSpeed;
        JumpForce = initialStat.JumpForce;
        Bullet = initialStat.Bullet;
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
            stream.SendNext(MoveSpeed);
            stream.SendNext(JumpForce);
            stream.SendNext(MaxMagazine);
            stream.SendNext(Shootingdelay);
            stream.SendNext(ReloadSpeed);
        }
        else
        {
            HP = (int)stream.ReceiveNext();
            MaxHp = (int)stream.ReceiveNext();
            MoveSpeed = (float)stream.ReceiveNext();
            JumpForce = (float)stream.ReceiveNext();
            MaxMagazine = (int)stream.ReceiveNext();
            Shootingdelay = (float)stream.ReceiveNext();
            ReloadSpeed = (float)stream.ReceiveNext();
        }
    }
}
