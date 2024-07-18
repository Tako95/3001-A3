using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIUnitBehaviour : MonoBehaviour
{
    private float engagementRange = 100;
    
    private float attackRange;

    LayerMask unitLayerMask;

    private I_IFFChallengeable myIFF;

    List<Unit> detectableEnemies;

    Unit currentTarget = null;

    Launcher launcher;

    TurretControl turret;

    private void Start()
    {
        launcher = GetComponentInChildren<Launcher>();
        turret = GetComponentInChildren<TurretControl>();
        unitLayerMask = LayerMask.GetMask("Unit");
        myIFF = GetComponent<I_IFFChallengeable>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {

        }
    }

    private void FixedUpdate()
    {
        FindTargets();

        currentTarget = SelectTarget();

        if(currentTarget != null)
        {
            Debug.DrawLine(transform.position, currentTarget.transform.position, Color.yellow);

            Vector3 toTarget = currentTarget.transform.position - transform.position;

        }
        
    }

    public bool ShotCheck(Unit target, float angleTolerance)
    {
        //LOS check
        Vector3 toTarget = target.transform.position - transform.position;


        //distance check
        Vector3 ToTarget = target.transform.position - transform.position;
        float distance = ToTarget.magnitude;
        if (distance > attackRange)
        {
            return false;
        }

        //Shot angle check
       

        return false;
    }

    private Unit SelectTarget()
    {
        float closestUnitDistance = 0f;
        Unit closestUnit = null;

        foreach(Unit potentialTarget in detectableEnemies)
        {
            float distance = Vector3.Distance(transform.position, potentialTarget.transform.position);
            if (distance <= closestUnitDistance)
            {
                closestUnit = potentialTarget;
                closestUnitDistance = distance;
            }
        }

        return closestUnit;
    }

    public void FindTargets()
    {
        detectableEnemies.Clear();

        Collider[] colliders = Physics.OverlapSphere(transform.position, engagementRange, unitLayerMask);

        foreach (Collider collider in colliders)
        {
            Unit unit = collider.GetComponent<Unit>();

            if (unit != null)
            {
                Debug.Log("Found: " + unit.name);

                if (unit.Team == myIFF.Team) continue;

                Vector3 toOther = collider.transform.position - transform.position;

                Ray rayToOther = new Ray(transform.position, toOther.normalized);

                RaycastHit HitInfo;

                if (Physics.Raycast(rayToOther, out HitInfo, engagementRange))
                {
                    if (HitInfo.collider == collider)
                    {
                        detectableEnemies.Add(unit);
                        Debug.DrawLine(transform.position, unit.transform.position, Color.yellow, Time.fixedDeltaTime, false);
                    }
                }
            }
        }
    }

}
