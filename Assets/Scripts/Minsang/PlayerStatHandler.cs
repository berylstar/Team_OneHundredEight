using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using Photon.Pun;

public class PlayerStatHandler : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private PlayerStatSO initialStat;

    public PlayerStat CurrentStat { get; private set; }
    public LinkedList<PlayerStat> statModifiers = new LinkedList<PlayerStat>();
 
    [SerializeField] private float healthChangeDelay = .5f;
    private float _timeSinceLastChange = float.MaxValue;
    private bool _invincibility = false;

    private bool _isReady = false;

    public event Action OnDamage;
    public event Action OnHeal;
    public event Action OnDeath;
    public event Action OnInvincibilityEnd;

    public Coroutine co;

    //TODO : 나중에 헬스시스템으로 따로 빼는게 괜찮긴할듯합니다
    public void SetInvincible(bool onoff)
    {
        StopCoroutine(co);
        _invincibility = onoff;
    }

    public bool ChangeHealth(float change)
    {
        if (change == 0  || _invincibility)
        {
            return false;
        }

        _timeSinceLastChange = 0f;
        CurrentStat.HP += change;
        CurrentStat.HP = Mathf.Clamp(CurrentStat.HP, 0.0f, CurrentStat.MaxHp);
        
        StopCoroutine(co);
        co = StartCoroutine(COInvincible(healthChangeDelay));
        
        if (change > 0)
        {
            OnHeal?.Invoke();
        }
        else
        {
            OnDamage?.Invoke();
        }

        if (CurrentStat.HP <= 0.0f)
        {
            OnDeath?.Invoke();
        }

        return true;
    }

    public IEnumerator COInvincible(float Time)
    {
        _invincibility = true;
        yield return new WaitForSeconds(Time);
        _invincibility = false;
    }

    public void SetHealth(float change)
    {
        //이렇게 써도되겠지만 고민좀 해봅시다 네..
        CurrentStat.HP = change;
    }

    public void InitPlayerStat(PlayerStatSO initialStat)
    {
        InitPlayerStat();

        GameManager.Instance.PlayerStats.Add(this);
    }

    public void InitPlayerStat()
    {
        if(!TryGetComponent<PhotonView>(out var pv) || !pv.IsMine)
        {
            return;
        }

        CurrentStat = new PlayerStat();
        this.initialStat = initialStat;
        CurrentStat.HP = initialStat.MaxHp;
        CurrentStat.MaxHp = initialStat.MaxHp;
        CurrentStat.MoveSpeed = initialStat.MoveSpeed;
        CurrentStat.JumpForce = initialStat.JumpForce;

        pv.RPC(nameof(ReadyRPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void ReadyRPC()
    {
        _isReady = true;
    }

    public void AddStatModifier(PlayerStat statModifier)
    {
        statModifiers.AddLast(statModifier);
        UpdateCharacterStats();
    }

    public void RemoveStatModifier(PlayerStat statModifier)
    {
        statModifiers.Remove(statModifier);
        UpdateCharacterStats();
    }

    public void UpdateCharacterStats()
    {
        SetBaseStat();

        foreach (PlayerStat modifier in statModifiers.OrderBy(x => x.statsChangeType))
        {
            if (modifier.statsChangeType == Define.StatsChangeType.Override)
            {
                UpdateStat((o, o1) => o1, modifier);
            }
            else if (modifier.statsChangeType == Define.StatsChangeType.Add)
            {
                UpdateStat((o, o1) => o + o1, modifier);
            }
            else if (modifier.statsChangeType == Define.StatsChangeType.Multiple)
            {
                UpdateStat((o, o1) => o * o1, modifier);
            }
        }
    }

    private void SetBaseStat()
    {
        CurrentStat.MaxHp = initialStat.MaxHp;
        CurrentStat.MoveSpeed = initialStat.MoveSpeed;
        CurrentStat.JumpForce = initialStat.JumpForce;
    }

    private void UpdateStat(Func<float, float, float> operation, PlayerStat newModifier)
    {
        CurrentStat.MaxHp = operation(CurrentStat.MaxHp, newModifier.MaxHp);
        CurrentStat.JumpForce = operation(CurrentStat.JumpForce, newModifier.JumpForce);
        CurrentStat.MoveSpeed = operation(CurrentStat.MoveSpeed, newModifier.MoveSpeed);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (!_isReady)
            return;

        if (stream.IsWriting)
        {
            stream.SendNext(CurrentStat.HP);
            stream.SendNext(CurrentStat.MaxHp);
        }
        else
        {
            CurrentStat.HP = (float)stream.ReceiveNext();
            CurrentStat.MaxHp = (float)stream.ReceiveNext();
        }
    }
}
