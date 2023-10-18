using UnityEngine;

namespace Weapon.Model
{
    public class PlayerStatus
    {
        public PlayerStatus(string nickname, string characterImage, WeaponData weaponData)
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