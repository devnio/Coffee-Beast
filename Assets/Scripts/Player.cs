using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Objects")]
    public Rigidbody rb;
    public Transform unibalance;
    public Transform objectSpot;

    [Header("Movement")]
    public float maxVelocity;
    public float velSpeed;
    public float rotationSpeed;
    public float velDamping;

    private Quaternion targetOrient;
    private float orientationThreshold;

    private bool isInCartonZone;
    private int isCarryingCartonId;

    private bool isInBeansZone;

    private void Start() {
        this.isCarryingCartonId = -1;
        this.orientationThreshold = 0.1f;
    }

    private void Update() {
        float targetAngle = Mathf.Atan2(this.rb.velocity.x, this.rb.velocity.z) * Mathf.Rad2Deg;
        this.targetOrient = Quaternion.Euler(0, targetAngle, 0);  
        if (Quaternion.Angle(this.transform.rotation, this.targetOrient) > orientationThreshold)
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, 
                                                      this.targetOrient, 
                                                      this.rotationSpeed * Time.deltaTime);
        } 

        this.unibalance.localRotation = Quaternion.Euler(this.rb.velocity.magnitude/this.maxVelocity*90f, 0, 0);
    }

    private void FixedUpdate() {
        this.rb.velocity -= this.velDamping * this.rb.velocity;
        if (this.rb.velocity.magnitude > this.maxVelocity)
        {
            Vector3.ClampMagnitude(this.rb.velocity, this.maxVelocity);
        }
    }

    public void Move(Vector3 dir)
    {
        this.rb.AddForce(dir*this.velSpeed, ForceMode.Force);
    }

    private void CarryObject(Transform t)
    {
        t.position = this.objectSpot.position;
        t.SetParent(this.objectSpot);
    }

    public void Interact()
    {
        if (this.isInCartonZone)
        {
            TakeCarton();
        }
        if (this.isInBeansZone)
        {
            DropCarton();
            // TODO: spawn bean in the zone
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
    }

    // ======================
    // CARTON INTERACTION
    // ======================
    private void TakeCarton()
    {
        if (this.isCarryingCartonId == -1)
        {
            Carton c = SpawnManager.Instance.GetCarton();
            if (c != null)
            {
                this.isCarryingCartonId = c.id;
                CarryObject(c.carton.transform);
            }
        }
    }
    private void DropCarton()
    {
        if (this.isCarryingCartonId != -1)
        {
            SpawnManager.Instance.DisposeCarton(isCarryingCartonId);
            this.isCarryingCartonId = -1;
        }
    }

    public int GetCartonCarryId()
    {
        return this.isCarryingCartonId;
    }

    

}
