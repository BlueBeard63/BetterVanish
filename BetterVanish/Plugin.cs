using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using BetterVanish.Patches;
using Rocket.Core.Plugins;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace BetterVanish
{
    public class BetterVanishPlugin : RocketPlugin
    {
        private Harmony harmony;
    
        protected override void Load()
        {
            HarmonyExceptionHandler.OnReportCleanupRequested += OnReportCleanupRequested;
            PatchAlerts.OnPlayerAlertRequested += OnPlayerAlertRequested;
            PatchAlerts.OnAlertRequested += OnAlertRequested;
            PatchAlerts.OnZombieAlertRequested += OnZombieAlertRequested;
            PatchAlerts.OnZombieTick += OnZombieTick;

            harmony = new Harmony("com.restoremonarchy.hidevanish");
            harmony.PatchAll();

            Logger.Log($"{Name} {Assembly.GetName().Version.ToString(3)} has been loaded!", ConsoleColor.Yellow);
        }

        protected override void Unload()
        {
            HarmonyExceptionHandler.OnReportCleanupRequested -= OnReportCleanupRequested;
            PatchAlerts.OnPlayerAlertRequested -= OnPlayerAlertRequested;
            PatchAlerts.OnAlertRequested -= OnAlertRequested;
            PatchAlerts.OnZombieAlertRequested -= OnZombieAlertRequested;
            PatchAlerts.OnZombieTick -= OnZombieTick;

            harmony.UnpatchAll(harmony.Id);
        }
        
        private void OnReportCleanupRequested(Type type, Exception exception, MethodBase originalMethod)
        {
            Logger.LogException(exception,
                $"Failed to patch original method {originalMethod?.FullDescription()} from patching type {type.FullDescription()}");
        
            UnloadPlugin();
        }
        
        private static void OnPlayerAlertRequested(Player player, ref bool allow)
        {
            if (IsPlayerVanished(player))
            {
                allow = false;
            }
        }

        private static void OnAlertRequested(Vector3 position, float radius, ref bool allow)
        {
            var players = new List<Player>();
            PlayerTool.getPlayersInRadius(position, 3, players);
            if (players.Count == 0)
            {
                return;
            }
            
            var player = players[0];
            if (IsPlayerVanished(player))
            {
                allow = false;
            }
        }

        private static void OnZombieAlertRequested(Zombie zombie, ref bool allow)
        {
            if (zombie.player == null || !IsPlayerVanished(zombie.player))
            {
                return;
            }
            
            allow = false;        
            zombie.player = null;
        }

        private static void OnZombieTick(Zombie zombie, ref bool allow)
        {
            if (zombie.player == null || !IsPlayerVanished(zombie.player))
            {
                return;
            }
            
            allow = false;        
            zombie.player = null;
        }

        private static bool IsPlayerVanished(Player player)
        {
            var unturnedPlayer = UnturnedPlayer.FromPlayer(player);

            return unturnedPlayer.VanishMode;
        }
    }
}