using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTo : MonoBehaviour
{
    Vector2 startpos, endpos, direction;
    float touchtimestart, touchtimefinito, timeinterval;

    [SerializeField]
    float throwForceInXandY = 1f;

    [SerializeField]
    float throwForceInZ = 50f;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Began)
        {
            touchtimestart = Time.time;
            startpos = Input.GetTouch(0).position;
        }

        // If you release your finger

        if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            touchtimefinito = Time.time;

            timeinterval = touchtimefinito - touchtimestart;

            endpos = Input.GetTouch(0).position;

            direction = startpos - endpos;

            // adds force to ball rigidbody
            rb.isKinematic = false;
            rb.AddForce(-direction.x * throwForceInXandY, -direction.y * throwForceInXandY, throwForceInZ / timeinterval);

            // Destroys ball in 4 seconds
            Destroy(gameObject, 3f);
        }
    }
}
