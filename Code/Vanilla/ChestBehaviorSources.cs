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


[MonoDetourTargets(typeof(ChestBehavior))]
internal static class ChestBehaviorSources
{
    private const int createPickupInfoVariableNumber = 4;
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.ChestBehavior.BaseItemDrop.ILHook(ChestItemDrop);
    }


    private static void ChestItemDrop(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.MatchRelaxed(
            x => x.MatchLdloca(createPickupInfoVariableNumber),
            x => x.MatchLdarg(0),
            x => x.MatchStfld("RoR2.GenericPickupController/CreatePickupInfo", "chest") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Ldloc, createPickupInfoVariableNumber),
            w.Create(OpCodes.Ldarg_0),
            w.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, ChestBehavior chestBehavior) =>
            {
                if (chestBehavior == null || chestBehavior.gameObject == null)
                {
                    return createPickupInfo;
                }
                createPickupInfo.SetPickupDropSource(chestBehavior.gameObject);
                return createPickupInfo;
            }),
            w.Create(OpCodes.Stloc, createPickupInfoVariableNumber)
        );
    }
}