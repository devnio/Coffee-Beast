using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Objects")]
    public Beast beast;
    public Rigidbody rb;
    public Transform unibalance;
    public Transform objectSpot;
    
    [Header("Smash Zone")]
    public Transform tableBeanSpot;
    public Transform cameraSmashZone;
    public Transform cameraCoffeeZone;
    public Transform smashTool;

    public BarLoader smashLoader;

    [Header("Movement")]
    public float maxVelocity;
    public float velSpeed;
    public float rotationSpeed;
    public float velDamping;

    private Vector3 inputVel;

    private Quaternion targetOrient;
    private float orientationThreshold;

    private bool isInCartonZone;
    private int isCarryingCartonId;

    private bool isInBeansZone;
    private Bean carryingBean;
    
    private bool isInSmashZone;
    private bool smashMode;

    public bool coffeeMode {get; private set;}
    public CoffeeTools Cup;

    private bool isInBeastZone;

    private void Start() {
        this.isCarryingCartonId = -1;
        this.orientationThreshold = 0.1f;
    }

    private void Update() {
        float targetAngle = Mathf.Atan2(this.inputVel.x, this.inputVel.z) * Mathf.Rad2Deg;
        this.targetOrient = Quaternion.Euler(0, targetAngle, 0);  
        if (Quaternion.Angle(this.transform.rotation, this.targetOrient) > orientationThreshold)
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, 
                                                      this.targetOrient, 
                                                      this.rotationSpeed * Time.deltaTime);
        } 

        this.unibalance.localRotation = Quaternion.Euler(this.inputVel.magnitude*40, 0, 0);

        if (this.smashMode)
        {
            Vector3 smashToolPos = Camera.main.transform.position;
            Vector3 worldPos = (Vector3.one*0.5f - Camera.main.ScreenToViewportPoint(Input.mousePosition)) * 25f + new Vector3(-3f, -8f, 0f);
            smashToolPos.x = this.smashTool.transform.position.x;
            smashToolPos.y = -worldPos.y;
            smashToolPos.z = worldPos.x;
            
            this.smashTool.transform.position = smashToolPos;
        }

        this.transform.Translate(inputVel * this.velSpeed * Time.deltaTime, Space.World);
    }

    public void UpdateVelocityValues(Vector3 vel)
    {
        inputVel.x = vel.x;
        inputVel.z = vel.z;
    }

    // private void FixedUpdate() {
    //     this.rb.MovePosition(this.rb.position + inputVel * this.velSpeed * Time.deltaTime);
    // }

    private void CarryObject(Transform t)
    {
        t.position = this.objectSpot.position; 
        t.SetParent(this.objectSpot);
    }

    public void Interact()
    {
        if (this.isInCartonZone  && (this.isCarryingCartonId == -1) && (this.carryingBean==null))
        {
            TakeCarton();
        }
        if (this.isInBeansZone && (this.carryingBean==null))
        {
            if (this.isCarryingCartonId != -1)
            {
                DropCarton();
                SpawnManager.Instance.SpawnBean();
            }
            else
            {
                TakeBean();
            }
        }
        if (this.isInSmashZone && (this.carryingBean != null))
        {
            // Smash the bean
            if (!this.smashMode)
            {
                DropBeanOnTable();
            }
        }
        if (this.isInBeastZone && this.coffeeMode && this.Cup != null)
        {
            FeedCoffeeToBeast();
        }
    }

    // ======================
    // ON TRIGGER INTERACTION
    // ======================
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("CartonInteractionZone"))
        {
            this.isInCartonZone = true;
        }
        else if (other.CompareTag("BeansInteractionZone"))
        {
            this.isInBeansZone = true;
        }    
        else if (other.CompareTag("SmashZone"))
        {
            this.isInSmashZone = true;
        }
        else if (other.CompareTag("BeastZone"))
        {
            this.isInBeastZone = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("CartonInteractionZone"))
        {
            this.isInCartonZone = false;
        }    
        else if (other.CompareTag("BeansInteractionZone"))
        {
            this.isInBeansZone = false;
        }    
        else if (other.CompareTag("SmashZone"))
        {
            this.isInSmashZone = false;
        }
        else if (other.CompareTag("BeastZone"))
        {
            this.isInBeastZone = false;
        }
    }

    // ======================
    // CARTON INTERACTION
    // ======================
    private void TakeCarton()
    {
        Carton c = SpawnManager.Instance.GetCarton();
        if (c != null)
        {
            this.isCarryingCartonId = c.id;
            CarryObject(c.carton.transform);
        }
    }
    private void DropCarton()
    {
        SpawnManager.Instance.DisposeCarton(isCarryingCartonId);
        this.isCarryingCartonId = -1;
    }

    public int GetCartonCarryId()
    {
        return this.isCarryingCartonId;
    }

    // ======================
    // BEAN INTERACTION
    // ======================
    private void TakeBean()
    {
        Bean bean = SpawnManager.Instance.GetBean();
        if (bean != null)
        {
            // this.isCarryingBean = true;
            this.carryingBean = bean;
            this.carryingBean.SetKinematic(true);
            CarryObject(bean.transform);
        }
    }

    public void RemoveBean()
    {
        this.carryingBean = null;
    }

    private void DropBeanOnTable()
    {
        this.carryingBean?.ShootBean(this.tableBeanSpot, false);
        this.carryingBean?.SetKinematic(false, true);
        this.carryingBean?.SetSmashLoader(this.smashLoader);
        this.carryingBean?.SetCameraCoffeeZone(this.cameraCoffeeZone);
        CameraFollow.Instance.ZoomIntoSmashZone(this.cameraSmashZone);
    }
    
    // ======================
    // SMASH INTERACTION
    // ======================
    public void ActivateSmashMode()
    {
        this.smashMode = true;
    }

    // ======================
    // COFFEE INTERACTION
    // ======================
    public void ActivateCoffeeMode()
    {
        this.smashMode = false;
        this.coffeeMode = true;
    }

    
    public void CarryCup(Transform t, CoffeeTools Cup)
    {
        CarryObject(t);
        this.Cup = Cup;
    }

    public void RemoveCup()
    {
        this.Cup.ResetMe();
        this.Cup.transform.parent = null;
        this.Cup = null;
    }

    // ======================
    // FEED INTERACTION
    // ======================
    public void FeedCoffeeToBeast()
    {
        this.coffeeMode = false;
        RemoveCup();
        this.beast.CoffeeShot();
    }

}
