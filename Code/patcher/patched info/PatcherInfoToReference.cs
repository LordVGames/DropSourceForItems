using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace RoR2;
#pragma warning restore IDE0130 // Namespace does not match folder structure


public class GenericPickupController
{
    public GameObject dsfi_dropSource;
    public string dsfi_cachedDropSourceName;
    public struct CreatePickupInfo
    {
        public GameObject dsfi_dropSource;
        public string dsfi_cachedDropSourceName;
    }
}


public class PickupPickerController
{
    public GameObject dsfi_dropSource;
    public string dsfi_cachedDropSourceName;
}