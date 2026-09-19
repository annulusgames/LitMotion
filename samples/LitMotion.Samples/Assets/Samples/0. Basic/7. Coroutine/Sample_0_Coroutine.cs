using System.Collections;
using UnityEngine;
using LitMotion;
using LitMotion.Extensions;

namespace LitMotionSamples
{
    public class Sample_0_Coroutine : MonoBehaviour
    {
        [SerializeField] Transform[] targets;

        IEnumerator Start()
        {
            for (int i = 0; i < targets.Length; i++)
            {
                var direction = i % 2 == 0 ? 1 : -1;
                yield return LMotion.Create(-5f * direction, 5f * direction, 2f)
                    .WithEase(Ease.InOutSine)
                    .BindToPositionX(targets[i]);
            }
        }
    }
}