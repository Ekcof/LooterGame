using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public interface IUIWindow
{
    IUIWindow Open();
    void Close();
}

public abstract partial class UIWindow : MonoBehaviour, IUIWindow
{
    [SerializeField]
    private Button _closeButton;
    
    public UnityEvent OnWindowClosed { get; } = new UnityEvent();

    protected virtual void Awake()
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(Close);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
        OnWindowClosed.Invoke();
    }

    public virtual IUIWindow Open()
    {
        gameObject.SetActive(true);
        return this;
    }
}