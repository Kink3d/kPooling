# kPooling
[![openupm](https://img.shields.io/npm/v/com.kink3d.pooling?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.kink3d.pooling/)

### Customizable Object Pooling for Unity.

![alt text](https://github.com/Kink3d/kPooling/wiki/Images/Home00.png?raw=true)
*GameObject pooling example*

kPooling is an object pooling system for Unity. It is based on a flexible generic typed API and supports creation and management of `GameObject` type pools by default. kPooling also comes with a simple but powerful `Processor` API for adding pooling support for any C# type in both runtime and Editor.

Refer to the [Wiki](https://github.com/Kink3d/kPooling/wiki/Home) for more information.

## Instructions

### Install via OpenUPM
The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.kink3d.pooling
```

### Install via Git URL
- Open your project manifest file (`MyProject/Packages/manifest.json`).
- Add `"com.kink3d.pooling": "https://github.com/Kink3d/kPooling.git"` to the `dependencies` list.
- Open or focus on Unity Editor to resolve packages.

## Requirements
- Unity 2019.3.0f3 or higher.
