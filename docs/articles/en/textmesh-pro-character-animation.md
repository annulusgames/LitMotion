# TextMesh Pro Character Animation

In addition to the ability to animate text and values in TextMesh Pro, LitMotion provides the ability to animate specified characters.

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

Character values are applied on top of the mesh TextMesh Pro generates. Position is an offset from where the character is laid out, rotation and scale are applied around the character's center, and color is a tint multiplied with the character's vertex colors, so rich text colors, vertex gradients and changes to `TMP_Text.color` are kept.

Characters keep the values their motions end with, including when the mesh is updated (such as rewriting the text or calling `ForceMeshUpdate()`). Values are stored per character index, so after the text is rewritten they apply to the characters at the same indices. Call `ResetTMPChars()` to return the characters to how TextMesh Pro draws them.

```cs
text.ResetTMPChars();
```