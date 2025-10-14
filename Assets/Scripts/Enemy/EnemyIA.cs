using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    [Header("IA Follow Navigation")]
    public float minDistanceToFollow = 1.2f;
    public float minDistanceToPoint = 1.0f;
    public float timeToStartNavigation = 3.0f;
    public float spdOfNavigation = 1.0f;
    public float spdOfFollow = 3.0f;

    [Header("IA Follow Navigation")]
    public Transform eyes;
    public float visionRadius = 10f;
    public float visionAngle = 45f;
    public float timeOfNavigation;
    public GameObject[] navegationsPoints;

    private float timerNav;
    private int pointIndex;
    private bool canSeePlayer = false;

   // private Animator anim;
    private Transform target;
    private NavMeshAgent navMesh;

    void Start()
    {
       // anim = GetComponent<Animator>();
        navMesh = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        navegationsPoints = GameObject.FindGameObjectsWithTag("NavigationPoint");
        pointIndex = GetRandomPointIndex();
    }

    void Update()
    {
        timerNav = Mathf.Clamp(timerNav -= Time.deltaTime, 0, timeOfNavigation);
        canSeePlayer = CanSeePlayer();

        if (canSeePlayer || timerNav > 0)
            Follow();
        else
            Navegation();
    }

    private bool CanSeePlayer()
    {
        if (target == null)
            return false;

        var directionToTarget = (target.position - eyes.position).normalized;

        if (Vector3.Angle(eyes.forward, directionToTarget) < visionAngle / 2)
        {
            RaycastHit hit;
            if (Physics.Raycast(eyes.position, directionToTarget, out hit, visionRadius))
            {
                if (hit.transform.CompareTag("Player"))
                {
                    timerNav = timeOfNavigation;
                    return true;
                }
            }
        }
        return false;
    }

    private void Follow()
    {
        navMesh.speed = spdOfFollow;

        if (Vector3.Distance(transform.position, target.position) > minDistanceToFollow)
        {
           // anim.SetBool("Follow", true);
            navMesh.enabled = true;
            navMesh.SetDestination(target.position);
        }
        else
        {
            var lookPos = new Vector3(target.position.x, 0, target.position.z);
            transform.LookAt(lookPos);
            navMesh.enabled = false;
           // anim.SetBool("Follow", false);
        }
    }

    private void Navegation()
    {
        //anim.SetBool("Follow", false);
        navMesh.speed = spdOfNavigation;

        if (navMesh.enabled && Vector3.Distance(transform.position, navegationsPoints[pointIndex].transform.position) > minDistanceToPoint)
        {
            //anim.SetBool("Navegation", true);
            navMesh.SetDestination(navegationsPoints[pointIndex].transform.position);
        }
        else
        {
            navMesh.enabled = false;
            //anim.SetBool("Navegation", false);
            pointIndex = GetRandomPointIndex();
            StartCoroutine(StartNavigation());
        }
    }

    private int GetRandomPointIndex()
    {
        var i = UnityEngine.Random.Range(0, (navegationsPoints.Length - 1));

        if (pointIndex == i) // Verifica se o ponto gerado é igual ao atual
        {
            return GetRandomPointIndex();
        }

        var enemys = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemys)
        {
            if (enemy.GetComponent<EnemyIA>().pointIndex == i)
            {
                return GetRandomPointIndex();
            }
        }
        return i;
    }

    private IEnumerator StartNavigation()
    {
        yield return new WaitForSeconds(timeToStartNavigation);
        navMesh.enabled = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = canSeePlayer ? Color.blue : Color.red;
        Quaternion leftRayRotation = Quaternion.AngleAxis(-visionAngle / 2, transform.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(visionAngle / 2, transform.up);
        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawRay(eyes.position, leftRayDirection * visionRadius);
        Gizmos.DrawRay(eyes.position, rightRayDirection * visionRadius);
        Gizmos.DrawLine(eyes.position, eyes.position + leftRayDirection * visionRadius);
        Gizmos.DrawLine(eyes.position, eyes.position + rightRayDirection * visionRadius);
    }
}