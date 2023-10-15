using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    //여기서 타겟은 뭘까? -> isMine인 녀석
    //isMine인 녀석의 스텟을 변화시키면 나머지애들의 스텟이 바뀔거임.. ㅇㅈ?
    //->그렇다면 아이템은 포톤일 필요가 없는것아닐까? (isMine인 녀석만 적용되면 되는거니까..?)

    //1.아이템의 SO를 따로 만들어서 CharacterStats랑 연동시킨다. (내가귀찮지만 기능세분화가 가능하다)
    //중간삽입삭제가 빈번하게 일어나긴 하지만 for문을 통해서 전체적으로 순회하고 3개를 동시에 지워야하기때문에 LinkedList는 구현에서 좀 어려움이 있을듯
    [SerializeField] private List<ItemStats> _itemStats;
    public static Item Create(GameObject target , Define.ItemType pickupType)
    {
        GameObject go;
        Item item;
        if(pickupType == Define.ItemType.Random)
        {
            pickupType = (Define.ItemType)Random.Range((int)Define.ItemType.Random + 1 , (int)Define.ItemType.End);
        }
        switch (pickupType)
        {
            //TODO(KDM) : 각각 아이템들을 프리펩화 해두기
            case Define.ItemType.HpDown:
                go = Object.Instantiate(Resources.Load<GameObject>("Item"));
                break;
            case Define.ItemType.HpUp:
                go = Object.Instantiate(Resources.Load<GameObject>("Item"));
                break;
            case Define.ItemType.Invincible:
                go = Object.Instantiate(Resources.Load<GameObject>("Item"));
                break;
            case Define.ItemType.SpeedDown:
                go = Object.Instantiate(Resources.Load<GameObject>("Item"));
                break;
            case Define.ItemType.SpeedUp:
                go = Object.Instantiate(Resources.Load<GameObject>("Item"));
                break;
            case Define.ItemType.ReverseKey:
                go = Object.Instantiate(Resources.Load<GameObject>("Item"));
                break;
            default:
                return null;
        }
        item = go.GetComponent<Item>();

        item._target = target;

        item.AllStatModifier();
        
        return item;
    }

    private void AllStatModifier()
    {
        for(int i = 0 ; i < _itemStats.Count ; ++i)
        {
            ItemStats stat = _itemStats[i];

            if (stat.Duration == 0)
                stat.isTimed = false;
            else
                stat.isTimed = true;


            switch (stat.statSO.BuffType)
            {
                case Define.BuffType.Hp:
                    //TODO(KDM) : 체력 회복 및 감소 구현(HealthSystem)
                    break;
                case Define.BuffType.Speed:
                    //TODO(KDM) : 스텟 적용 구현(CharacterStatsHandler)
                    break;
                case Define.BuffType.Invincible:
                    //TODO(KDM) : 무적 구현(HealthSystem)
                    break;
                case Define.BuffType.ReverseKey:
                    //TODO(KDM) : 키반전 구현(PlayerInput쪽이려나?)
                    break;
            }
        }
    }

    void RemoveStat(ItemStats stat)
    {
        switch (stat.statSO.BuffType)
        {
            case Define.BuffType.Hp:
                //TODO(KDM) : 감소,증가 된 체력 복구 구현(HealthSystem)
                break;
            case Define.BuffType.Speed:
                //TODO(KDM) : 스텟 적용 해제 구현(CharacterStatsHandler)
                break;
            case Define.BuffType.Invincible:
                //TODO(KDM) : 무적 해제 구현(HealthSystem)
                break;
            case Define.BuffType.ReverseKey:
                //TODO(KDM) : 키반전 해제 구현(PlayerInput쪽이려나?)
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        foreach(ItemStats stat in _itemStats)
        {
            if(stat.isTimed == false)
            {
                _itemStats.Remove(stat);
                continue;
            }

            stat.Duration -= Time.deltaTime;
            if(stat.Duration < 0)
            {
                RemoveStat(stat);
                _itemStats.Remove(stat);
            }
        }

        if (_itemStats.Count == 0)
            Destroy(gameObject);
    }

}
