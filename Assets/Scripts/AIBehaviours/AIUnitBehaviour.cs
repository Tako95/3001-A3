//name: Philip Okoye
//ID: 101484841

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIUnitBehaviour : MonoBehaviour
{
    private float engagementRange = 100;
    private float attackRange = 50;

    LayerMask unitLayerMask;

    List<Unit> detectedEnemies = new List<Unit>();

    TurretControl turretControl;

    Launcher launcher;

    Unit Target = null;

    private void Start()
    {
        unitLayerMask = LayerMask.GetMask("Unit");

        turretControl = GetComponentInChildren<TurretControl>();

        launcher = GetComponentInChildren<Launcher>();
    }

    private void Update()
    {
        DetectEnemyUnits();

        Target = FindNearest(detectedEnemies);

        Attack(Target);
    }

    private void Attack(Unit Target)
    {
        if (Target == null && attackRange >= 50)
        {
            launcher.CeaseTriggerPull();

            turretControl.SetDesiredAngularVelocity(0);
        }
        else
        {
            bool isMoving = GetComponent<Rigidbody>().velocity.magnitude > 0.5;

            if (isMoving == true)
            {
                launcher.CeaseTriggerPull();
            }
            else if (isMoving == false)
            {
                launcher.BeginTriggerPull();
            }

            //launcher.BeginTriggerPull();

            Vector3 toTarget = Target.transform.position - turretControl.transform.position;
            float angleToTarget = Vector3.SignedAngle(turretControl.transform.forward, toTarget, Vector3.up);
            float angleError = Mathf.Abs(angleToTarget);

            if (angleError < 10)
            {
                turretControl.SetDesiredAngularVelocity(angleToTarget);
            }
            else if(angleToTarget < 0)
            {
                turretControl.SetDesiredAngularVelocity(-600);
            }
            else if (angleToTarget > 0)
            {
                turretControl.SetDesiredAngularVelocity(600);  
            }

            Debug.DrawLine(transform.position, Target.transform.position, Color.red);
        }
    }

    private Unit FindNearest(List<Unit> detectedEnemies)
    {
        float closestDistance = float.PositiveInfinity;
        Unit closestUnit = null;

        foreach(Unit unit in detectedEnemies)
        {
            float distance = Vector3.Distance(unit.transform.position, transform.position);

            if (distance < closestDistance)
            {
                closestUnit = unit;
                closestDistance = distance;
            }
        }

        return closestUnit;
    }

    private void DetectEnemyUnits()
    {
        detectedEnemies.Clear();

        Team MyTeam = GetComponent<Unit>().Team;

        Collider[] colliders = Physics.OverlapSphere(transform.position, engagementRange, unitLayerMask);
       
        foreach (Collider collider in colliders)
        {
            Unit contact = collider.GetComponent<Unit>();
            if (contact == null) continue;

            bool isMyTeam = (contact.Team == MyTeam);

            if (isMyTeam == true) continue;

            Vector3 ToEnemy = collider.transform.position - transform.position;
            Ray LOSRay = new Ray(transform.position, ToEnemy);

            bool didhit = Physics.Raycast(LOSRay, out RaycastHit hitInfo, engagementRange);
            if (didhit)
            {
                if (hitInfo.collider == collider)
                {
                    detectedEnemies.Add(contact);

                    Debug.DrawLine(transform.position, contact.transform.position, Color.yellow);
                }
            }
        }
    }
}
