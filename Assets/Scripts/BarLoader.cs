using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarLoader : MonoBehaviour
{
    public Transform loaderBar;
    
    public float targetValue;
    public float startValue;

    public bool changeColor;
    public bool inverseLoad;

    private float currValue;
    private Material material;
    
    private Vector3 initScale;
    private Vector3 initPos;
    private float loadedPercent;

    // Start is called before the first frame update
    void Start()
    {
        this.material = this.loaderBar.GetComponent<MeshRenderer>().material;
        this.initScale = this.loaderBar.localScale;
        this.initPos = this.loaderBar.localPosition;
        this.material.color = Color.blue;
    }

    // private void Update() {
    //     Step(Time.deltaTime);
    // }

    public void Reset(float startValue = 0f, float targetValue = 0f, bool inverse = false, bool color = false)
    {
        this.loadedPercent = 0f;
        this.loaderBar.localScale = this.initScale;
        this.loaderBar.localPosition = this.initPos;
        this.currValue = 0f;

        this.startValue = startValue;
        this.targetValue = targetValue;
        this.inverseLoad = inverse;
        this.changeColor = color;

        this.material.color = Color.blue;

        if (inverseLoad) this.loadedPercent = 1f - this.loadedPercent;
        UpdateScale();
    }

    public void Step(float step)
    {
        this.loadedPercent = (this.startValue + this.currValue) / this.targetValue;
        if (this.loadedPercent > 1f) return;
        if (inverseLoad) this.loadedPercent = 1f - this.loadedPercent;
        if (changeColor) UpdateColor(this.loadedPercent);
        UpdateScale();
        this.currValue += step;
    }

    private void UpdateScale()
    {
        Vector3 scale = this.loaderBar.localScale;
        scale.x = this.loadedPercent * this.initScale.x;
        this.loaderBar.localScale = scale;

        Vector3 pos = this.loaderBar.localPosition;
        pos.x = this.initPos.x - (this.initScale.x - scale.x)/2f;
        this.loaderBar.localPosition = pos;
    }

    private void UpdateColor(float percent)
    {
        if (percent > 0.75f)
        {
            this.material.color = Color.green;
        }
        else if (percent > 0.5f)
        {
            this.material.color = Color.yellow;
        }
        else 
        {
            this.material.color = Color.red;
        }
    }

    public bool isCompleted()
    {
        // if (inverseLoad) return this.loadedPercent <= 0.05f;
        return this.loadedPercent > 1f;
    }

    public void UpdateTargetValue(float value)
    {
        this.targetValue = value;
    }
}
