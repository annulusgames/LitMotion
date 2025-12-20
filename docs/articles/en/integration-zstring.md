# ZString

By integrating [ZString](https://github.com/Cysharp/ZString) into your project, you can achieve zero-allocation string formatting.

If you install ZString via the Package Manager, the internal processing will automatically be replaced with ZString. If you use a unitypackage or similar method to install, you'll need to add `LITMOTION_SUPPORT_ZSTRING` to `Project Settings > Player > Scripting Define Symbols`.

### Zero-Allocation for BindToText

When passing a `string format` argument to `BindToText()`, LitMotion internally uses `string.Format()`. Since it takes an object as an argument, it incurs allocation due to boxing.

```cs
TMP_Text text;
LMotion.Create(0, 100, 2f)
    .BindToText(text, "{0:0}"); // Causes GC allocation per frame
```

The internal implementation of `BindToText()` is partially as follows:

```cs
// Part of the code for BindToText()
builder.BindWithState(text, format, (x, target, format) =>
{
    ...
    target.text = string.Format(format, x); // Allocates due to boxing here
});
```

Introducing ZString replaces this with processing using `ZString.Format()` within LitMotion. This reduction eliminates unnecessary boxing allocations.

```cs
builder.BindWithState(text, format, (x, target, format) =>
{
    ...
    target.text = ZString.Format(format, x); // Allows zero-allocation formatting
});
```

Furthermore, if the target is `TMP_Text`, instead of using `ZString.Format()`, LitMotion uses `SetTextFormat()`, an extension method of ZString. This method internally utilizes `TMP_Text.SetText()`, enabling entirely zero-allocation formatting processing.

```cs
builder.BindWithState(text, format, (x, target, format) =>
{
    ...
    target.SetTextFormat(format, x);
});
```