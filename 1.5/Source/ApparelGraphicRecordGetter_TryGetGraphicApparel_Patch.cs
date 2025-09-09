using HarmonyLib;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using UnityEngine;
using Verse;

namespace Minotaurs
{
    [HarmonyPatch(typeof(ApparelGraphicRecordGetter), "TryGetGraphicApparel")]
    public static class ApparelGraphicRecordGetter_TryGetGraphicApparel_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            var codes = codeInstructions.ToList();
            for (int i = 0; i < codes.Count; i++)
            {
                yield return codes[i];
                if (codes[i].opcode == OpCodes.Stloc_0)
                {
                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, 
                        AccessTools.Method(typeof(ApparelGraphicRecordGetter_TryGetGraphicApparel_Patch), nameof(TryChangePath)));
                    yield return new CodeInstruction(OpCodes.Stloc_0);
                }
            }
        }

        public static string TryChangePath(string path, Apparel apparel)
        {
            if (PawnApparelGenerator.IsHeadgear(apparel.def))
            {
                var pawn = apparel.Wearer;
                if (pawn != null)
                {
                    var head = pawn.story.headType;
                    var headExtension = head.GetModExtension<HeadExtension>();
                    if (headExtension != null && headExtension.headgearPathPrefix.NullOrEmpty() is false)
                    {
                        var tempPath = headExtension.headgearPathPrefix + path;
                        if (ContentFinder<Texture2D>.Get(tempPath + "_south", reportFailure: false) != null)
                        {
                            return tempPath;
                        }
                    }
                }
            }
            return path;
        }
    }

}
