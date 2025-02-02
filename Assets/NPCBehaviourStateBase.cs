public enum NPCBehaviourType
{
    Idle,
    Search,
    Attack,
    Defence
}

public interface INPCBehaviour
{
    NPCBehaviourType BehaviourType { get; }
    INPCHealth Health { get; }
    INPCPathFinder PathFinder { get; }
    void StartState(NPCBehaviourType previousBehaviour);
    void FinishState();
}

public abstract class NPCBehaviourStateBase : INPCBehaviour
{
    private INPCHealth _health;
    private INPCPathFinder _pathFinder;
    public abstract NPCBehaviourType BehaviourType { get; }

    public INPCHealth Health => _health;

    public INPCPathFinder PathFinder => _pathFinder;
    public NPCBehaviourStateBase(INPCHealth health, INPCPathFinder pathFinder)
    {
        _health = health;
        _pathFinder = pathFinder;
    }

    public abstract void StartState(NPCBehaviourType previousBehaviour);

    public abstract void FinishState();
}
