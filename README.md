## Lethal Model Switcher

Thank you for using the Lethal Model Switcher library! This library simplifies the process of switching models in the game Lethal Company. Below are instructions on how to use this library for your own model mod.

### Versioning

BepInEx uses semantic versioning, or semver, for the mod's version info. To increment it, you can either modify the version tag in the `.csproj` file directly, or use your IDE's UX to increment the version. Below is an example of modifying the `.csproj` file directly:

```xml
<!-- BepInEx Properties -->
<PropertyGroup>
    <AssemblyName>LethalModelSwitcher</AssemblyName>
    <Product>LethalModelSwitcher</Product>
    <!-- Change to whatever version you're currently on. -->
    <Version>1.0.0</Version>
</PropertyGroup>
```

Your IDE will have the setting in Package or NuGet under General or Metadata, respectively.

### Logging

A logger is provided to help with logging to the console. You can access it by using `LethalModelSwitcher.Logger` in any class outside the `LethalModelSwitcher` class.

Please use `LogDebug()` whenever possible, as any other log method will be displayed to the console and potentially cause performance issues for users.

If you choose to do so, make sure you change the following line in the `BepInEx.cfg` file to see the Debug messages:

```ini
[Logging.Console]

# ... #

## Which log levels to show in the console output.
# Setting type: LogLevel
# Default value: Fatal, Error, Warning, Message, Info
# Acceptable values: None, Fatal, Error, Warning, Message, Info, Debug, All
# Multiple values can be set at the same time by separating them with , (e.g. Debug, Warning)
LogLevels = All
```

### Harmony

This template uses Harmony for patching. For more specifics on how to use it, look at the [HarmonyX GitHub wiki](https://harmony.pardeike.net/) and the [Harmony docs](https://harmony.pardeike.net/articles/intro.html).

To make a new harmony patch, just use `[HarmonyPatch]` before any class you make that has a patch in it.

Then in that class, you can use `[HarmonyPatch(typeof(ClassToPatch), "MethodToPatch")]` where `ClassToPatch` is the class you're patching (e.g., `TVScript`), and `MethodToPatch` is the method you're patching (e.g., `SwitchTVLocalClient`).

Then you can use the appropriate prefix, postfix, transpiler, or finalizer attribute.

While you can use `return false;` in a prefix patch, it is highly discouraged as it can and will cause compatibility issues with other mods.

### Using LethalModelSwitcher Library

To use this library in your own model mod, you need to ensure the following:

1. **Add Dependencies**:
   Make sure you have the following dependencies installed:
   - [Lethal-Company-More-Suits](https://github.com/x753/Lethal-Company-More-Suits)
   - [ModelReplacementAPI](https://github.com/BepInEx/BepInEx) (included with BepInEx)

2. **Load Asset Bundle**:
   Ensure you load your asset bundle containing the models and sounds.

3. **Register Models**:
   Use the `ModelManager` class to register your models with the appropriate suit names.

### Example Mod Implementation

Here’s an example of how to implement your mod using the `LethalModelSwitcher` library:

```csharp
using BepInEx;
using UnityEngine;

[BepInPlugin("your.goku.mod.id", "Goku Mod", "1.0.0")]
[BepInDependency("LethalModelSwitcher")]
public class GokuMod : BaseUnityPlugin
{
    private void Awake()
    {
        // Load the asset bundle
        AssetLoader.LoadAssetBundle("lethalmodelswitchingbundle");

        // Load assets from the bundle
        var normalGokuSound = AssetLoader.LoadAudioClip("normalGokuSound");
        var ultraGokuSound = AssetLoader.LoadAudioClip("ultraGokuSound");
        var normalGokuPrefab = AssetLoader.LoadPrefab("NormalGokuPrefab");
        var ultraGokuPrefab = AssetLoader.LoadPrefab("UltraGokuPrefab");

        // Register models with suit names
        ModelManager.RegisterBaseModel("Orange Suit", "NormalGoku", typeof(NormalGokuReplacement), normalGokuSound, normalGokuPrefab);
        ModelManager.RegisterModelVariant("Orange Suit", "UltraInstinctGoku", typeof(UltraGokuReplacement), ultraGokuSound, ultraGokuPrefab);
    }
}
```

### Registering Models

You can register models using the `ModelManager` class methods:

```csharp
ModelManager.RegisterBaseModel("SuitName", "ModelName", typeof(ModelType), audioClip, modelPrefab);
ModelManager.RegisterModelVariant("BaseSuitName", "VariantName", typeof(VariantType), audioClip, modelPrefab);
```

Replace `"SuitName"`, `"ModelName"`, and other placeholders with your actual suit and model names, types, and assets.

By following these steps, you can easily integrate and use the `LethalModelSwitcher` library to manage model switching in your Lethal Company mods.
