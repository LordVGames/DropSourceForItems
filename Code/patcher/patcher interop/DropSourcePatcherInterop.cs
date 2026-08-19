using RoR2;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace DropSourceForItems;


public static class PatchedPickupInfoInterop
{
    public static GameObject GetPickupDropSource(this ref GenericPickupController.CreatePickupInfo pickupInfo) => pickupInfo.dsfi_dropSource;
    public static void SetPickupDropSource(this ref GenericPickupController.CreatePickupInfo pickupInfo, GameObject value)
    {
        pickupInfo.dsfi_dropSource = value;
        pickupInfo.dsfi_cachedDropSourceName = value.name;
    }
    public static string GetCachedPickupDropSourceName(this ref GenericPickupController.CreatePickupInfo pickupInfo) => pickupInfo.dsfi_cachedDropSourceName;
    [Obsolete("SetPickupDropSource will set this for you")]
    public static void SetCachedPickupDropSourceName(this ref GenericPickupController.CreatePickupInfo pickupInfo, string value) => pickupInfo.dsfi_cachedDropSourceName = value;


    public static GameObject GetPickupDropSource(this GenericPickupController gpc) => gpc.dsfi_dropSource;
    public static void SetPickupDropSource(this GenericPickupController gpc, GameObject value)
    {
        gpc.dsfi_dropSource = value;
        gpc.dsfi_cachedDropSourceName = value.name;
    }
    public static string GetCachedPickuDropSourceName(this GenericPickupController gpc) => gpc.dsfi_cachedDropSourceName;
    [Obsolete("SetPickupDropSource will set this for you")]
    public static void SetCachedPickupDropSourceName(this GenericPickupController gpc, string value) => gpc.dsfi_cachedDropSourceName = value;


    public static GameObject GetPickupDropSource(this PickupPickerController ppc) => ppc.dsfi_dropSource;
    public static void SetPickupDropSource(this PickupPickerController ppc, GameObject value)
    {
        ppc.dsfi_dropSource = value;
        ppc.dsfi_cachedDropSourceName = value.name;
    }
    public static string GetCachedPickupDropSourceName(this PickupPickerController ppc) => ppc.dsfi_cachedDropSourceName;
    [Obsolete("SetPickupDropSource will set this for you")]
    public static void SetCachedPickupDropSourceName(this PickupPickerController ppc, string value) => ppc.dsfi_cachedDropSourceName = value;
}