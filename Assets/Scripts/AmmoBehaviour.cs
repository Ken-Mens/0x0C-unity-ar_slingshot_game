using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoBehaviour : MonoBehaviour
{
    public Transform holder;
    public LayerMask aLayer;
    public LineRenderer lineRenderer;
    bool draggable = false;
    bool fired = false;
    Rigidbody ammo;
    Camera mCamera;
    private float sensitivity = 100.0f;
    public Text ammoCount;
    int ammoInt = 7;
    public Text scoreCount;
    int scoreInt = 0;
    public Transform targetHolder;
    public GameObject playAgain;
    public GameObject origin;

    void Start()
    {
        ammo = GetComponent<Rigidbody>();
        mCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!fired)
                    {
                        RaycastHit hit;
                        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, aLayer))
                        {
                            draggable = true;
                        }
                    }
                    break;
                case TouchPhase.Moved:
                    if (draggable)
                    {
                        ammo.transform.Translate(new Vector3(touch.deltaPosition.x * Time.deltaTime, touch.deltaPosition.y * Time.deltaTime, 0.0f) / sensitivity, Space.Self);
                        float zMag = new Vector2(holder.position.x - ammo.transform.position.x, holder.position.y - ammo.transform.position.y).magnitude;
                        ammo.transform.localPosition = new Vector3(ammo.transform.localPosition.x, ammo.transform.localPosition.y, zMag * -1);
                        //LINE RENDERING
                        lineRenderer.gameObject.SetActive(true);
                        lineRenderer.transform.position = ammo.transform.position;
                        lineRenderer.positionCount = 1;
                        lineRenderer.SetPosition(0, ammo.position);
                        int resolution = 30;
                        for (int i = 1; i <= resolution; i++)
                        {
                            float simulationTime = i / (float)resolution * 2f;
                            Vector3 displacement = (holder.position - ammo.transform.position) * 10f * simulationTime + Vector3.up * -9.81f * simulationTime * simulationTime / 2f;
                            Vector3 drawPoint = ammo.position + displacement;
                            lineRenderer.positionCount++;
                            lineRenderer.SetPosition(lineRenderer.positionCount - 1, drawPoint);
                        }
                        //END LINE RENDERING
                    }
                    break;
                case TouchPhase.Ended:
                    if (draggable)
                    {
                        lineRenderer.gameObject.SetActive(false);
                        Vector3 force = (holder.position - ammo.transform.position);
                        ammo.isKinematic = false;
                        ammo.AddForce(force * 10, ForceMode.VelocityChange);
                        fired = true;
                        draggable = false;
                        ammo.transform.SetParent(null);
                        ammoInt--;
                        ammoCount.text = ammoInt.ToString();
                    }
                    break;
            }
        }
        if (fired == true && ammo.transform.position.y < PlaneSetup.savedPlane.transform.position.y)
        {
            AmmoReset();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            scoreInt += 10;
            scoreCount.text = scoreInt.ToString();
            Destroy(collision.gameObject);
        }
        AmmoReset();
    }
    public void AmmoReset()
    {
        ammo.isKinematic = true;
        ammo.velocity = Vector3.zero;
        ammo.angularVelocity = Vector3.zero;
        ammo.gameObject.transform.position = holder.position;
        Vector3 rotation = new Vector3(mCamera.transform.eulerAngles.x, mCamera.transform.eulerAngles.y, mCamera.transform.eulerAngles.z);
        ammo.gameObject.transform.eulerAngles = rotation;
        ammo.transform.SetParent(holder);
        fired = false;
        if (ammoInt == 0 || targetHolder.childCount <= 0)
        {
            playAgain.SetActive(true);
            fired = true;
        }
    }

    public void NewGamePlus()
    {
        ammoInt = 7;
        ammoCount.text = ammoInt.ToString();
        scoreInt = 0;
        scoreCount.text = scoreInt.ToString();
        for (int i = 0; i < targetHolder.childCount; i++)
        {
            Destroy(targetHolder.GetChild(i).gameObject);
        }
        origin.GetComponent<PlaneSetup>().TargetInstantiate();
        origin.GetComponent<PlaneSetup>().TargetActivate();
        fired = false;
        playAgain.SetActive(false);
    }
}