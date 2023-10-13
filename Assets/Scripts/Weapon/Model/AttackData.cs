using System;
using Unity.VisualScripting;

namespace Weapon.Model
{
    [Serializable]
    public class AttackData
    {
        public float damage;
        public int magazine;
        public float delay;
        public float bulletSpd;
 
        public static AttackData operator +(AttackData a, AttackData b)
        {
            return new AttackData()
            {
                damage = a.damage + b.damage,
                magazine = a.magazine + b.magazine,
                delay = a.delay + b.delay,
                bulletSpd = a.bulletSpd + b.bulletSpd,
            };
        }
    }
}