# Migrate from PrimeTween

This page provides a simple guide for migrating from PrimeTween to LitMotion.

## Position Tweens

```cs
var endValue = new Vector3(1f, 2f, 3f);
var duration = 1f;

// PrimeTween
Tween.Position(transform, endValue, duration);

// LitMotion
LMotion.Create(transform.position, endValue, duration)
    .BindToPosition(transform);
```

## Value Tweens

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

## Tween Settings

```cs
// PrimeTween
Tween.Position(transform, endValue: endValue, duration: duration, ease: Ease.InOutSine, cycle: 2, cycleMode: CycleMode.Yoyo);

// LitMotion
LMotion.Create(transform.position, endValue, duration)
    .WithLoops(2, LoopType.Yoyo)
    .WithEase(Ease.OutQuad)
    .BindToPosition(transform);
```

## Tween Control

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

Adding callbacks to sequences is not supported in LitMotion. This is intentional, as LitMotion encourages the use of async methods for complex animations that require multiple callbacks. For more details, refer to the [Design Philosophy](./design-philosophy.md).

## Changing Update Timing

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

## Coroutine, async/await

```cs
// PrimeTween
yield return tween.ToYieldInstruction();
await tween;

// LitMotion
yield return handle.ToYieldInstruction();
await handle;
```

## Unsupported Features

### Delay

There is no equivalent of `Tween.Delay()` in LitMotion. For more details, refer to the [FAQ](faq.md).

### AtSpeed

There is no equivalent of `Tween.**AtSpeed()` in LitMotion. You need to calculate the necessary duration based on the distance between the start and end points.
