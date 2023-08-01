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

        [HarmonyPatch(typeof(TechTreeState), "HandleEndOfFrame")]
        [HarmonyPostfix]
        static void addVirtualCoresToClusters(TechTreeState __instance) {
            __instance.freeCores = 0;
            int num = Math.Max(__instance.totalResearchCores.Count, __instance.usedResearchCores.Count);
            for (int i = 0; i < num; i++) {
                int num2 = __instance.NumCoresOfTypePlaced(i) + VirtualCoresPlugin.virtualCoreCounts[(ResearchCoreDefinition.CoreType)i];
                if (__instance.usedResearchCores.Count > i) {
                    num2 -= __instance.usedResearchCores[i];
                }
                if (num2 > 0) {
                    int num3 = GameDefines.instance.coreClusterSizes[i];
                    if (num3 >= 0) {
                        __instance.freeCores += num2 * num3;
                    }
                    else {
                        __instance.freeCores += num2 / -num3;
                    }
                }
            }
            ref UpgradeState[] ptr = ref GameState.instance.upgradeStates;
            __instance.freeCoresAssembling = (float)__instance.freeCores * ptr[22].floatVal * 0.01f;
            __instance.freeCoresMining = (float)__instance.freeCores * ptr[19].floatVal * 0.01f;
            __instance.freeCoresPowerOutput = (float)__instance.freeCores * ptr[23].floatVal * 0.01f;
            __instance.freeCoresSmelting = (float)__instance.freeCores * ptr[20].floatVal * 0.01f;
            __instance.freeCoresThreshing = (float)__instance.freeCores * ptr[21].floatVal * 0.01f;
        }
    }
}
