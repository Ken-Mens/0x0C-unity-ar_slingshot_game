using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]

public class randomfloorplacement : MonoBehaviour
{
    private GameObject spawnNew;

    private ARRaycastManager RayRaycastManager;
    private ARPlaneManager arpPlaneManager;

    [SerializeField]
    public GameObject[] objectsArr;

    static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        RayRaycastManager = GetComponent<ARRaycastManager>();
        arpPlaneManager = GetComponent<ARPlaneManager>();
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    void Update()
    {
        if (TryGetTouchPosition(out Vector2 touchPosition))
            return;

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            if (RayRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
            {
                var hitpose = hits[0].pose;

                foreach (var plane in arpPlaneManager.trackables)
                {
                    plane.gameObject.SetActive(false);
                }

                arpPlaneManager.enabled = false;

                spawnNew = Instantiate(objectsArr[Random.Range(0, objectsArr.Length)], hitpose.position, hitpose.rotation);
            }
        }
    }
}
