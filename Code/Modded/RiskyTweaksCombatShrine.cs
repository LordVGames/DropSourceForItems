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
using static RoR2.GenericPickupController;
namespace DropSourceForItems.Modded;


[MonoDetourTargets(typeof(RiskyTweaks.Tweaks.Interactables.ShrineCombatItems))]
internal static class RiskyTweaksCombatShrine
{
    [MonoDetourHookInitialize]
    private static void Setup()
    {
        if (!ModSupport.RiskyTweaksMod.ModIsRunning)
        {
            return;
        }
        Mdh.RiskyTweaks.Tweaks.Interactables.ShrineCombatItems.ShrineCombatBehavior_OnDefeatedServer.ILHook(OnDefeated);
    }


    private static void OnDefeated(ILManipulationInfo info)
    {
        ILWeaver w = new(info);


        bool matchedFirst = false;
        w.MatchMultipleRelaxed(
            onMatch: w2 =>
            {
                if (!matchedFirst)
                {
                    matchedFirst = true;
                    w2.DeclareVariable(typeof(Vector3), out var velocityVariable);
                    w2.DeclareVariable(typeof(Vector3), out var positionVariable);
                    w2.DeclareVariable(typeof(PickupIndex), out var pickupVariable);


                    w2.InsertBeforeCurrent(
                        w2.Create(OpCodes.Stloc, velocityVariable),
                        w2.Create(OpCodes.Stloc, positionVariable),
                        w2.Create(OpCodes.Stloc, pickupVariable),
                        w2.Create(OpCodes.Ldarg_2),
                        w2.Create(OpCodes.Ldloc, pickupVariable),
                        w2.Create(OpCodes.Ldloc, positionVariable),
                        w2.Create(OpCodes.Ldloc, velocityVariable),
                        w2.CreateDelegateCall((ShrineCombatBehavior shrineCombatBehavior, PickupIndex pickupIndex, Vector3 position, Vector3 velocity) =>
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
                            pickupInfo.SetPickupDropSource(shrineCombatBehavior.gameObject);
                            PickupDropletController.CreatePickupDroplet(pickupInfo, position, velocity);
                        }),
                        w2.Create(OpCodes.Ldloc, pickupVariable),
                        w2.Create(OpCodes.Ldloc, positionVariable),
                        w2.Create(OpCodes.Ldloc, velocityVariable)
                    ).InsertBranchOver(w2.Instructions[w2.Index - 3], w2.Current);
                }
            },
            x => x.MatchCallOrCallvirt<PickupDropletController>("CreatePickupDroplet") && w.SetCurrentTo(x)
        ).ThrowIfFailure();


        int createPickupInfoVariableNumber = 22;
        w.MatchRelaxed(
            x => x.MatchLdloca(createPickupInfoVariableNumber),
            x => x.MatchCallOrCallvirt<Quaternion>("get_identity"),
            x => x.MatchStfld("RoR2.GenericPickupController/CreatePickupInfo", "rotation") && w.SetCurrentTo(x)
        ).ThrowIfFailure()
        .InsertAfterCurrent(
            w.Create(OpCodes.Ldloc, createPickupInfoVariableNumber),
            w.Create(OpCodes.Ldarg_2),
            w.CreateDelegateCall((GenericPickupController.CreatePickupInfo createPickupInfo, ShrineCombatBehavior shrineCombatBehavior) =>
            {
                if (shrineCombatBehavior == null || shrineCombatBehavior.gameObject == null)
                {
                    return createPickupInfo;
                }
                createPickupInfo.SetPickupDropSource(shrineCombatBehavior.gameObject);
                return createPickupInfo;
            }),
            w.Create(OpCodes.Stloc, createPickupInfoVariableNumber)
        );
    }
}