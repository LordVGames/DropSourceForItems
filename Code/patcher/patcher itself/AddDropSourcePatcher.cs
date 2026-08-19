using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace DropSourceForItems;


// ty R2API for existing so i could copy this setup
internal static class AddDropSourcePatcher
{
    public static IEnumerable<string> TargetDLLs { get; } = ["RoR2.dll"];
    public static void Patch(AssemblyDefinition assembly)
    {
        var createPickupInfo = assembly.MainModule.GetType("RoR2.GenericPickupController/CreatePickupInfo");
        var createPickupInfoDropSource = new FieldDefinition("dsfi_dropSource", FieldAttributes.Public, assembly.MainModule.ImportReference(typeof(GameObject)));
        createPickupInfo?.Fields.Add(createPickupInfoDropSource);
        var createPickupInfoCachedDropSourcename = new FieldDefinition("dsfi_cachedDropSourceName", FieldAttributes.Public, assembly.MainModule.ImportReference(typeof(string)));
        createPickupInfo?.Fields.Add(createPickupInfoCachedDropSourcename);


        var genericPickupController = assembly.MainModule.GetType("RoR2.GenericPickupController");
        var genericPickupControllerDropSource = new FieldDefinition("dsfi_dropSource", FieldAttributes.Public, assembly.MainModule.ImportReference(typeof(GameObject)));
        genericPickupController?.Fields.Add(genericPickupControllerDropSource);
        var genericPickupControllerCachedDropSourcename = new FieldDefinition("dsfi_cachedDropSourceName", FieldAttributes.Public, assembly.MainModule.ImportReference(typeof(string)));
        genericPickupController?.Fields.Add(genericPickupControllerCachedDropSourcename);


        var pickupPickerController = assembly.MainModule.GetType("RoR2.PickupPickerController");
        var pickupPickerControllerDropSource = new FieldDefinition("dsfi_dropSource", FieldAttributes.Public, assembly.MainModule.ImportReference(typeof(GameObject)));
        pickupPickerController?.Fields.Add(pickupPickerControllerDropSource);
        var pickupPickerControllerCachedDropSourceName = new FieldDefinition("dsfi_cachedDropSourceName", FieldAttributes.Public, assembly.MainModule.ImportReference(typeof(string)));
        pickupPickerController?.Fields.Add(pickupPickerControllerCachedDropSourceName);
    }
}
