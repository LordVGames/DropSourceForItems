using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
namespace DropSourceForItems.Vanilla;


[MonoDetourTargets(typeof(OptionChestBehavior))]
internal static class VoidPotential
{
    private const int createPickupInfoVariableNumber = 0;
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.OptionChestBehavior.ItemDrop.ILHook(ItemDrop);
    }


    private static void ItemDrop(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.MatchRelaxed(
            x => x.MatchLdloca(createPickupInfoVariableNumber),
            x => x.MatchLdarg(0),
            x => x.MatchLdfld<OptionChestBehavior>("pickupPrefab"),
            x => x.MatchStfld("RoR2.GenericPickupController/CreatePickupInfo", "prefabOverride") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Ldloc, createPickupInfoVariableNumber),
            w.Create(OpCodes.Ldarg_0),
            w.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, OptionChestBehavior optionChestBehavior) =>
            {
                if (optionChestBehavior == null || optionChestBehavior.gameObject == null)
                {
                    return createPickupInfo;
                }
                createPickupInfo.SetPickupDropSource(optionChestBehavior.gameObject);
                return createPickupInfo;
            }),
            w.Create(OpCodes.Stloc, createPickupInfoVariableNumber)
        );
    }
}