using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    private PlayerStatHandler _targetStatHandler;
    //여기서 타겟은 뭘까? -> isMine인 녀석
    //isMine인 녀석의 스텟을 변화시키면 나머지애들의 스텟이 바뀔거임.. ㅇㅈ?
    //->그렇다면 아이템은 포톤일 필요가 없는것아닐까? (isMine인 녀석만 적용되면 되는거니까..?)

    //1.아이템의 SO를 따로 만들어서 CharacterStats랑 연동시킨다. (내가귀찮지만 기능세분화가 가능하다)
    //중간삽입삭제가 빈번하게 나니까 링크드리스트로 -> 링크드리스트는 인스펙터에서 접근이안된다
    [SerializeField] private List<ItemStats> _itemStats;

    
    public static Item Create(GameObject target, Define.ItemType pickupType)
    {
        GameObject go;
        Item item;
        if (pickupType == Define.ItemType.Random)
        {
            pickupType = (Define.ItemType)Random.Range((int)Define.ItemType.Random + 1, (int)Define.ItemType.End);
        }
        switch (pickupType)
        {
            //TODO(KDM) : 각각 아이템들을 프리펩화 해두기
            case Define.ItemType.HpDown:
                go = Object.Instantiate(Resources.Load<GameObject>("Items/Hp30Down"));
                break;
            case Define.ItemType.HpUp:
                go = Object.Instantiate(Resources.Load<GameObject>("Items/Hp30Up"));
                break;
            case Define.ItemType.Invincible:
                go = Object.Instantiate(Resources.Load<GameObject>("Items/Invincible"));
                break;
            case Define.ItemType.SpeedDown:
                go = Object.Instantiate(Resources.Load<GameObject>("Items/SpeedMul0.5"));
                break;
            case Define.ItemType.SpeedUp:
                go = Object.Instantiate(Resources.Load<GameObject>("Items/SpeedMul1.5"));
                break;
            case Define.ItemType.ReverseKey:
                go = Object.Instantiate(Resources.Load<GameObject>("Items/Reverse"));
                break;
            default:
                return null;
        }
        item = go.GetComponent<Item>();

        item._target = target;
        item._targetStatHandler = target.GetComponent<PlayerStatHandler>();
        item.AllStatModifier();

        return item;
    }

    private void AllStatModifier()
    {
        for (int i = 0; i < _itemStats.Count; i++)
        {
            ItemStats stat = _itemStats[i];
            if (stat.Duration == 0)
                stat.isTimed = false;
            else
                stat.isTimed = true;


            switch (stat.statSO.BuffType)
            {
                case Define.BuffType.Hp:
                    if (stat.statSO.StatsChangeType == Define.StatsChangeType.Multiple)
                        _targetStatHandler.ChangeHealth(_targetStatHandler.CurrentStat.HP * stat.statSO.Value - _targetStatHandler.CurrentStat.HP);
                    else if (stat.statSO.StatsChangeType == Define.StatsChangeType.Add)
                        _targetStatHandler.ChangeHealth(stat.statSO.Value);
                    else
                        _targetStatHandler.ChangeHealth(stat.statSO.Value - _targetStatHandler.CurrentStat.HP);
                    break;
                case Define.BuffType.Speed:
                    stat.ApplyStat = new PlayerStat();
                    stat.ApplyStat.MoveSpeed = stat.statSO.Value;
                    stat.ApplyStat.statsChangeType = stat.statSO.StatsChangeType;
                    _targetStatHandler.AddStatModifier(stat.ApplyStat);
                    break;
                case Define.BuffType.Invincible:
                    _targetStatHandler.SetInvincible(true);
                    break;
            }
        }
    }

    void RemoveStat(ItemStats stat)
    {
        switch (stat.statSO.BuffType)
        {
            case Define.BuffType.Hp:
                //TODO(KDM) : 감소,증가 된 체력 복구 구현(HealthSystem) 이거는 안쓰는게 맞을 것 같은데.. 뭔말알? 나중에 필요하면 구현
                break;
            case Define.BuffType.Speed:
                _targetStatHandler.RemoveStatModifier(stat.ApplyStat);
                break;
            case Define.BuffType.Invincible:
                _targetStatHandler.SetInvincible(false);
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (_itemStats.Count == 0)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = _itemStats.Count - 1; i >= 0; --i)
        {
            ItemStats stat = _itemStats[i];

            if (stat.isTimed == false)
            {
                _itemStats.Remove(stat);
                continue;
            }

            stat.Duration -= Time.deltaTime;
            if (stat.Duration < 0)
            {
                RemoveStat(stat);
                _itemStats.Remove(stat);
            }
        }
    }

}
