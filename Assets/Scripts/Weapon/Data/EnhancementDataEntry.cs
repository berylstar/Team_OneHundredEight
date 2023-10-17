using Weapon.Model;

namespace Weapon.Data
{
    public class EnhancementDataEntry
    {
        public string Name;
        public string IconName;
        public string Desc;
        public int BulletDamage;
        public float BulletSpeed;
        public int MaxMagazine;
        public float ShotInterval;
        public float ReloadTime;

        public EnhancementData ToEnhancementData() => new EnhancementData()
        {
            Name = Name,
            IconUrl = IconName,
            Desc = Desc,
            AttackData = new AttackData()
            {
                bulletDamage = BulletDamage,
                bulletSpeed = BulletSpeed,
                maxMagazine = MaxMagazine,
                shotInterval = ShotInterval,
                reloadTime = ReloadTime
            }
        };
    }
}