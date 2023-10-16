using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class PlayerStatHandler : MonoBehaviour, IPunObservable
{
    [SerializeField] private PlayerStatSO initialStat;

    [field: SerializeField] public int HP { get; private set; } // 인스펙터에서 확인용. 추후 제거
    [field: SerializeField] public int MaxHp { get; private set; }
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float JumpForce { get; private set; }
    [field: SerializeField] public WeaponData Weapon { get; private set; }

    private void Start()
    {
        HP = initialStat.MaxHp;
        MaxHp = initialStat.MaxHp;
        MoveSpeed = initialStat.MoveSpeed;
        JumpForce = initialStat.JumpForce;
        // Weapon = NetworkManager.Instance.WeaponDatas[initialStat.WeaponIndex];
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
