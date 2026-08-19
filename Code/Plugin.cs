using BepInEx;
using MonoDetour;
using RoR2;
namespace DropSourceForItems;


[BepInAutoPlugin]
[BepInDependency(EnemiesReturns.EnemiesReturnsPlugin.GUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(Modded.ModSupport.RiskyTweaksMod.GUID, BepInDependency.DependencyFlags.SoftDependency)]
public partial class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        ConfigOptions.BindConfigOptions(Config);
        Log.Init(Logger);
        MonoDetourManager.InvokeHookInitializers(typeof(Plugin).Assembly, false);
    }
}