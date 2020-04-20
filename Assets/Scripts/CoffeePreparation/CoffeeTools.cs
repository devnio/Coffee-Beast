using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeTools : MonoBehaviour
{
    public Player player;
    public BarLoader barLoader;
    public CoffeeTools[] cupAndHandle;

    public Transform targetCupTransform;
    public Transform targetManicoTransform;

    private Vector3 initTransformCup_pos;
    private Quaternion initTransformCup_orient;

    private Vector3 initTransformManico_pos;
    private Quaternion initTransformManico_orient;

    private bool isCupPlaced;
    private bool isManicoPlaced;
    private bool isButtonPressed;

    private void Start() {
        if (this.CompareTag("Cup"))
        {
            initTransformCup_pos = this.transform.position;
            initTransformCup_orient = this.transform.rotation;
        }
        else if (this.CompareTag("Manico"))
        {
            initTransformManico_pos = this.transform.position;
            initTransformManico_orient = this.transform.rotation;
        }
    }

    private void Update() {
        if (isButtonPressed)
        {
            if (!this.barLoader.isCompleted())
            {
                this.barLoader.Step(Time.deltaTime);
            }
            else
            {
                FinishMachine();
            }
        }
    }

    private void FinishMachine()
    {
        this.barLoader.Reset(0, 3f, false, true);
        this.isButtonPressed = false;
        this.cupAndHandle[0].PlayerCarryCup();
        this.cupAndHandle[1].ResetMe();
        CameraFollow.Instance.GoBackToNormal();
    }

    private void PlayerCarryCup()
    {
        this.player.CarryCup(this.transform, this);
    }

    private void OnMouseDown() {
        if (!isButtonPressed)
        {
            if (this.CompareTag("CoffeeMachineButton"))
            {
                if (this.cupAndHandle[0].isCupPlaced && this.cupAndHandle[1].isManicoPlaced)
                {
                    this.isButtonPressed = true;
                }
            }
            else
            {
                MoveToTarget();
            }
        }
        
    }

    private void MoveToTarget()
    {
        if (this.CompareTag("Cup"))
        {
            this.isCupPlaced = true;
            this.transform.position = this.targetCupTransform.position;
            this.transform.rotation = this.targetCupTransform.rotation;
        }
        else if (this.CompareTag("Manico"))
        {
            this.isManicoPlaced = true;
            this.transform.position = this.targetManicoTransform.position;
            this.transform.rotation = this.targetManicoTransform.rotation;
        }
    }

    public void ResetMe()
    {
        if (this.CompareTag("Cup"))
        {
            this.isCupPlaced = false;
            this.transform.position = this.initTransformCup_pos;
            this.transform.rotation = this.initTransformCup_orient;
        }
        else if (this.CompareTag("Manico"))
        {
            this.isManicoPlaced = false;
            this.transform.position = this.initTransformManico_pos;
            this.transform.rotation = this.initTransformManico_orient;
        }
    }
}
