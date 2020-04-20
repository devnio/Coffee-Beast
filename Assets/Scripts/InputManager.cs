using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public Player player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 vel = Vector3.zero;
        vel.x = Input.GetAxis("Horizontal");
        vel.z = Input.GetAxis("Vertical");
        this.player.UpdateVelocityValues(vel);

        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Space))
        {
            this.player.Interact();
        }
    }
}
