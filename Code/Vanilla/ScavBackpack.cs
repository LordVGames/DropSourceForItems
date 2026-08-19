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


[MonoDetourTargets(typeof(RoR2.ScavBackpackBehavior))]
internal static class ScavBackpack
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.ScavBackpackBehavior.ItemDrop.ILHook(ItemDrop);
    }


    private static void ItemDrop(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.DeclareVariable(typeof(Vector3), out var velocityVariable);
        w.DeclareVariable(typeof(Vector3), out var positionVariable);
        w.DeclareVariable(typeof(PickupIndex), out var pickupVariable);


        w.MatchRelaxed(
            x => x.MatchCallOrCallvirt<PickupDropletController>("CreatePickupDroplet") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertBeforeCurrent(
            w.Create(OpCodes.Stloc, velocityVariable),
            w.Create(OpCodes.Stloc, positionVariable),
            w.Create(OpCodes.Stloc, pickupVariable),
            w.Create(OpCodes.Ldarg_0),
            w.Create(OpCodes.Ldloc, pickupVariable),
            w.Create(OpCodes.Ldloc, positionVariable),
            w.Create(OpCodes.Ldloc, velocityVariable),
            w.CreateDelegateCall((ShrineChanceBehavior chanceShrineBehavior, PickupIndex pickupIndex, Vector3 position, Vector3 velocity) =>
            {
                var pickupInfo = new GenericPickupController.CreatePickupInfo
                {
                    rotation = Quaternion.identity,
                    pickup = new UniquePickup
                    {
                        pickupIndex = pickupIndex
                    },
                    position = position,
                    duplicated = false,
                    recycled = false
                };
                pickupInfo.SetPickupDropSource(chanceShrineBehavior.gameObject);
                PickupDropletController.CreatePickupDroplet(pickupInfo, position, velocity);
            }),
            w.Create(OpCodes.Ldloc, pickupVariable),
            w.Create(OpCodes.Ldloc, positionVariable),
            w.Create(OpCodes.Ldloc, velocityVariable)
        ).InsertBranchOver(w.Instructions[w.Index - 3], w.Current);
    }
}