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


[MonoDetourTargets(typeof(RoR2.HalcyoniteShrineInteractable))]
internal static class HalcyonShrine
{
    private const int createPickupInfoVariableNumber = 9;
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.HalcyoniteShrineInteractable.DropRewards.ILHook(DropRewards);
    }


    private static void DropRewards(ILManipulationInfo info)
    {
        ILWeaver w = new(info);


        w.MatchMultipleRelaxed(
            onMatch: w2 =>
            {
                w2.InsertAfterCurrent(
                    w2.Create(OpCodes.Ldloc, createPickupInfoVariableNumber),
                    w2.Create(OpCodes.Ldarg_0),
                    w2.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, HalcyoniteShrineInteractable halcyoniteShrineInteractable) =>
                    {
                        if (halcyoniteShrineInteractable == null || halcyoniteShrineInteractable.gameObject == null)
                        {
                            return createPickupInfo;
                        }
                        createPickupInfo.SetPickupDropSource(halcyoniteShrineInteractable.gameObject);
                        return createPickupInfo;
                    }),
                    w2.Create(OpCodes.Stloc, createPickupInfoVariableNumber)
                );
            },
            x => x.MatchLdloca(createPickupInfoVariableNumber),
            x => x.MatchLdloc(4),
            x => x.MatchStfld("RoR2.GenericPickupController/CreatePickupInfo", "position") && w.SetCurrentTo(x)
        ).ThrowIfFailure();
    }
}