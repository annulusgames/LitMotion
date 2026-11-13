# TextMesh Proの文字をアニメーションさせる

LitMotionではTextMesh Proのテキストや値をアニメーションさせる機能に加え、一つ一つの文字を個別にアニメーションさせる機能が用意されています。

![gif-img1](../../images/textmeshpro-character-motion.gif)

```cs
TMP_Text text;
for (int i = 0; i < text.textInfo.characterCount; i++)
{
    LMotion.Create(Color.white, Color.red, 1f)
        .WithDelay(i * 0.1f)
        .WithEase(Ease.OutQuad)
        .BindToTMPCharColor(text, i);
    
    LMotion.Punch.Create(Vector3.zero, Vector3.up * 30f, 1f)
        .WithDelay(i * 0.1f)
        .WithEase(Ease.OutQuad)
        .BindToTMPCharPosition(text, i);
}
```

文字の値はTextMesh Proが生成したメッシュの上に適用されます。位置は文字が配置された位置からのオフセット、回転とスケールは文字の中心を基準に適用され、色は文字の頂点カラーに乗算されるティントとして扱われます。そのため、リッチテキストの色や頂点グラデーション、`TMP_Text.color`の変更は維持されます。

モーションの終了後も、文字は最後の値を保持します。これはメッシュの更新(テキストの書き換えや`ForceMeshUpdate()`の呼び出しなど)があった場合も同様です。値は文字のインデックスごとに保持されるため、テキストを書き換えた後は同じインデックスの文字に適用されます。TextMesh Proが描画する元の状態に戻すには`ResetTMPChars()`を呼び出してください。

```cs
text.ResetTMPChars();
```