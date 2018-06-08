using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlanetMaterialSetting : MonoBehaviour {

    Material planetMaterial;
    
	// Use this for initialization
	void Start () {
        planetMaterial = GetComponent<Renderer>().material;
	}
	
	public void SetHeightScale(Slider slider)
    {
        planetMaterial.SetFloat("_HeightScale", slider.value);
    }

    public void SetWaterHeight(Slider slider)
    {
        planetMaterial.SetFloat("_WaterHeight", slider.value);
    }
}
