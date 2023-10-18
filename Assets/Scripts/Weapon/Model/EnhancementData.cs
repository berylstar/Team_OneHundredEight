namespace Weapon.Model
{
    public class EnhancementData
    {
        public string Name;
        public string IconUrl;
        public string Desc;
        public AttackData AttackData;

        public override string ToString() => $"name:{Name}, IconUrl : {IconUrl}, Desc:{Desc}, AttackData : {AttackData}";
    }
}