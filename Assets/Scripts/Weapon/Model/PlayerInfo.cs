using UnityEngine;

namespace Weapon.Model
{
    public class PlayerInfo
    {
        public PlayerInfo(string nickname, string characterImage, WeaponData weaponData)
        {
            Nickname = nickname;
            CharacterImage = characterImage;
            WeaponData = weaponData;
        }

        public string Nickname { get; }
        public string CharacterImage { get; }
        public WeaponData WeaponData { get; }
    }
}