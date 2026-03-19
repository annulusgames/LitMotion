using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Sandbox : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text text;

    void Start()
    {
        LSequence.Create()
            .Append(LMotion.Create(-5f, 5f, 0.5f).BindToPositionX(target))
            .Append(LMotion.Create(0f, 5f, 0.5f).BindToPositionY(target))
            .Append(LMotion.Create(-2f, 2f, 1f).BindToPositionZ(target))
            .Append(LMotion.Create(5f, 0f, 0.5f).BindToPositionX(target))
            .Append(LMotion.Create(5f, 0f, 0.5f).BindToPositionY(target))
            .Append(LMotion.Create(2f, 0f, 1f).BindToPositionZ(target))
            .Run(configure =>
            {
                configure.WithEase(Ease.InCirc);
            })
            .AddTo(this);

        for (int i = 0; i < text.text.Length; i++)
        {
            LMotion.Create(0f, 1f, 0.5f)
                .WithDelay(i * 0.1f)
                .BindToTMPCharColorA(text, i)
                .AddTo(this);
        }
    }
}
