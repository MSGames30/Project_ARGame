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

    //Référence au personnage IA qu'on va déplacer
    [SerializeField]
    NavMeshAgent agent;

    //DEBUG
    [SerializeField]
    GameObject prefabToDestroy;

    private void Start()
    {
        //On arrête le déplacement de l'IA à l'initialisation, si la référence n'est pas null
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
        //On écoute les événements de type onFingerDown de l'input manager
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown += FingerDown;
    }

    //On désactive le enhanced touch support lors de la désactivation de l'objet
    private void OnDisable()
    {
        TouchSimulation.Disable();
        EnhancedTouchSupport.Disable();
        //On arrête d'écouter les évents de type onFingerDown
        UnityEngine.InputSystem.EnhancedTouch.Touch.onFingerDown -= FingerDown;
    }

    private void FingerDown(UnityEngine.InputSystem.EnhancedTouch.Finger finger)
    {
        //Si un seul doigt est pressé
        if (finger.index != 0) return;

        //On créé un rayon depuis l'endroit ou on a touché l'écran
        Ray ray = Camera.main.ScreenPointToRay(finger.currentTouch.screenPosition);
        //Variable pour stocker les données du hit du rayon
        RaycastHit hit;

        //Si en faisant un rayon on touche un objet
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            //Variable pour stocker les données du hit sur le navMesh
            NavMeshHit navHit;

            //On vérifie que le point qui est entré en colision avec le sol (cube) est sur le navmesh
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
