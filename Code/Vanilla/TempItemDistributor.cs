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


[MonoDetourTargets(typeof(RoR2.PickupDistributorBehavior))]
internal static class TempItemDistributor
{
    private const int createPickupInfoVariableNumber = 4;
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.PickupDistributorBehavior.Drop.ILHook(TempItemDrop);
    }


    private static void TempItemDrop(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.MatchRelaxed(
            x => x.MatchLdloca(createPickupInfoVariableNumber),
            x => x.MatchLdloc(0),
            x => x.MatchCallOrCallvirt<Transform>("get_position"),
            x => x.MatchStfld("RoR2.GenericPickupController/CreatePickupInfo", "position") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Ldloc, createPickupInfoVariableNumber),
            w.Create(OpCodes.Ldarg_0),
            w.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, PickupDistributorBehavior pickupDistributorBehavior) =>
            {
                if (pickupDistributorBehavior == null || pickupDistributorBehavior.gameObject == null)
                {
                    return createPickupInfo;
                }
                createPickupInfo.SetPickupDropSource(pickupDistributorBehavior.gameObject);
                return createPickupInfo;
            }),
            w.Create(OpCodes.Stloc, createPickupInfoVariableNumber)
        );
    }
}