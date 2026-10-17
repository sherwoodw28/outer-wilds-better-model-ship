using HarmonyLib;
using UnityEngine;

namespace BetterModelShip
{
    [HarmonyPatch]
    public static class RemoteFlightConsolePatches
    {


        //Issues were caused with the player gravity as the player was being temporarily disabled with the Model Ship during this flow.
        //Essentially brings the destruction handling for the model ship forward a little, meaning player is still active when it occurs.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DestructionVolume), nameof(DestructionVolume.VanishModelRocketShip))]
        public static void ExitBeforeModelShipDestruction(OWRigidbody __0)
        {
            if (!BetterModelShip.IsPilotingModelShip || __0 != BetterModelShip.ModelShipBody)
            {
                return;
            }

            RemoteFlightConsole console = Object.FindObjectOfType<RemoteFlightConsole>();

            if (console != null && console._modelShipBody == __0)
            {
                console.OnModelRocketShipDestroyed();
            }
        }

        //Prevents issues for repeated calls to that OnDestroyed function.
        [HarmonyPrefix]
        [HarmonyPatch(typeof(RemoteFlightConsole), nameof(RemoteFlightConsole.OnModelRocketShipDestroyed))]
        public static bool PreventRepeatedDestructionExit(RemoteFlightConsole __instance)
        {
            return __instance._modelShipBody != null;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(RemoteFlightConsole), nameof(RemoteFlightConsole.Update))]
        public static void RemoteFlightConsole_Update(RemoteFlightConsole __instance)
        {
            __instance._verticalThrustPrompt.SetVisibility(false);
            __instance._horizontalThrustPrompt.SetVisibility(false);
        }
    }
}
