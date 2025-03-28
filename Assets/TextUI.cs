using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;


[Serializable]
public class TextUI : IDisposable
{
    [SerializeField] private TMP_Text _text;
    private CancellationTokenSource _cts = new();
    private bool _isTyping;

    public void Dispose()
    {
        _cts.CancelAndDispose();
    }

    public void SetEmpty()
    {
        _cts.CancelAndDispose();
        _text.text = string.Empty;
    }

    public void SetText(string newText, bool isTyping = false)
    {
        if (isTyping)
        {
            _cts.CancelAndDispose();
            _cts = new();
            _ = TypeText(newText, _cts.Token);
        }
        else
        {
            _text.text = newText;
        }
    }

    private async UniTaskVoid TypeText(string text, CancellationToken token)
    {
        if (token.IsCancellationRequested || string.IsNullOrEmpty(text))
            return;

        _text.text = $"<color=#FFFFFF00>{text}</color>";

        for (int i = 0; i < text.Length; i++)
        {
            if (token.IsCancellationRequested || !_isTyping)
            {
                _text.text = text;
                return;
            }

            _text.text = $"{text[..(i + 1)]}<color=#FFFFFF00>{text[(i + 1)..]}</color>";

            try
            {
                await UniTask.Delay(10, cancellationToken: token);
            }
            catch
            {
                _text.text = text;
                return;
            }
        }

        _text.text = text;
        _isTyping = false;
    }
}