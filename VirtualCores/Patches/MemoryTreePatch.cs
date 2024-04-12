using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace VirtualCores.Patches
{
    public class MemoryTreePatch
    {
        [HarmonyPatch(typeof(MemoryTreeMachineList), "UpdateAll")]
        [HarmonyPostfix]
        static void addVirtualCore(MemoryTreeMachineList __instance) {
            for(int i = 0; i < __instance.curCount; i++) {
                int index, num;
                ResearchCoreDefinition researchCoreDefinition;
                if (__instance.myArray[i].pendingCoreCount == 0 &&
                    __instance.myArray[i].GetErrorState() == MemoryTreeInstance.ErrorState.Full &&
                    __instance.myArray[i].FindCoreInInventory(out index, out researchCoreDefinition)) {
                    ++VirtualCoresPlugin.virtualCoreCounts[researchCoreDefinition.coreType];
                    __instance.myArray[i].GetInputInventory().RemoveResourcesFromSlot(index, out num, 1, false);
                }
            }
        }
    }
}
