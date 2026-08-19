using System;
using System.Collections.Generic;
using System.Text;
using BepInEx.Configuration;
using MiscFixes.Modules;
namespace DropSourceForItems;


public static class ConfigOptions
{
    private const string _sectionName = "when the config has options";
    internal static ConfigEntry<bool> ShowDebugLogging;
        

    internal static void BindConfigOptions(ConfigFile config)
    {
        ShowDebugLogging = config.BindOption(
            _sectionName,
            "Show debug logging",
            "does what it says",
            false
        );
    }
}