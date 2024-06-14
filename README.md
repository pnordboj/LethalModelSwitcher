# Not finished yet still WIP
# Lethal Model Switcher

## What is Lethal Model Switcher?

This development library is a mod and a tool for developers, it lets users attatch sounds to their models with ease.
The reason i develop this library was primarily for myself at first as i was developing a Goku mod for Lethal Company.
Where i wanted to let the user "transform" into super sayian, with transformation and sound etc.
This became a rather difficult task so while developing the mod and coming real close to being finished, i realised that this could be a tool instead of a single mod.
A tool that everyone could use and have fun with, so instead i redid everything and
renamed it into Lethal Model Switcher - a easy to use tool for developers to develop their model mods.

Features at first release might change, as its under heavy development (Alot of changes happening rapidly)
The goal is obviously to have all "Upcoming Features" at release but i dont want to promise anything.
My current goal is to have support for multiple model variants within 1 model, that you can swap between when you want!

## Features
- **Model Registration**: Easily register base and variants through the ModelReplacementAPI library
- **Sound Support**: Attatch sounds when toggling between the models
- **Accessible**: Easy to use for people that havent touched code before.

## Upcoming Features


- **Dynamic UI**: for selecting and previewing models you want to toggle between
- **API** for customizing model switching behavior.
- **Support** for animations and effects.
- **Better wiki** documentation and examples here on GitHub.
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
