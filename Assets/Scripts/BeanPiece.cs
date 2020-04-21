using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeanPiece : MonoBehaviour
{
    Vector3 initPos;
    Quaternion initOrient;
    public Bean bean;

    public BeanPiece[] subdivision;

    public bool active;

    private Coroutine activationCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        this.initPos = this.transform.localPosition;
        this.initOrient = this.transform.localRotation;
    }

    private void Subdivide()
    {
        for (int i = 0; i < subdivision.Length; i++)
        {
            subdivision[i].transform.position = this.transform.position;
            subdivision[i].gameObject.SetActive(true);
            subdivision[i].ActivateAfterTime(0.6f);
        }
        if (subdivision.Length > 0) ResetMe();
    }

    private void ResetMe() {
        this.transform.localPosition = this.initPos;
        this.transform.localRotation = this.initOrient;
        this.gameObject.SetActive(false);
        if (this.activationCoroutine != null)
        {
            StopCoroutine(this.activationCoroutine);
        }        
        this.active = false;
    }

    public void ResetChildsAndMe()
    {
        ResetMe();
        for(int i = 0; i < this.subdivision.Length; i++)
        {
            this.subdivision[i].ResetMe();
            this.subdivision[i].ResetChildsAndMe();
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.CompareTag("SmashTool") && this.active)
        {
            if (subdivision.Length > 0)
            {
                Subdivide();
            }
            this.bean.smashLoader.Step(1);
            if (this.bean.smashLoader.isCompleted())
            {
                this.bean.BeanHasBeenSmashed();
            }
        }
    }

    public void ActivateAfterTime(float time)
    {
        this.activationCoroutine = StartCoroutine(ActivateAfter(time));
    }

    private IEnumerator ActivateAfter(float time)
    {
        float passedTime = 0f;
        while (passedTime < time)
        {
            passedTime += Time.deltaTime;
            yield return null;
        }
        this.active = true;
    }
}
