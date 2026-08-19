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


[MonoDetourTargets(typeof(EntityStates.Scrapper.ScrappingToIdle))]
internal static class Scrapper
{
    private const int createPickupInfoVariableNumber = 2;
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.EntityStates.Scrapper.ScrappingToIdle.OnEnter.ILHook(DroppingScrap);
    }


    private static void DroppingScrap(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.MatchRelaxed(
            x => x.MatchLdloca(createPickupInfoVariableNumber),
            x => x.MatchLdloc(0),
            x => x.MatchCallOrCallvirt("RoR2.GenericPickupController/CreatePickupInfo", "set_pickup") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Ldloc, createPickupInfoVariableNumber),
            w.Create(OpCodes.Ldarg_0),
            w.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, EntityStates.Scrapper.ScrappingToIdle scrappingToIdle) =>
            {
                if (scrappingToIdle == null || scrappingToIdle.gameObject == null)
                {
                    return createPickupInfo;
                }
                createPickupInfo.SetPickupDropSource(scrappingToIdle.gameObject);
                return createPickupInfo;
            }),
            w.Create(OpCodes.Stloc, createPickupInfoVariableNumber)
        );
    }
}