using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Weapon.Controller;
using Weapon.Model;

[Serializable]
public class WeaponData
{
    public string weaponName;
    public string bulletName;
    public AttackData baseAttackData;
}