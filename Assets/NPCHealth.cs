using Mirror;
using UniRx;
using UnityEngine;

public interface INPCHealth
{
    /// <summary>
    /// Reactive property for subscription on changing hp localy (use in UI)
    /// </summary>
    IReadOnlyReactiveProperty<int> HealthRx { get; }
    void TakeDamage(int amount);
    void RestoreHealth();
}

public class NPCHealth : NetworkBehaviour, INPCHealth
{
    [SerializeField] private int _maxHealth = 100;

    [SyncVar(hook = nameof(OnHealthChanged))]
    private int _healthPoints;

    private readonly ReactiveProperty<int> _healthRx = new ReactiveProperty<int>();

    public IReadOnlyReactiveProperty<int> HealthRx => _healthRx;

    /// <summary>
    /// Executes on change of health parameters
    /// </summary>
    /// <param name="oldValue"></param>
    /// <param name="newValue"></param>
    private void OnHealthChanged(int oldValue, int newValue)
    {
        _healthRx.Value = newValue;
    }

    public void TakeDamage(int amount)
    {
        if (isServer)
        {
            // Обновляем SyncVar, чтобы разослать клиентам
            _healthPoints = Mathf.Max(_healthPoints - amount, 0);
        }
        else
        {
            // Если не сервер — отправляем команду
            CmdTakeDamage(amount);
        }

    }

    [Command]
    private void CmdTakeDamage(int damage)
    {
        TakeDamage(damage);
    }

    public void RestoreHealth()
    {
        if (isServer)
        {
            _healthPoints = _maxHealth;
        }
        else
        {
            CmdRestoreHealth();
        }
    }

    [Command]
    private void CmdRestoreHealth()
    {
        RestoreHealth();
    }
}