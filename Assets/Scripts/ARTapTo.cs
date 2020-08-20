using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARRaycastManager))]
public class ARTapTo : MonoBehaviour
{
    public GameObject gameObjectToInstantiate;

    private GameObject spawnObject;
    private ARRaycastManager _arRaycasterManager;
    private Vector2 touchPos;
    private List<GameObject> Placeprefablist = new List<GameObject>();

    [SerializeField]
    private int maxPrefabcount = 0;
    private int placeprefabcount;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _arRaycasterManager = GetComponent<ARRaycastManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }


    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if(_arRaycasterManager.Raycast(touchPosition, hits, trackableTypes:TrackableType.PlaneWithinPolygon))
        {
            var hitPose = hits[0].pose;

            if(placeprefabcount < maxPrefabcount)
                {
                    spawPrefab(hitPose);
                }
        }
    }
    private void spawPrefab(Pose hitPose)
    {
        spawnObject = Instantiate(gameObjectToInstantiate, hitPose.position, hitPose.rotation);
        Placeprefablist.Add(spawnObject);
        placeprefabcount++;
    }
}

