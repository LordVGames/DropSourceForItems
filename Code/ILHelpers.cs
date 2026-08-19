using MiscFixes.Modules;
using Mono.Cecil.Cil;
using MonoDetour.Cil;
using MonoDetour.Cil.Analysis;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace DropSourceForItems;


internal static class ILHelpers
{
    internal static void LogILInstructions(this ILWeaver w)
    {
        foreach (Instruction instruction in w.Instructions)
        {
            Log.Warning(instruction);
        }
    }
}