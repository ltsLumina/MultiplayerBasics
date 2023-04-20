using System;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent;

    Camera main;

    [Command] void CmdMove(Vector3 point)
    {
        if (!NavMesh.SamplePosition(point - transform.position, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;

        agent.SetDestination(hit.position);
    }

    override public void OnStartAuthority()
    {
        main = Camera.main;
    }

    [ClientCallback]
    void Update()
    {
        if (!isOwned || !Input.GetMouseButtonDown(0)) return;
        Ray ray = main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity)) return;
        CmdMove(hit.point);
    }
}