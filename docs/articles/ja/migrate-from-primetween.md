# PrimeTweenから移行する

このページでは、PrimeTweenからLitMotionへ移行するための簡易的な対応を示します。

## 位置のトゥイーン

```cs
var endValue = new Vector3(1f, 2f, 3f);
var duration = 1f;

// PrimeTween
Tween.Position(transform, endValue, duration);

// LitMotion
LMotion.Create(tranform.position, endValue, duration)
    .BindToPosition(transform);
```

## 値のトゥイーン

```cs
// PrimeTween
var value = 0f;
Tween.Custom(value, endValue, duration, x => value = x);

// LitMotion
LMotion.Create(value, endValue, duration)
    .Bind(x => value = x);
```

## Punch / Shake

```cs
// PrimeTween
Tween.ShakePosition(transform, ...);
Tween.PunchPosition(transform, ...);

// LitMotion
LMotion.Punch.Create(...)
    .BindToPosition(transform);
LMotion.Shake.Create(...)
    .BindToPosition(transform);
```

## トゥイーンの設定

```cs
// PrimeTween
Tween.Position(transform, endValue: endValue, duration: duration, ease: Ease.InOutSine, cycle: 2, cycleMode: CycleMode.Yoyo);

// LitMotion
LMotion.Create(transform.position, endValue, duration)
    .WithLoops(2, LoopType.Yoyo)
    .WithEase(Ease.OutQuad);
    .BindToPosition(transform);
```

## トゥイーンの制御

```cs
// PrimeTween
tween.isPaused = true;
tween.Complete();
tween.Kill();

// LitMotion
handle.PlaybackSpeed = 0f;
handle.Complete();
handle.Cancel();
```

## Sequence

```cs
// PrimeTween
Sequence.Create()
    .Chain(...) 
    .Group(...)
    .Insert(...);

// LitMotion
LSequence.Create()
    .Append(...)
    .Join(...)
    .Insert(...)
    .Run();
```

Sequenceに対するコールバックの追加はLitMotionではサポートされていません。これは意図的なもので、LitMotionではコールバックを多用する複雑なアニメーションはasyncメソッドを利用すべきだと考えているためです。詳細は[設計思想](./design-philosophy.md)を参照してください。

## 更新タイミングの変更

```cs
// PrimeTween
Tween.PositionX(transform, endValue: 10f, new TweenSettings(duration: 1f, useFixedUpdate: true));

// LitMotion
LMotion.Create(transform.position.x, 10f, 1f)
    .WithScheduler(MotionScheduler.FixedUpdate)
    .BindToPositionX(transform);
```

## Inspector

```cs
// PrimeTween
[SerializeField] TweenSettings<float> settings;
Tween.PositionY(transform, settings);

// LitMotion
[SerializeField] SerializableMotionSettings<float, NoOptions> settings;
LMotion.Create(settings).BindToPositionY(transform);
```

## コルーチン、async/await

```cs
// PrimeTween
yield return tween.ToYieldInstruction();
await tween;

// LitMotion
yield return handle.ToYieldInstruction();
await handle;
```

## サポートされない機能

### Delay

`Tween.Delay()`に相当するメソッドはLitMotionには存在しません。詳細は[よくある質問](faq.md)を参照してください。

### AtSpeed

`Tween.**AtSpeed()`に相当する機能はLitMotionには存在しません。始点・終点の距離から必要なdurationの値を計算してください。
