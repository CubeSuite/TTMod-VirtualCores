using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;

namespace VirtualCores.Patches
{
    public class SaveStatePatch 
    {
        [HarmonyPatch(typeof(SaveState), "LoadFileData", typeof(SaveState.SaveMetadata), typeof(string))]
        [HarmonyPostfix]
        static void loadMod(SaveState __instance, SaveState.SaveMetadata saveMetadata, string replayLocation) {
            VirtualCoresPlugin.loadData(saveMetadata.worldName);
        }

        [HarmonyPatch(typeof(SaveState), "SaveToFile")]
        [HarmonyPostfix]
        static void saveMod(SaveState __instance, string saveLocation, bool saveToPersistent = true) {
            VirtualCoresPlugin.saveData(__instance.metadata.worldName);
        }
    }
}
