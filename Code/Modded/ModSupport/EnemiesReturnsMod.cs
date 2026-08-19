using System;
using System.Collections.Generic;
using System.Text;
namespace DropSourceForItems.Modded.ModSupport;


internal static class EnemiesReturnsMod
{
    private static bool? _modexists;
    internal static bool ModIsRunning
    {
        get
        {
            _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(EnemiesReturns.EnemiesReturnsPlugin.GUID);
            return (bool)_modexists;
        }
    }
}