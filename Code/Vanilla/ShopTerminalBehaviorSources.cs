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


[MonoDetourTargets(typeof(RoR2.ShopTerminalBehavior))]
internal static class ShopTerminalBehaviorSources
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.ShopTerminalBehavior.DropPickup_System_Boolean.ILHook(ShopTerminalDrop);
    }


    private static void ShopTerminalDrop(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.DeclareVariable(typeof(bool), out var isDuplicatedVariable);
        w.DeclareVariable(typeof(Vector3), out var velocityVariable);
        w.DeclareVariable(typeof(Vector3), out var positionVariable);
        w.DeclareVariable(typeof(UniquePickup), out var pickupVariable);


        w.MatchRelaxed(
            x => x.MatchCallOrCallvirt<PickupDropletController>("CreatePickupDroplet") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertBeforeCurrent(
            w.Create(OpCodes.Stloc, isDuplicatedVariable),
            w.Create(OpCodes.Stloc, velocityVariable),
            w.Create(OpCodes.Stloc, positionVariable),
            w.Create(OpCodes.Stloc, pickupVariable),
            w.Create(OpCodes.Ldarg_0),
            w.Create(OpCodes.Ldloc, pickupVariable),
            w.Create(OpCodes.Ldloc, positionVariable),
            w.Create(OpCodes.Ldloc, velocityVariable),
            w.Create(OpCodes.Ldloc, isDuplicatedVariable),
            w.CreateDelegateCall((ShopTerminalBehavior shopTerminalBehavior, UniquePickup pickup, Vector3 position, Vector3 velocity, bool isDuplicated) =>
            {
                var pickupInfo = new GenericPickupController.CreatePickupInfo
                {
                    rotation = Quaternion.identity,
                    pickup = pickup,
                    position = position,
                    duplicated = isDuplicated,
                    recycled = false,
                };
                pickupInfo.SetPickupDropSource(shopTerminalBehavior.gameObject);
                PickupDropletController.CreatePickupDroplet(pickupInfo, position, velocity);
            }),
            w.Create(OpCodes.Ldloc, pickupVariable),
            w.Create(OpCodes.Ldloc, positionVariable),
            w.Create(OpCodes.Ldloc, velocityVariable),
            w.Create(OpCodes.Ldloc, isDuplicatedVariable)
        ).InsertBranchOver(w.Instructions[w.Index - 4], w.Current);
    }
}