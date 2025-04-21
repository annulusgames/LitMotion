# ZString

プロジェクトに[ZString](https://github.com/Cysharp/ZString)を導入することで、文字列のフォーマット処理をゼロアロケーション化することができます。

ZStringをPackage Managerから導入した場合は、自動で内部の処理がZStringを使用したものに置き換えられます。unitypackage等で導入した場合は、`Project Settings > Player > Scripting Define Symbols`に`LITMOTION_SUPPORT_ZSTRING`を追加する必要があります。

### BindToTextのゼロアロケーション化

`BindToText()`の引数に`string format`を渡した場合、LitMotionは内部で`string.Format()`を使用します。これはobjectを引数にとるため、ボックス化によるアロケーションが発生します。

```cs
TMP_Text text;
LMotion.Create(0, 100, 2f)
    .BindToText(text, "{0:0}"); // フレーム毎にGCアロケーションが発生する
```

`BindToText()`の内部実装は以下のようになっています。

```cs
// BindToText()のコードの一部
builder.Bind(text, format, (x, target, format) =>
{
    ...
    target.text = string.Format(format, x); // ここでボックス化によるアロケーションが発生
});
```

ZStringを導入することで、LitMotionはこれを`ZString.Format()`を使用した処理に置き換えます。これによって余計なボックス化のアロケーションを削減できます。

```cs
builder.Bind(text, format, (x, target, format) =>
{
    ...
    target.text = ZString.Format(format, x); // ゼロアロケーションでFormatが可能
});
```

さらに対象が`TMP_Text`の場合、`ZString.Format()`の代わりにZStringの拡張メソッドである`SetTextFormat()`を使用します。これは内部で`TMP_Text.SetText()`を使用するため、完全にゼロアロケーションでフォーマットの処理を行うことが可能になります。

```cs
builder.Bind(text, format, (x, target, format) =>
{
    ...
    target.SetTextFormat(format, x);
});
```