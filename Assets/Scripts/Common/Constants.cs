using System.Collections.Generic;
using UnityEngine;
using static System.IO.Path;

namespace Common
{
    public static class Constants
    {
        public static class Min
        {
            public const int MinDamage = 1;
            public const float MinBulletSpd = 1f;
            public const int MinMagazine = 1;
            public const float MinAttackDelay = 0.2f;
            public const float MinReloadTime = 0.1f;
        }

        public static class FilePath
        {
            private const string EnhancementCsvFile = "enhancement_dataset";
            private const string WeaponCsvFile = "108_dataset - weapon";
            public static string ENHANCEMENT_CSV_FILE => $"data{DirectorySeparatorChar}{EnhancementCsvFile}";
            public static string WEAPON_CSV_FILE => $"data{DirectorySeparatorChar}{WeaponCsvFile}";
        }

        public const float TimeToNextRound = 5f;
        public const float SelectionTime = 20;

        public static List<Color> PlayerColors { get; } = new List<Color>()
        {
            new Color(Color.blue.r, Color.blue.g, Color.blue.b, 0.6f),
            new Color(Color.green.r, Color.green.g, Color.green.b, 0.6f),
            new Color(Color.red.r, Color.red.g, Color.red.b, 0.6f),
            new Color(Color.magenta.r, Color.magenta.g, Color.magenta.b, 0.6f),
            new Color(Color.cyan.r, Color.cyan.g, Color.cyan.b, 0.6f)
        };
    }
}