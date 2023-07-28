using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualCores.Patches
{
    public class TechTreeUIPatch
    {
        [HarmonyPatch(typeof(TechTreeUI), "RefreshCores")]
        [HarmonyPostfix]
        static void showVirtualCores(TechTreeUI __instance) {
            List<CoreNeedCard> cards = getCoreNeedCards(__instance);
            List<ResearchCoreDefinition.CoreType> coreTypesToDisplay = getCoreTypesToDisplay(__instance);

            if(cards.Count == 0 || coreTypesToDisplay.Count == 0) {
                return;
            }

            List<int> totalResearchCores = TechTreeState.instance.totalResearchCores;
            List<int> usedResearchCores = TechTreeState.instance.usedResearchCores;
            for (int i = 0; i < cards.Count; i++) {
                CoreNeedCard coreNeedCard = cards[i];
                int coreTypeIndex = (int)coreTypesToDisplay[i];
                int totalResearchCoresOfType = 0;
                if (coreTypeIndex < totalResearchCores.Count) {
                    totalResearchCoresOfType = totalResearchCores[coreTypeIndex];
                }
                int availableResearchCoresOfType = ((float)totalResearchCoresOfType * TechTreeState.coreEffiencyMultipliers[coreTypeIndex]).FloorToInt();
                if (coreTypeIndex < usedResearchCores.Count) {
                    availableResearchCoresOfType -= usedResearchCores[coreTypeIndex];
                }
                availableResearchCoresOfType += VirtualCoresPlugin.virtualCoreCounts[(ResearchCoreDefinition.CoreType)coreTypeIndex];
                availableResearchCoresOfType = Mathf.Max(0, availableResearchCoresOfType);
                coreNeedCard.SetAvailableOutOfTotal(coreTypesToDisplay[i], availableResearchCoresOfType, totalResearchCoresOfType);
            }
        }

        private static List<CoreNeedCard> getCoreNeedCards(TechTreeUI __instance) {
            List<CoreNeedCard> cards = new List<CoreNeedCard>();
            FieldInfo activeCoreCardsInfo = typeof(TechTreeUI).GetField("activeCoreCards", BindingFlags.NonPublic | BindingFlags.Instance);
            if (activeCoreCardsInfo != null) {
                cards = (List<CoreNeedCard>)activeCoreCardsInfo.GetValue(__instance);
            }
            else {
                Debug.Log("Couldn't access activeCoreCards");
            }

            return cards;
        }

        private static List<ResearchCoreDefinition.CoreType> getCoreTypesToDisplay(TechTreeUI __instance) {
            List<ResearchCoreDefinition.CoreType> coreTypesToDisplay = new List<ResearchCoreDefinition.CoreType>();
            FieldInfo coreTypesToDisplayInfo = typeof(TechTreeUI).GetField("coreTypesToDisplay", BindingFlags.NonPublic | BindingFlags.Instance);
            if (coreTypesToDisplayInfo != null) {
                coreTypesToDisplay = (List<ResearchCoreDefinition.CoreType>)coreTypesToDisplayInfo.GetValue(__instance);
            }
            else {
                Debug.Log("Couldn't acess coreTypesToDisplay");
            }

            return coreTypesToDisplay;
        }
    }
}
