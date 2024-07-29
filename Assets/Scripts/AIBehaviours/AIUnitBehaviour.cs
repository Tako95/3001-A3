//name: Philip Okoye
//ID: 101484841

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIUnitBehaviour : MonoBehaviour
{
    private float engagementRange = 100;

    private float attackRange = 150;

    LayerMask unitLayerMask;

    private I_IFFChallengeable myIFF;

    List<Unit> detectableEnemies = new List<Unit>();

    Unit Target = null;

    Launcher launcher;

    TurretControl turret;

    TankLocomotion movement;

    private void Start()
    {
        launcher = GetComponentInChildren<Launcher>();

        turret = GetComponentInChildren<TurretControl>();

        unitLayerMask = LayerMask.GetMask("Unit");

        myIFF = GetComponent<I_IFFChallengeable>();
    }

    private void Update()
    {
        FindTargets();
        
        Target = SelectTarget();

        ShotCheck(Target);
    }


    public void ShotCheck(Unit target)
    {
        if (target == null)
        {
            launcher.CeaseTriggerPull();
            turret.SetDesiredAngularVelocity(0);
        }
        else
        {
            if (movement.IsStoppedMoving())
            {
                launcher.BeginTriggerPull();
            }
            else
            {
                launcher.CeaseTriggerPull();
            }

            Vector3 toTarget = target.transform.position - turret.transform.position;
            float angleTotarget = Vector3.SignedAngle(turret.transform.forward, toTarget, Vector3.up);
            float angleError = Mathf.Abs(angleTotarget);

            if (angleError < 5)
            {
                turret.SetDesiredAngularVelocity(0);
            }
            else if (angleTotarget < 0)
            {
                turret.SetDesiredAngularVelocity(-100);
            }
            else if (angleTotarget > 0)
            {
                turret.SetDesiredAngularVelocity(100);
            }

            Debug.DrawLine(transform.position, target.transform.position, Color.red);
        }
    }

    private Unit SelectTarget()
    {
        float closestUnitDistance = 0f;
        Unit closestUnit = null;

        foreach (Unit potentialTarget in detectableEnemies)
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

    void FindTargets()
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
