using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.AI;
using System.Reflection;


public class Raycaster : MonoBehaviour
{
    //Objet a spawn lors de l'appui pour indiquer la destination de l'IA
    [SerializeField]
    GameObject placedPointPrefab;

    //Distance du raycast
    [SerializeField]
    float rayDistance = 2000.0f;

    //R�f�rence au personnage IA qu'on va d�placer
    [SerializeField]
    NavMeshAgent agent;

    //DEBUG
    [SerializeField]
    GameObject prefabToDestroy;

    private void Start()
    {
        //On arr�te le d�placement de l'IA � l'initialisation, si la r�f�rence n'est pas null
        if (agent != null)
        {
            agent.isStopped = true;
        }
    }

    private void Update()
    {
    }

    //On active le enhanced touch support a l'initialisation de l'objet
    private void OnEnable()
    {
        TouchSimulation.Enable();
        EnhancedTouchSupport.Enable();
        //On �coute les �v�nements de type onFingerDown de l'input manager
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    //On d�sactive le enhanced touch support lors de la d�sactivation de l'objet
    private void OnDisable()
    {
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
        //On arr�te d'�couter les �vents de type onFingerDown
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(UnityEngine.InputSystem.EnhancedTouch.Finger finger)
    {
        //Si un seul doigt est press�
        if (finger.index != 0) return;

        //On cr�� un rayon depuis l'endroit ou on a touch� l'�cran
        Ray ray = Camera.main.ScreenPointToRay(finger.currentTouch.screenPosition);
        //Variable pour stocker les donn�es du hit du rayon
        RaycastHit hit;

        //Si en faisant un rayon on touche un objet
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            //Variable pour stocker les donn�es du hit sur le navMesh
            NavMeshHit navHit;

            //On v�rifie que le point qui est entr� en colision avec le sol (cube) est sur le navmesh
            if (NavMesh.SamplePosition(hit.point, out navHit, rayDistance, NavMesh.AllAreas))
            {
                //On stocke la position sur le navmesh
                Vector3 result = navHit.position;

                //On indique a notre agent (Personage IA) l'endroit ou il doit ce rendre 
                agent.destination = result;
                agent.Move(result);
            }
        }
    }
}
