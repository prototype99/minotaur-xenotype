using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace Minotaurs
{
    [HarmonyPatch(typeof(BeardDef), "GraphicFor")]
    public static class BeardDef_GraphicFor_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            var texPathField = AccessTools.Field(typeof(StyleItemDef), nameof(StyleItemDef.texPath));
            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (codes[i].LoadsField(texPathField))
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(BeardDef_GraphicFor_Patch), nameof(TryChangePath)));
                }
            }
        }

        public static string TryChangePath(string path, Pawn pawn)
        {
            var head = pawn.story.headType;
            var headExtension = head.GetModExtension<HeadExtension>();
            if (headExtension != null && headExtension.beardPathPrefix.NullOrEmpty() is false)
            {
                var tempPath = headExtension.beardPathPrefix + path;
                if (ContentFinder<Texture2D>.Get(tempPath + "_south", reportFailure: false) != null)
                {
                    return tempPath;
                }
            }
            return path;
        }
    }

}
