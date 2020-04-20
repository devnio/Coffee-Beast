using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : Singleton<CameraFollow>
{
    public Transform baseView;
    public Player player;
    public float rotationSpeed;

    private Quaternion initOrient;
    private Quaternion targetOrient;

    private Vector3 initForward;
    private float orientationThreshold;

    private bool follow;

    // Start is called before the first frame update
    void Start()
    {
        this.follow = true;
        this.initOrient = Quaternion.Euler(0f, 60f, 0f) * this.transform.rotation; //this.transform.rotation;//
        this.orientationThreshold = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (follow)
        {
             // float targetAngle = -Mathf.Atan2(this.player.transform.position.x, this.player.transform.position.z) * Mathf.Rad2Deg;
            Vector3 dir = this.player.transform.position - this.transform.position;
            Vector3 diff = this.initForward - dir;
            float sign = (dir.x > this.initForward.x)? -1.0f : 1.0f;

            // float targetAngle = Vector3.Angle(this.initForward, dir);
            float targetAngle = Vector3.Angle(Vector3.left, diff) * sign;
            // this.targetOrient = Quaternion.LookRotation(dir, this.transform.up); 
            this.targetOrient = Quaternion.Euler(0, targetAngle, 0) * this.initOrient;  
            if (Quaternion.Angle(this.transform.rotation, this.targetOrient) > orientationThreshold)
            {
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, 
                                                        this.targetOrient, 
                                                        this.rotationSpeed * Time.deltaTime);
            } 
        }
    }


    public void ZoomIntoSmashZone(Transform t)
    {
        follow = false;
        StartCoroutine(C_ZoomInto(t));
        this.player.ActivateSmashMode();
    }

    public void ZoomIntoCoffeeZone(Transform t)
    {
        follow = false;
        StartCoroutine(C_ZoomInto(t));
        this.player.ActivateCoffeeMode();
    }

    public void GoBackToNormal()
    {
        StartCoroutine(C_ZoomInto(baseView, true));
    }

    private IEnumerator C_ZoomInto(Transform t, bool follow = false)
    {  
        bool a = true;
        bool b = true;
        while (a || b)
        {
            if ((Quaternion.Angle(this.transform.rotation, t.rotation) > orientationThreshold))
            {
                this.transform.rotation = Quaternion.Lerp(this.transform.rotation, 
                                                    t.rotation, 
                                                    this.rotationSpeed * Time.deltaTime);
            }
            else { a = false; }

            Vector3 dir = t.position - this.transform.position;
            if (dir.magnitude > 7f)
            {
                this.transform.position += dir.normalized * 100f * Time.deltaTime;
            }
            else { b = false; }


            yield return null;
        } 
        this.follow = follow;
    }

}
