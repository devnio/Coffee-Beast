using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bean : MonoBehaviour
{
    public Rigidbody rb;
    public float spawnForce;
    
    public BeanPiece[] pieces;

    private Vector3[] position;
    private Quaternion[] orientation;
    
    public BarLoader smashLoader {get; private set;}
    public Transform cameraCoffeeZone {get; private set;}

    public void BeanHasBeenSmashed()
    {
        ResetPieces();
        smashLoader.Reset(0, 10, false, true);
        CameraFollow.Instance.ZoomIntoCoffeeZone(this.cameraCoffeeZone);
        SpawnManager.Instance.DisposeBean(this);
    }

    private void ResetPieces()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        for(int i = 0; i < pieces.Length; i++)
        {
            pieces[i].ResetChildsAndMe();
        }
    }

    private void SmashIntoPieces()
    {
        this.transform.GetChild(0).gameObject.SetActive(false);
        SetKinematic(true, false);
        for(int i = 0; i < pieces.Length; i++)
        {
            pieces[i].gameObject.SetActive(true);
            pieces[i].ActivateAfterTime(0.6f);
        }
    }

    public void ShootBean(Transform transform, bool addForce = true)
    {
        this.rb.useGravity = true;
        this.rb.isKinematic = false;
        this.gameObject.SetActive(true);
        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.position = transform.position;
        this.transform.parent = null;
        if (addForce) rb.AddForce(new Vector3(1f, -1f, -1f) * spawnForce, ForceMode.Impulse);
    }

    public void SetKinematic(bool state, bool gravity = false)
    {
        this.rb.isKinematic = state;
        this.rb.useGravity = gravity;
    }

    public void SetSmashLoader(BarLoader bl)
    {
        this.smashLoader = bl; 
    }

    public void SetCameraCoffeeZone(Transform t)
    {
        this.cameraCoffeeZone = t; 
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("SmashTool"))
        {
            SmashIntoPieces();
            this.smashLoader.Step(1);
        }
    }
}
