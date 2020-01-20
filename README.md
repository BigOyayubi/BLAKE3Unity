# BLAKE3Unity

simple BLAKE3 bindings for Unity3d.

BLAKE3 is crasy fast cryptographics hash function.

see https://github.com/BLAKE3-team/BLAKE3

# Install

```
# build plugins
$ sh make_ios.sh
$ sh make_android.sh
$ sh make_macOS.sh

# copy Plugins
$ cp -ap Plugins/* /your/project/Assets/Plugins/

# copy C# binding
$ cp unity-sample/Assets/Scripts/Bindings.cs /your/project/Assets/Scripts/
```

# C# Sample

```csharp
// simple
var output = new byte[Hasher.OUTPUT_SIZE];
Hasher.Calc(input_bytes, output);

// stream
using(var hasher = Hasher.Create()){
   hasher.Update(input_bytes1);
   hasher.Update(input_bytes2);
   hasher.Finalize(output);
}
```

# TODO

* Windows Support
* (DONE)IntPtr IF(C#)
