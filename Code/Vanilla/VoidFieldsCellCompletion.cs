using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace DropSourceForItems.Vanilla;


[MonoDetourTargets(typeof(RoR2.ArenaMissionController))]
internal static class VoidFieldsCellCompletion
{
    private const int createPickupInfoVariableNumber = 13;
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.ArenaMissionController.EndRound.ILHook(EndRound);
    }


    private static void EndRound(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.MatchRelaxed(
            x => x.MatchLdloca(createPickupInfoVariableNumber),
            x => x.MatchLdarg(0),
            x => x.MatchLdfld<ArenaMissionController>("pickupPrefab"),
            x => x.MatchStfld("RoR2.GenericPickupController/CreatePickupInfo", "prefabOverride") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Ldloc, createPickupInfoVariableNumber),
            w.Create(OpCodes.Ldarg_0),
            w.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, ArenaMissionController arenaMissionController) =>
            {
                if (arenaMissionController == null || arenaMissionController.rewardSpawnPosition == null)
                {
                    return createPickupInfo;
                }
                // reward spawn position is 2 under its void cell object
                GameObject voidCell = arenaMissionController.rewardSpawnPosition.transform?.parent?.parent?.gameObject;
                if (voidCell == null)
                {
                    return createPickupInfo;
                }
                createPickupInfo.SetPickupDropSource(voidCell);
                return createPickupInfo;
            }),
            w.Create(OpCodes.Stloc, createPickupInfoVariableNumber)
        );
    }
}