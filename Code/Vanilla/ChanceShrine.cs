using Mono.Cecil.Cil;
using MonoDetour;
using MonoDetour.Cil;
using MonoDetour.HookGen;
using MonoMod.Cil;
using RoR2;
using RoR2.Artifacts;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
namespace DropSourceForItems.Vanilla;


[MonoDetourTargets(typeof(ShrineChanceBehavior))]
internal static class ChanceShrine
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        Mdh.RoR2.ShrineChanceBehavior.AddShrineStack.ILHook(AddShrineStack);
    }


    private static void AddShrineStack(ILManipulationInfo info)
    {
        ILWeaver w = new(info);
        w.DeclareVariable(typeof(Vector3), out var velocityVariable);
        w.DeclareVariable(typeof(Vector3), out var positionVariable);
        w.DeclareVariable(typeof(UniquePickup), out var pickupVariable);


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
            w.CreateDelegateCall((ShrineChanceBehavior chanceShrineBehavior, UniquePickup pickup, Vector3 position, Vector3 velocity) =>
            {
                var pickupInfo = new GenericPickupController.CreatePickupInfo
                {
                    rotation = Quaternion.identity,
                    pickup = pickup,
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
        // hate going through instructions like ^^ but if it causes problems i'll do something about it later
    }
}