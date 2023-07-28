using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace VirtualCores.Patches
{
    public class TechTreeStatePatch
    {
        [HarmonyPatch(typeof(TechTreeState), "NumCoresAvailable")]
        [HarmonyPrefix]
        static bool addNumVirtualCores(TechTreeState __instance, ResearchCoreDefinition.CoreType type, ref int __result) {
            while(__instance.usedResearchCores.Count <= (int)type) {
                __instance.usedResearchCores.Add(0);
            }
            __result = ((float)__instance.NumCoresOfTypePlaced((int)type) 
                 * TechTreeState.coreEffiencyMultipliers[(int)type]).FloorToInt()
                 - __instance.usedResearchCores[(int)type]
                 + VirtualCoresPlugin.virtualCoreCounts[type];
            return false;
        }
    }
}
