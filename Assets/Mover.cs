using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Mover : MonoBehaviour {

    private GameObject guide;
    private GameObject block;
    private GameObject ui;
    private GameObject canvas;

    public Slider speedSlider;
    public Slider lengthSlider;

    private int dir;

    public float speed = 10.0f;
    public float length = 10.0f;

    //private int lastSpeed = 10;
    //private int lastLength = 10;

	// Use this for initialization
	void Start () {
        guide = transform.Find("Guide").gameObject;
        block = transform.Find("Block").gameObject;
        ui = transform.Find("UI").gameObject;
        canvas = GameObject.Find("Canvas");

        

        guide.transform.localScale = new Vector3(length * 0.1f, 0.1f, 0.01f);
        dir = -1;

        speedSlider.onValueChanged.AddListener(delegate
        {
            SpeedValueChanged(speedSlider);
        });
        lengthSlider.onValueChanged.AddListener(delegate
        {
            RangeValueChanged(lengthSlider);
        });
	}

    private void SpeedValueChanged(Slider target)
    {
        speed = speedSlider.value;
    }

    private void RangeValueChanged(Slider target)
    {
        length = lengthSlider.value;
        guide.transform.localScale = new Vector3(length * 0.1f, 0.1f, 0.01f);
    }
	
	// Update is called once per frame
	void Update () {

        /*if (lastLength != length)
        {
            guide.transform.localScale = new Vector3(length * 0.1f, 0.1f, 0.01f);
            lastLength = length;
        }*/

        float rightSide = guide.transform.localPosition.x + Mathf.Abs(guide.transform.localScale.x / 2.0f);
        float leftSide = guide.transform.localPosition.x - Mathf.Abs(guide.transform.localScale.x / 2.0f);
        //Debug.Log("x is " + block.transform.localPosition.x);
        //Debug.Log("right is " + rightSide);
        //Debug.Log("left is " + leftSide);
        if (block.transform.localPosition.x >= rightSide)
        {
            dir = -1;
        }
        else if (block.transform.localPosition.x <= leftSide)
        {
            dir = 1;
        }
        block.transform.Translate(new Vector3(dir*speed*0.005f, 0.0f, 0.0f));
	}

    public void showUI()
    {
        ui.transform.parent = canvas.transform;
        ui.transform.position = new Vector3(0, 0, 0);
        ui.transform.rotation = new Quaternion(0, 0, 0, 0);
        ui.transform.localScale = new Vector3(1, 1, 1);
    }
    public void hideUI()
    {
        ui.transform.parent = transform;
    }
}
