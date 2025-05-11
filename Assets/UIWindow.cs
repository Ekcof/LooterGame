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
    [SerializeField]
    private Canvas _canvas;
    public static bool IsActive { get; private set; }

	public UnityEvent OnWindowClosed { get; } = new UnityEvent();

    protected virtual void Awake()
    {
        _closeButton.onClick.RemoveAllListeners();
        _closeButton.onClick.AddListener(Close);
    }

    public virtual void Close()
    {
        IsActive = false;
		Debug.Log($"____! Close {name}");
        SetInteractive(false);
        _canvas.enabled = false;
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		OnWindowClosed.Invoke();
    }

    public virtual IUIWindow Open()
    {
        IsActive = true;
		SetInteractive(true);
        _canvas.enabled = true;
		gameObject.SetActive(true);
		Cursor.lockState = CursorLockMode.Confined;
		Cursor.visible = true;
		return this;
    }

    public virtual void SetInteractive(bool isInteractive)
	{
		_closeButton.interactable = isInteractive;
	}
}