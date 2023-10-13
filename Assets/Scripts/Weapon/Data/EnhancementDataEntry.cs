using Weapon.Model;

namespace Weapon.Data
{
    public class EnhancementDataEntry
    {
        public string Name;
        public string IconName;
        public string Desc;
        public float Damage;
        public int Magazine;
        public float Delay;
        public float BulletSpd;

        public EnhancementData ToEnhancementData() => new EnhancementData()
        {
            Name = Name,
            IconUrl = IconName,
            Desc = Desc,
            AttackData = new AttackData()
            {
                bulletSpd = BulletSpd,
                delay = Delay,
                damage = Damage,
                magazine = Magazine
            },
            Selected = -1,
        };
    }
}