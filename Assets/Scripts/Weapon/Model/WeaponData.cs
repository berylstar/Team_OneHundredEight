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
    public string tooltip;
    public string bulletName;
    public AttackData baseAttackData;
}