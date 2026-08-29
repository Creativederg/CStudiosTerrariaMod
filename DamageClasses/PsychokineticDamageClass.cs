using Terraria;
using Terraria.ModLoader;

namespace CStudios.Content.DamageClasses
{
    public class PsychokineticDamageClass : DamageClass
    {
        public override StatInheritanceData GetModifierInheritance(DamageClass damageClass)
        {
            if (damageClass == DamageClass.Magic || damageClass == DamageClass.Summon)
                return StatInheritanceData.Full;

            return StatInheritanceData.None;
        }

        public override bool GetEffectInheritance(DamageClass damageClass)
        {
            return damageClass == DamageClass.Magic || damageClass == DamageClass.Summon;
        }

        public override bool UseStandardCritCalcs => true;
    }
}