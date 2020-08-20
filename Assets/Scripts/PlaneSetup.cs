using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.AI;

[RequireComponent(typeof(ARRaycastManager))]
public class PlaneSetup : MonoBehaviour
{
    //PlaneDetect variables
    private ARPlaneManager _arPlaneManager;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;
    private int status = 1;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public static GameObject savedPlane;

    //RuntimeBake variables
    Vector3 BoundsCenter = Vector3.zero;
    Vector3 BoundsSize = new Vector3(512f, 4000f, 512f);
    LayerMask BuildMask;
    LayerMask NullMask;
    NavMeshData NavMeshData;
    NavMeshDataInstance NavMeshDataInstance;

    // Target Instantiate variables
    public int numTargets;
    public GameObject targetPrefab;
    public GameObject targetHolder;
    public Canvas setupCanvas;
    public Canvas playCanvas;

    void Awake()
    {
        // For plane detection
        _arPlaneManager = GetComponent<ARPlaneManager>();
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }

    void Start()
    {
        BuildMask = ~0;
        NullMask = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (status == 2)
        {
            BuildNavMesh();
        }
        if (status == 3)
        {
            TargetInstantiate();
            status = 4;
        }
        if (status == 4)
        {
            setupCanvas.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
            //Start button instantiate.
            setupCanvas.transform.GetChild(1).gameObject.SetActive(true);
            status = 5;
        }
        if (status == 1/* && DetectTouch(out Vector2 touchPosition)*/)
        {
            if (setupCanvas.transform.GetChild(0).GetChild(0).gameObject.activeSelf)
            {
                foreach (ARPlane plane in _arPlaneManager.trackables)
                {
                    setupCanvas.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    setupCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    break;
                }
            }
            if (DetectTouch(out Vector2 touchPosition))
            {
                if (_arRaycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
                {
                    var planeId = hits[0].trackableId;
                    foreach (ARPlane plane in _arPlaneManager.trackables)
                    {
                        if (plane.trackableId != planeId)
                        {
                            //plane.gameObject.SetActive(false);
                            Destroy(plane.gameObject);
                        }
                        else
                        {
                            savedPlane = plane.gameObject;
                        }
                    }
                    _arPlaneManager.enabled = false;
                    _arRaycastManager.enabled = false;
                    status = 2;
                    setupCanvas.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                }
            }
        }
    }

    bool DetectTouch(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }

    void BuildNavMesh()
    {
        AddNavMeshData();
        Build();
        UpdateNavmeshData();
        status = 3;
    }

    void AddNavMeshData()
    {
        if (NavMeshData != null)
        {
            if (NavMeshDataInstance.valid)
            {
                NavMesh.RemoveNavMeshData(NavMeshDataInstance);
            }
            NavMeshDataInstance = NavMesh.AddNavMeshData(NavMeshData);
        }
    }

    void UpdateNavmeshData()
    {
        StartCoroutine(UpdateNavmeshDataAsync());
    }

    IEnumerator UpdateNavmeshDataAsync()
    {
        AsyncOperation op = NavMeshBuilder.UpdateNavMeshDataAsync(
            NavMeshData,
            NavMesh.GetSettingsByID(0),
            GetBuildSources(BuildMask),
            new Bounds(BoundsCenter, BoundsSize));
        yield return op;

        AddNavMeshData();
        Debug.Log("Update finished " + Time.realtimeSinceStartup.ToString());
    }

    void Build()
    {
        NavMeshData = NavMeshBuilder.BuildNavMeshData(
            NavMesh.GetSettingsByID(0),
            GetBuildSources(NullMask),
            new Bounds(BoundsCenter, BoundsSize),
            Vector3.zero,
            Quaternion.identity);
        AddNavMeshData();
    }

    List<NavMeshBuildSource> GetBuildSources(LayerMask mask)
    {
        List<NavMeshBuildSource> sources = new List<NavMeshBuildSource>();
        NavMeshBuilder.CollectSources(
            new Bounds(BoundsCenter, BoundsSize),
            mask,
            NavMeshCollectGeometry.PhysicsColliders,
            0,
            new List<NavMeshBuildMarkup>(),
            sources);
        Debug.Log("Sources found: " + sources.Count.ToString());
        return sources;
    }

    public void NewGame()
    {
        //Start button generates Ammo, activates Wander script on Targets, activates play UI.
        // Just activate Ammo
        this.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(true);
        //Get children from targetHolder and loop through them to activate wander script
        TargetActivate();
        //Activate playCanvas
        playCanvas.gameObject.SetActive(true);
        //Deactivate setupCanvas
        setupCanvas.gameObject.SetActive(false);
    }

    public void TargetInstantiate()
    {
        GameObject newTarget;
        targetHolder.transform.position = savedPlane.transform.position;
        for (int i = 0; i < numTargets; i++)
        {
            newTarget = Instantiate(targetPrefab, savedPlane.transform.position + (Vector3.up * 0.1f), Quaternion.identity, targetHolder.transform);
            //newTarget.transform.position = savedPlane.transform.position + (Vector3.up * 0.1f);
        }
    }

    public void TargetActivate()
    {
        for (int i = 0; i < targetHolder.transform.childCount; i++)
        {
            targetHolder.transform.GetChild(i).GetComponent<TargetBehaviour>().enabled = true;
        }
    }
}