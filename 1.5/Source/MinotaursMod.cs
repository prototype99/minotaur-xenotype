using HarmonyLib;
using Verse;

namespace Minotaurs
{
    public class MinotaursMod : Mod
    {
        public MinotaursMod(ModContentPack pack) : base(pack)
        {
			new Harmony("MinotaursMod").PatchAll();
        }
    }

}
