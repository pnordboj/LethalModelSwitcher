# Lethal Model Switcher

Thank you for using the Lethal Model Switcher library! This library simplifies the process of switching models in the game Lethal Company. Below are instructions on how to use this library for your own model mod.

## Features
- **Model Registration**: Easily register base and variants through the ModelReplacementAPI library
- **Dynamic UI**: for selecting and previewing models you want to toggle between
- **Sound Support**: Attatch sounds when toggling between the models

## Upcoming Features

- Enhanced UI for model selection.
- API for customizing model switching behavior.
- Support for animations and effects.
- Better wiki documentation and examples here on GitHub.
- General improvements and bug fixes based on user feedback.

## Using LethalModelSwitcher Library

To use this library in your own model mod, you need to ensure the following:

### Using the library
Add the `LethalModelSwitcher.dll` file as a reference into your project and your good to go!

### Add Dependencies

Add the following dependencies to your mod:

```csharp
[BepInDependency("x753.More_Suits")]
[BepInDependency("nordbo.LethalModelSwitcher")]
```

### Load Asset Bundle

Ensure you load your asset bundle containing the models and sounds.

### Register Models

Use the `ModelManager` class to register your models with the appropriate suit names.

Load the asset bundle and register models in the `Awake` method of your mod:

```csharp
    private void Awake()
    {
        // Load the asset bundle
        AssetLoader.LoadAssetBundle("lethalmodelswitchingbundle");

        // Load assets from the bundle
        var niceSound = AssetLoader.LoadAudioClip("niceSound");
        var veryNiceSound = AssetLoader.LoadAudioClip("veryNiceSound");
        var coolModel = AssetLoader.LoadPrefab("coolModel");
        var veryCoolModel = AssetLoader.LoadPrefab("veryCoolModel");

        // Register suit names, model names, and assets
        ModelManager.RegisterBaseModel("SuitName", "ModelName", typeof(ModelType), audioClip, modelPrefab);
        ModelManager.RegisterModelVariant("BaseSuitName", "VariantName", typeof(VariantType), audioClip, modelPrefab);
    }
```

You can register models using the `ModelManager` class methods:

```csharp
ModelManager.RegisterBaseModel("SuitName", "ModelName", typeof(ModelType), audioClip, modelPrefab);
ModelManager.RegisterModelVariant("BaseSuitName", "VariantName", typeof(VariantType), audioClip, modelPrefab);
```

Replace `"SuitName"`, `"ModelName"`, and other placeholders with your actual suit and model names, types, and assets.

By following these steps, you can easily integrate and use the `LethalModelSwitcher` library to manage model switching in your Lethal Company mods.

## Credits
- **ModelReplacementAPI**: Provides the API for model replacements. [ModelReplacementAPI](https://thunderstore.io/c/lethal-company/p/BunyaPineTree/ModelReplacementAPI/).
- **45x Suit Variants**: Inspiration for this library. [45x Suit Variant](https://thunderstore.io/c/lethal-company/p/45x_Dev/45x_Suit_Variants/).

Special thanks to the Lethal Company modding community for their contributions and support.

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/I2I4XZ2R6)
---
