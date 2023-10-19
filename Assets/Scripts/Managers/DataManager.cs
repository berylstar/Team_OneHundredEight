using Common;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Weapon.Data;

namespace Managers
{
    public class DataManager
    {
        private static DataManager _instance;

        public static DataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataManager();
                }

                return _instance;
            }
        }

        private List<WeaponData> _weaponDataList;

        public IReadOnlyList<WeaponData> WeaponDataList
        {
            get
            {
                if (_weaponDataList == null)
                {
                    _weaponDataList =
                        CsvReader.ReadCsvFromResources<WeaponDataEntry>(Constants.FilePath.WEAPON_CSV_FILE, 1)
                            .Select(it => it.ToWeaponData())
                            .ToList();
                }

                return _weaponDataList;
            }
        }

        public readonly string[] CharacterImageUrlArray = new[]
        {
            "PlayerImage/Mouse", "PlayerImage/Human", "PlayerImage/Skeleton", "PlayerImage/Slime", "PlayerImage/stone"
        };
    }
}