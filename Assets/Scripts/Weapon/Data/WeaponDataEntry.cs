namespace Weapon.Data
{
    public class WeaponDataEntry
    {
        public string Name;
        public string Desc;
        public string ImageUrl;
        public string BulletName;
        public int Damage;
        public float BulletSpd;
        public int MaxMagazine;
        public float Delay;
        public float ReloadTime;

        public WeaponDataEntry(string name,
            string desc,
            string imageUrl,
            string bulletName,
            int damage,
            float bulletSpd,
            int maxMagazine,
            float delay,
            float reloadTime)
        {
            Name = name;
            Desc = desc;
            ImageUrl = imageUrl;
            BulletName = bulletName;
            Damage = damage;
            BulletSpd = bulletSpd;
            MaxMagazine = maxMagazine;
            Delay = delay;
            ReloadTime = reloadTime;
        }

        public WeaponDataEntry() { }

        public WeaponData ToWeaponData()
        {
            return new WeaponData()
            {
                weaponName = Name,
                spriteName = ImageUrl,
                bulletName = BulletName,
                tooltip = Desc,
                baseAttackData = new AttackData()
                {
                    bulletDamage = Damage,
                    bulletSpeed = BulletSpd,
                    maxMagazine = MaxMagazine,
                    reloadTime = ReloadTime,
                    shotInterval = Delay,
                    statsChangeType = Define.StatsChangeType.Override
                },
            };
        }
    }
}