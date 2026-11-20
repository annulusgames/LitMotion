# モーションの設定

モーションの作成時にイージングや繰り返し回数、コールバックなどの設定を追加することができます。
設定を追加するには`With-`メソッドを使用します。

記述する際にはメソッドチェーンを用いて複数の設定を同時に適用できます。

```cs
var value = 0f;
LMotion.Create(0f, 10f, 2f)
    .WithEase(Ease.OutQuad)
    .WithComplete(() => Debug.Log("Complate!"))
    .Bind(x => value = x);
```

### メソッド一覧

#### WithEase

モーションに適用するイージング関数を指定します。

これにはAnimtaionCurveを指定することも可能です。(`Ease.CustomAnimationCurve`は`WithEase(AnimationCurve)`を使用した際に自動で設定されます。このオプションを`WithEase(Ease)`で指定しないでください。)

さらに、Burstでコンパイルした関数ポインタを完全に独自のイージング関数として使用できます。`CustomEaseDelegate`シグネチャの静的メソッドを定義し、`[BurstCompile]`と`[MonoPInvokeCallback]`を付与して`BurstCompiler.CompileFunctionPointer`でコンパイルしてください。(`Ease.CustomFunction`は`WithEase(FunctionPointer<CustomEaseDelegate>)`を使用した際に自動で設定されます。このオプションを`WithEase(Ease)`で指定しないでください。)

```cs
using AOT;
using Unity.Burst;
using Unity.Mathematics;

[BurstCompile]
static class CustomEases
{
    [BurstCompile]
    [MonoPInvokeCallback(typeof(CustomEaseDelegate))]
    public static float OutQuint(float t) => 1f - math.pow(1f - t, 5f);
}

static readonly FunctionPointer<CustomEaseDelegate> outQuint =
    BurstCompiler.CompileFunctionPointer<CustomEaseDelegate>(CustomEases.OutQuint);

LMotion.Create(0f, 10f, 1f)
    .WithEase(outQuint)
    .Bind(x => value = x);
```

独自関数はBurstコンパイルされた更新用ジョブの内部で評価されるため、`t`のみに依存する純粋関数である必要があります。`LoopType`によっては1フレームに複数回呼ばれます(`LoopType.Incremental`では1フレームに2回評価されます)。組み込みのイージングと同様に、`t = 0`で`0`、`t = 1`で`1`を返すようにしてください。

以下の制約に注意してください。

* 関数ポインタはドメインリロードで無効になります。リロード後に再コンパイルしてください(静的初期化子や初期化用メソッドなどで行います)。
* 独自関数はシリアライズできないため、`SerializableMotionSettings`やInspectorからは設定できません。コードからのみ使用できます。
* Burstが無効な場合(EditorでBurstのコンパイルをオフにした場合など)は、キャッシュされたマネージドデリゲート経由で呼び出されます。動作はしますが、Burstコンパイルされた経路より遅くなります。

#### WithDelay

モーションの開始を指定された秒数だけ遅延させます。`DelayType`や`SkipValuesDuringDelay`を指定することで挙動を調整できます。

* DelayType

    ループ時の遅延の動作を指定します。

    | DelayType           | 動作                                                 |
    | ------------------- | ---------------------------------------------------- |
    | DelayType.FirstLoop | デフォルトの設定。最初のループのみ遅延を適用します。 |
    | DelayType.EveryLoop | 各ループごとに遅延を適用します。                     |

* SkipValuesDuringDelay

    遅延時間中にBindの処理をスキップするかどうかを指定します。デフォルトではtrueに設定されます。


#### WithLoops

モーションを繰り返す回数を設定します。デフォルトでは1に設定されています。
-1を設定することで、停止されるまで無限に繰り返すモーションを作成することも可能です。

また、第2引数に`LoopType`を設定することで繰り返し時の挙動を設定することが可能です。

| LoopType | 動作 | 
| - | - | 
| LoopType.Restart | デフォルトの設定。ループ終了時に開始値にリセットされます。| 
| LoopType.Flip | 開始値と終了地を往復するように値をアニメーションさせます。| 
| LoopType.Increment | ループごとに値が増加します。 | 
| LoopType.Yoyo | 開始値と終了値をヨーヨーのような動きでアニメーションさせます。| 

#### WithOnComplete

再生終了時のコールバックを指定します。`Preserve()`で`MotionHandle`を再利用する場合、このコールバックはモーションの完了時に毎回呼び出されます。

#### WithOnCancel

キャンセル時のコールバックを指定します。

#### WithOnLoopComplete

各ループ終了時のコールバックを指定します。これはモーションの完了時にも呼び出され、順番は`OnLoopComplate` → `OnComplete`になります。

#### WithScheduler

モーションの再生に使用するSchedulerを指定します。作成したモーションは指定したSchedulerに対応したPlayerLoopで更新が行われます。

| Scheduler | 動作 |
| - | - |
| MotionScheduler.Initialization | Initializationのタイミングで更新を行います。 |
| MotionScheduler.InitializationIgnoreTimeScale | Initializationのタイミングで更新を行います。また、`Time.timeScale`の影響を無視します。 |
| MotionScheduler.InitializationRealtime | Initializationのタイミングで更新を行います。また、`Time.timeScale`の影響を無視し、`Time.realtimeSinceStartup`を用いて時間の計算を行います。 |
| MotionScheduler.EarlyUpdate | EarlyUpdateのタイミングで更新を行います。 |
| MotionScheduler.EarlyUpdateIgnoreTimeScale | EarlyUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視します。 |
| MotionScheduler.EarlyUpdateRealtime | EarlyUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視し、`Time.realtimeSinceStartup`を用いて時間の計算を行います。 |
| MotionScheduler.FixedUpdate | FixedUpdateのタイミングで更新を行います。 |
| MotionScheduler.PreUpdate | PreUpdateのタイミングで更新を行います。 |
| MotionScheduler.PreUpdateIgnoreTimeScale | PreUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視します。 |
| MotionScheduler.PreUpdateRealtime | PreUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視し、`Time.realtimeSinceStartup`を用いて時間の計算を行います。 |
| MotionScheduler.Update | Updateのタイミングで更新を行います。 |
| MotionScheduler.UpdateIgnoreTimeScale | Updateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視します。 |
| MotionScheduler.UpdateRealtime | Updateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視し、`Time.realtimeSinceStartup`を用いて時間の計算を行います。 |
| MotionScheduler.PreLateUpdate | PreLateUpdateのタイミングで更新を行います。 |
| MotionScheduler.PreLateUpdateIgnoreTimeScale | PreLateUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視します。 |
| MotionScheduler.PreLateUpdateRealtime | PreLateUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視し、`Time.realtimeSinceStartup`を用いて時間の計算を行います。 |
| MotionScheduler.PostLateUpdate | PostLateUpdateのタイミングで更新を行います。 |
| MotionScheduler.PostLateUpdateIgnoreTimeScale | PostLateUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視します。 |
| MotionScheduler.PostLateUpdateRealtime | PostLateUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視し、`Time.realtimeSinceStartup`を用いて時間の計算を行います。 |
| MotionScheduler.TimeUpdate | TimeUpdateのタイミングで更新を行います。 |
| MotionScheduler.TimeUpdateIgnoreTimeScale | TimeUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視します。 |
| MotionScheduler.TimeUpdateRealtime | TimeUpdateのタイミングで更新を行います。また、`Time.timeScale`の影響を無視し、`Time.realtimeSinceStartup`を用いて時間の計算を行います。 |
| MotionScheduler.Manual | ManualMotionDispatcherを用いて更新を行います。詳細は[ManualMotionDispatcher](manual-motion-dispatcher.md)を参照してください。 |
| EditorMotionScheduler.Update (LitMotion.Editor) | EditorApplication.updateのタイミングで更新を行います。このSchedulerはエディタ限定で使用できます。 |

#### WithCancelOnError

モーションの`Bind`関数内でcatchされていない例外が発生した際にモーションをキャンセルします。デフォルトではfalseに設定されています。

#### WithImmediateBind

モーションをスケジュールするタイミングでBindの処理を実行します。デフォルトではtrueに設定されています。

#### WithDebugName

デバッグに利用する名前を設定します。詳細は[LitMotion Debugger](litmotion-debugger.md)の項目を参照してください。

#### WithRoundingMode (int, long)

小数点以下の値の丸め方を設定します。このオプションはint型のモーションにのみ適用可能です。

| RoundingMode | 動作 |
| - | - |
| RoundingMode.ToEven | デフォルトの設定。最も近い整数値に値を丸め、値が中間にある場合は最も近い偶数に丸めます。 |
| RoundingMode.AwayFromZero | 一般的な四捨五入の動作。最も近い整数値に値を丸め、値が中間にある場合は0から遠ざかる方向に値を丸めます。 |
| RoundingMode.ToZero | 0に近づく方向に値を丸めます。 |
| RoundingMode.ToPositiveInfinity | 正の無限大に近づく方向に値を丸めます。 |
| RoundingMode.ToNegativeInfinity | 負の無限大に近づく方向に値を丸めます。 |

#### WithScrambleMode (FixedString-)

まだ表示されていない文字の部分をランダムな文字で埋めることができます。このオプションは文字列のモーションにのみ適用可能です。

| ScrambleMode | 動作 |
| - | - |
| ScrambleMode.None | デフォルトの設定。まだ表示されていない部分には何も表示されません。 |
| ScrambleMode.Uppercase | ランダムな大文字のアルファベットで空白を埋めます。 |
| ScrambleMode.Lowercase | ランダムな小文字のアルファベットで空白を埋めます。 |
| ScrambleMode.Numerals | ランダムな数字で空白を埋めます。 |
| ScrambleMode.All | ランダムな大文字/小文字のアルファベット、または数字で空白を埋めます。 |
| (ScrambleMode.Custom) | 指定された文字列の中のランダムな数字で空白を埋めます。このオプションは明示的に指定できず、WithScrambleModeの引数にstringを渡した際に設定されます。|

#### WithRichText (FixedString-)

RichTextのサポートを有効化し、RichTextタグが含まれるテキストの文字送りが可能になります。このオプションは文字列のモーションにのみ適用可能です。

#### WithFrequency (Punch, Shake)

Punch, Shakeの振動の周波数(終了時までの振動回数)を設定します。初期値は10に設定されています。

#### WithDampingRatio (Punch, Shake)

Punch, Shakeの振動の減衰比を設定します。この値が1の場合は完全に減衰し、0の場合は一切減衰しません。初期値は1に設定されています。

#### WithRandomSeed (FixedString-, Shake)

モーションの再生の際に使用する乱数のシードを指定できます。これによりScrambleCharsや振動のランダムな動作を制御することができます。