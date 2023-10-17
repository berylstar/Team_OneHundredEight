using System;
using Unity.VisualScripting;

[Serializable]
public class AttackData
{
    public Define.StatsChangeType statsChangeType;
    public int bulletDamage;
    public float bulletSpeed;
    public int maxMagazine;
    public float shotInterval;
    public float reloadTime;

    //public static AttackData operator +(AttackData a, AttackData b)
    //{
    //    return new AttackData()
    //    {
    //        bulletDamage = a.bulletDamage + b.bulletDamage,
    //        bulletSpeed = a.bulletSpeed + b.bulletSpeed,
    //        maxMagazine = a.maxMagazine + b.maxMagazine,
    //        shotInterval = a.shotInterval + b.shotInterval,
    //        reloadTime = a.reloadTime + b.reloadTime
    //    };
    //}

    //public static AttackData operator *(AttackData a, AttackData b)
    //{
    //    return new AttackData()
    //    {
    //        bulletDamage = a.bulletDamage * b.bulletDamage,
    //        bulletSpeed = a.bulletSpeed * b.bulletSpeed,
    //        maxMagazine = a.maxMagazine * b.maxMagazine,
    //        shotInterval = a.shotInterval * b.shotInterval,
    //        reloadTime = a.reloadTime * b.reloadTime
    //    };
    //}
}