using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beast : MonoBehaviour
{
    public BarLoader barLoader;
    public float coffeeShotPowerTime;
    public float timeBeforeDead;
    public float timePassed;

    public Animator animator;

    // Update is called once per frame
    void Update()
    {
        if (!this.barLoader.isCompleted())
        {
            this.timePassed += Time.deltaTime;
            this.barLoader.Step(Time.deltaTime);
            UIManager.Instance.UpdateScore(this.timePassed);
        }
        else
        {
            // game over
            Time.timeScale = 0f;
            UIManager.Instance.DisplayGameOverButton();
        }
    }

    public void CoffeeShot()
    {
        this.timeBeforeDead += coffeeShotPowerTime;
        this.barLoader.UpdateTargetValue(this.timeBeforeDead);
        this.animator.SetTrigger("HappyAnimation");
    }
}
