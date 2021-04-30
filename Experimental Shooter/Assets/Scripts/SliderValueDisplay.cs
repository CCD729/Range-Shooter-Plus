using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValueDisplay : MonoBehaviour
{
    [SerializeField]
    private Slider parentSlider;
    // Start is called before the first frame update
    void Start()
    {
        parentSlider = transform.parent.GetComponent<Slider>();
    }
    public void SetValueText(float value)
    {
        GetComponent<Text>().text = value.ToString("f0");
    }
}
