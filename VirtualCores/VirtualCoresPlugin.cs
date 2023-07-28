using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using VirtualCores.Patches;
using HarmonyLib;
using I2.Loc.SimpleJSON;
using UnityEngine;

namespace VirtualCores
{
    [BepInPlugin(MyGUID, PluginName, VersionString)]
    public class VirtualCoresPlugin : BaseUnityPlugin
    {
        private const string MyGUID = "com.equinox.VirtualCores";
        private const string PluginName = "VirtualCores";
        private const string VersionString = "1.0.0";

        private static readonly Harmony Harmony = new Harmony(MyGUID);
        public static ManualLogSource Log = new ManualLogSource(PluginName);

        private static string saveDataFolder = Application.persistentDataPath + "/VirtualCores";
        public static Dictionary<ResearchCoreDefinition.CoreType, int> virtualCoreCounts = new Dictionary<ResearchCoreDefinition.CoreType, int>();

        private void Awake() {
           
            // Apply all of our patches
            Logger.LogInfo("PluginName: " + PluginName + ", VersionString: " + VersionString + " is loading...");
            Harmony.PatchAll();
            Logger.LogInfo("PluginName: " + PluginName + ", VersionString: " + VersionString + " is loaded.");
            Log = Logger;

            foreach(ResearchCoreDefinition.CoreType coreType in Enum.GetValues(typeof(ResearchCoreDefinition.CoreType))) {
                virtualCoreCounts.Add(coreType, 0);
            }

            Directory.CreateDirectory(saveDataFolder);
            Harmony.CreateAndPatchAll(typeof(MemoryTreePatch));
            Harmony.CreateAndPatchAll(typeof(TechTreeUIPatch));
            Harmony.CreateAndPatchAll(typeof(TechTreeStatePatch));
            Harmony.CreateAndPatchAll(typeof(SaveStatePatch));
        }

        public static void saveData(string worldName) {
            string filePath = saveDataFolder + "/" + worldName + ".txt";
            string amounts = string.Join(",", virtualCoreCounts.Values);
            File.WriteAllText(filePath, amounts);
        }

        public static void loadData(string worldName) {
            string filePath = saveDataFolder + "/" + worldName + ".txt";
            if (File.Exists(filePath)) {
                string amounts = File.ReadAllText(filePath);
                string[] amountsSplit = amounts.Split(',');
                for(int i = 0; i < amountsSplit.Length; i++) {
                    ResearchCoreDefinition.CoreType key = virtualCoreCounts.Keys.ToList()[i];
                    virtualCoreCounts[key] = int.Parse(amountsSplit[i]);
                }
            }
        }
    }
}
