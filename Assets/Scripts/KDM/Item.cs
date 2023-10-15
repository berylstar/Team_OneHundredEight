using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Item : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    //���⼭ Ÿ���� ����? -> isMine�� �༮
    //isMine�� �༮�� ������ ��ȭ��Ű�� �������ֵ��� ������ �ٲ����.. ����?
    //->�׷��ٸ� �������� ������ �ʿ䰡 ���°;ƴұ�? (isMine�� �༮�� ����Ǹ� �Ǵ°Ŵϱ�..?)

    //1.�������� SO�� ���� ���� CharacterStats�� ������Ų��. (������������ ��ɼ���ȭ�� �����ϴ�)
    //�߰����Ի����� ����ϰ� �Ͼ�� ������ for���� ���ؼ� ��ü������ ��ȸ�ϰ� 3���� ���ÿ� �������ϱ⶧���� LinkedList�� �������� �� ������� ������
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
            //TODO(KDM) : ���� �����۵��� ������ȭ �صα�
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
                    //TODO(KDM) : ü�� ȸ�� �� ���� ����(HealthSystem)
                    break;
                case Define.BuffType.Speed:
                    //TODO(KDM) : ���� ���� ����(CharacterStatsHandler)
                    break;
                case Define.BuffType.Invincible:
                    //TODO(KDM) : ���� ����(HealthSystem)
                    break;
                case Define.BuffType.ReverseKey:
                    //TODO(KDM) : Ű���� ����(PlayerInput���̷���?)
                    break;
            }
        }
    }

    void RemoveStat(ItemStats stat)
    {
        switch (stat.statSO.BuffType)
        {
            case Define.BuffType.Hp:
                //TODO(KDM) : ����,���� �� ü�� ���� ����(HealthSystem)
                break;
            case Define.BuffType.Speed:
                //TODO(KDM) : ���� ���� ���� ����(CharacterStatsHandler)
                break;
            case Define.BuffType.Invincible:
                //TODO(KDM) : ���� ���� ����(HealthSystem)
                break;
            case Define.BuffType.ReverseKey:
                //TODO(KDM) : Ű���� ���� ����(PlayerInput���̷���?)
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
