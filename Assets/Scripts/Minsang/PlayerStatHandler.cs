using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

using Photon.Pun;

public class PlayerStatHandler : MonoBehaviour, IPunObservable
{
    [SerializeField] private PlayerStatSO initialStat;

    [field: SerializeField] public WeaponData Weapon { get; private set; }

    public PlayerStat CurrentStat { get; private set; }
    public LinkedList<PlayerStat> statModifiers = new LinkedList<PlayerStat>();
 
    [SerializeField] private float healthChangeDelay = .5f;
    private float _timeSinceLastChange = float.MaxValue;
    private bool _invincibility = false;

    public event Action OnDamage;
    public event Action OnHeal;
    public event Action OnDeath;
    public event Action OnInvincibilityEnd;

    //TODO : 나중에 헬스시스템으로 따로 빼는게 괜찮긴할듯합니다
    public void SetInvincible(bool onoff)
    {
        _invincibility = onoff;
    }

    public bool ChangeHealth(float change)
    {
        if (change == 0 || _timeSinceLastChange < healthChangeDelay || _invincibility)
        {
            return false;
        }

        _timeSinceLastChange = 0f;
        CurrentStat.HP += change;
        CurrentStat.HP = Mathf.Clamp(CurrentStat.HP, 0.0f, CurrentStat.MaxHp);

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

    private void Awake()
    {
        CurrentStat = new PlayerStat();
    }

    private void Start()
    {
        InitPlayerStat();

        GameManager.Instance.PlayerStats.Add(this);
    }

    public void InitPlayerStat()
    {
        CurrentStat.HP = initialStat.MaxHp;
        CurrentStat.MaxHp = initialStat.MaxHp;
        CurrentStat.MoveSpeed = initialStat.MoveSpeed;
        CurrentStat.JumpForce = initialStat.JumpForce;

        Weapon = GameManager.Instance.Weapons[initialStat.WeaponIndex];
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
        CurrentStat.MaxHp = (int)operation(CurrentStat.MaxHp, newModifier.MaxHp);
        CurrentStat.JumpForce = operation(CurrentStat.JumpForce, newModifier.JumpForce);
        CurrentStat.MoveSpeed = operation(CurrentStat.MoveSpeed, newModifier.MoveSpeed);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(CurrentStat.HP);
            stream.SendNext(CurrentStat.MaxHp);
            stream.SendNext(CurrentStat.JumpForce);
            stream.SendNext(CurrentStat.MoveSpeed);
        }
        else
        {
            CurrentStat.HP = (float)stream.ReceiveNext();
            CurrentStat.MaxHp = (float)stream.ReceiveNext();
            CurrentStat.JumpForce = (float)stream.ReceiveNext();
            CurrentStat.MoveSpeed = (float)stream.ReceiveNext();
        }
    }
}
