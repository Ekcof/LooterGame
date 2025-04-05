using Mirror;
using UnityEngine;
using UnityEngine.AI;

public interface INPCPathFinder
{
    void MoveTo(Vector3 destination);
}

public class NPCPathFinder : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private NetworkTransformBase _networkTransform;

    private void Start()
    {
        return;
        //
        _networkTransform.interpolatePosition = true;
        _networkTransform.interpolateRotation = true;
        if (isServer)
        {
            _agent.enabled = true;
        }
        else
        {
            _agent.enabled = false; // Отключить на клиентах
        }

    }

    [Command]
    public void CmdMoveTo(Vector3 destination)
    {
        if (isServer)
        {
            MoveTo(destination);
        }
    }

    public void MoveTo(Vector3 destination)
    {
        if (_agent.isActiveAndEnabled)
        {
            _agent.SetDestination(destination);
        }
    }
}
