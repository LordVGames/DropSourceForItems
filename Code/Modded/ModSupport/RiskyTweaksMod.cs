using System;
using System.Collections.Generic;
using System.Text;
namespace DropSourceForItems.Modded.ModSupport;


internal static class RiskyTweaksMod
{
    internal const string GUID = "com.Moffein.RiskyTweaks";
    private static bool? _modexists;
    internal static bool ModIsRunning
    {
        get
        {
            _modexists ??= BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(GUID);
            return (bool)_modexists;
        }
    }
}