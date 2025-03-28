using UnityEngine;
using UnityEngine.UI;

public interface IUIWindow
{
    IUIWindow Open();
    void Close();
}

public abstract partial class UIWindow : MonoBehaviour, IUIWindow
{
    [SerializeField]
    private Button _closeButton;

    protected virtual void Awake()
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(Close);
    }

    public virtual void Close()
    {
        gameObject.SetActive(false);
    }

    public virtual IUIWindow Open()
    {
        gameObject.SetActive(true);
        return this;
    }
}
