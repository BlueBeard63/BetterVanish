using System;
using System.Reflection;
using HarmonyLib;
using SDG.Unturned;
using UnityEngine;
using RocketLogger = Rocket.Core.Logging.Logger;

namespace BetterVanish.Patches
{
    [HarmonyPatch]
    internal static class PatchAlerts
    {
        public delegate void PlayerAlertRequest(Player player, ref bool allow);
        public static event PlayerAlertRequest OnPlayerAlertRequested;

        public delegate void AlertRequested(Vector3 position, float radius, ref bool allow);
        public static event AlertRequested OnAlertRequested;

        public delegate void ZombieAlert(Zombie zombie, ref bool allow);
        public static event ZombieAlert OnZombieAlertRequested;

        public delegate void ZombieTick(Zombie zombie, ref bool allow);
        public static event ZombieTick OnZombieTick;

        [HarmonyCleanup]
        private static Exception Cleanup(Exception ex, MethodBase original)
        {
            HarmonyExceptionHandler.ReportCleanupException(typeof(PatchAlerts), ex, original);
            return null;
        }
        [HarmonyPatch(typeof(AlertTool), nameof(AlertTool.alert), typeof(Player), typeof(Vector3), typeof(float), typeof(bool), typeof(Vector3), typeof(bool))]
        [HarmonyPrefix]
        private static bool PatchPlayerAlert(Player player, Vector3 position, float radius, bool sneak, Vector3 spotDir, bool isSpotOn)
        {
            try
            {
                var allow = true;
                OnPlayerAlertRequested?.Invoke(player, ref allow);
                return allow;
            }
            catch (Exception ex)
            {
                RocketLogger.LogException(ex, $"An error occured while firing \"{nameof(OnPlayerAlertRequested)}\"");
                return true;
            }
        }
        [HarmonyPatch(typeof(AlertTool), nameof(AlertTool.alert), typeof(Vector3), typeof(float))]
        [HarmonyPrefix]
        private static bool PatchAlert(Vector3 position, float radius)
        {
            try
            {
                var allow = true;
                OnAlertRequested?.Invoke(position, radius, ref allow);
                return allow;
            }
            catch (Exception ex)
            {
                RocketLogger.LogException(ex, $"An error occured while firing \"{nameof(OnAlertRequested)}\"");
                return true;
            }
        }
        [HarmonyPatch(typeof(Zombie), nameof(Zombie.alert), typeof(Vector3), typeof(bool))]
        [HarmonyPrefix]
        private static bool PatchZombieAlert(Zombie __instance, Vector3 newPosition, bool isStartling)
        {
            if (__instance.player == null)
            {
                return true;
            }
            
            var allow = true;
            try
            {
                OnZombieAlertRequested?.Invoke(__instance, ref allow);
                return allow;
            }
            catch (Exception ex)
            {
                RocketLogger.LogException(ex, $"An error occured while firing {nameof(OnZombieAlertRequested)}");
                return true;
            }
        }
        [HarmonyPatch(typeof(Zombie), nameof(Zombie.tick))]
        [HarmonyPrefix]
        private static bool PatchZombieTick(Zombie __instance)
        {
            try
            {
                var allow = true;
                OnZombieTick?.Invoke(__instance, ref allow);
                return allow;
            }
            catch (Exception ex)
            {
                RocketLogger.LogException(ex, $"An error occured while firing \"{nameof(OnZombieTick)}\"");
                return true;
            }
        }
    }
}