# DropSourceForItems

This mod adds new fields to `GenericPickupController`, `GenericPickupController.CreatePickupInfo` and `PickupPickerController` that store what GameObject an item was spawned from and its name (eventually for the cases of enemies dropping an item on death). To use these new fields, use the extension methods `GetPickupDropSource`, `SetPickupDropSource` and `GetCachedPickupDropSourceName`. The cached name is set for you when using `SetPickupDropSource`.

The functionality for setting drop sources were manually added, so mods' own item drops won't support this unless added by this mod or themselves. Items from choice pickups have their drop source set to the choice pickup itself instead of the thing that spawned the choice pickup (i.e. drop source from void potential is the void potential instead of the thing that spawned the void potential. void potential itself will have its drop source set to the thing that dropped it)

To use this, just add the `DropSourcePatcherInterop.dll` file from the mod as a project reference. Adding the mod from thunderstore through nuget may also work, but I have not tested that yet

### Mods supported

- EnemiesReturns
- - Lynx shrine
- RiskyTweaks
- - Combat shrine dropping items


### Todo

- Setup drop sources for items dropped by enemies on death
- - Then you could the cached drop source name (since the enemy would be null after death) to find the type of body that dropped the item