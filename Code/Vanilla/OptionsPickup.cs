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


[MonoDetourTargets(typeof(PickupPickerController))]
internal static class OptionsPickup
{
    private const int createPickupInfoVariableNumber = 1;
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.PickupPickerController.CreatePickup_RoR2_UniquePickup.ILHook(ILHookThing);
    }


    private static void ILHookThing(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.MatchRelaxed(
            x => x.MatchLdloca(createPickupInfoVariableNumber),
            x => x.MatchLdarg(0),
            x => x.MatchLdfld<PickupPickerController>("chestGeneratedFrom"),
            x => x.MatchStfld("RoR2.GenericPickupController/CreatePickupInfo", "chest") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Ldloc, createPickupInfoVariableNumber),
            w.Create(OpCodes.Ldarg_0),
            w.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, PickupPickerController pickupPickerController) =>
            {
                if (pickupPickerController == null || pickupPickerController.gameObject == null)
                {
                    return createPickupInfo;
                }
                createPickupInfo.SetPickupDropSource(pickupPickerController.gameObject);
                return createPickupInfo;
            }),
            w.Create(OpCodes.Stloc, createPickupInfoVariableNumber)
        );
    }
}