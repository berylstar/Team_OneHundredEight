namespace Weapon.Model
{
    public class RoundPlayerInfoState
    {
        public int PlayerIndex { get; }
        public string Name { get; }
        public string IconUrl { get; }
        public EnhancementData SelectedEnhancement { get; }

        public RoundPlayerInfoState(string name, int playerIndex, string iconUrl, EnhancementData selectedEnhancement)
        {
            Name = name;
            PlayerIndex = playerIndex;
            IconUrl = iconUrl;
            SelectedEnhancement = selectedEnhancement;
        }
        
    }
}