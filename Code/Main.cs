using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using RoR2BepInExPack.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
namespace DropSourceForItems;


[MonoDetourTargets(typeof(GenericPickupController))]
[MonoDetourTargets(typeof(PickupDropletController))]
public static class Main
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.GenericPickupController.CreatePickup.ILHook(TransferDropSourceToPickup);
        Mdh.RoR2.PickupDropletController.CreateCommandCube.ILHook(TransferDropSourceToCommandPickup);
    }


    private static void TransferDropSourceToCommandPickup(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.DeclareVariable(typeof(PickupPickerController), out var ppcVariable);


        w.MatchRelaxed(
            x => x.MatchDup(),
            x => x.MatchCallOrCallvirt<GameObject>("GetComponent") && w.SetCurrentTo(x),
            x => x.MatchDup()
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Stloc, ppcVariable),
            w.Create(OpCodes.Ldloc, ppcVariable)
        );


        w.MatchRelaxed(
            x => x.MatchStfld<PickupPickerController>("chestGeneratedFrom") && w.SetCurrentTo(x),
            x => x.MatchCallOrCallvirt<NetworkServer>("Spawn")
        ).ThrowIfFailure()
        .InsertBeforeCurrentStealLabels(
            w.Create(OpCodes.Ldarg_0), // PickupDropletController
            w.Create(OpCodes.Ldloc, ppcVariable), // PickupPickerController
            w.CreateDelegateCall((PickupDropletController pdc, PickupPickerController ppc) =>
            {
                Log.Debug("AfterPickupMadeCommand");
                if (pdc == null || ppc == null)
                {
                    return;
                }
                if (pdc.pickupState == null)
                {
                    Log.Debug("Command pickupState was null!");
                    return;
                }
                GameObject dropSource = pdc.createPickupInfo.GetPickupDropSource();
                if (dropSource == null)
                {
                    Log.Debug($"Drop source on createPickupInfo for {pdc.createPickupInfo.pickup} was null!");
                    return;
                }
                Log.Debug($"Drop source on createPickupInfo for {pdc.createPickupInfo.pickup} is {dropSource}");
                ppc.SetPickupDropSource(pdc.createPickupInfo.GetPickupDropSource());
            })
        );
    }


    private static void TransferDropSourceToPickup(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.MatchRelaxed(
            x => x.MatchLdloc(0) && w.SetCurrentTo(x),
            x => x.MatchCallOrCallvirt<NetworkServer>("Spawn")
        ).ThrowIfFailure()
        .InsertBeforeCurrentStealLabels(
            w.Create(OpCodes.Ldarg_0), // GenericPickupController.CreatePickupInfo
            w.Create(OpCodes.Ldobj, typeof(GenericPickupController.CreatePickupInfo)), // i dont get why this is needed but whatever
            w.Create(OpCodes.Ldloc_1), // GenericPickupController
            w.Create(OpCodes.Ldloc_3), // PickupPickerController
            w.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, GenericPickupController gpc, PickupPickerController ppc) =>
            {
                Log.Debug("AfterPickupMade");
                if (createPickupInfo.pickup == null)
                {
                    Log.Debug("Pickup in createPickupInfo was null!");
                    return;
                }
                GameObject dropSource = createPickupInfo.GetPickupDropSource();
                if (dropSource == null)
                {
                    Log.Debug($"Drop source on createPickupInfo for {createPickupInfo.pickup} was null!");
                    return;
                }
                Log.Debug($"Drop source on createPickupInfo for {createPickupInfo.pickup} is {dropSource}");


                if (gpc != null)
                {
                    Log.Debug("Setting drop source on GenericPickupController");
                    gpc.SetPickupDropSource(dropSource);
                }
                else if (ppc != null)
                {
                    Log.Debug("Setting drop source on PickupPickerController");
                    ppc.SetPickupDropSource(dropSource);
                }
                else
                {
                    Log.Error("Both GenericPickupController AND PickupPickerController were null when trying to transfer drop source???");
                }
            })
        );
    }
}