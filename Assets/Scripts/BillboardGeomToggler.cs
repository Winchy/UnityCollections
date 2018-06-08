using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BillboardGeomToggler : MonoBehaviour {

    public GameObject terrain;

    public GameObject weather;

    public GameObject snow;

    public GameObject rain;

    public void OnTerrainToggle(UnityEngine.UI.Toggle toggle)
    {
        weather.SetActive(!toggle.isOn);
        terrain.SetActive(toggle.isOn);
    }

    public void OnSnowToggle(UnityEngine.UI.Toggle toggle)
    {
        weather.SetActive(toggle.isOn);
        terrain.SetActive(!toggle.isOn);
        snow.SetActive(toggle.isOn);
        rain.SetActive(!toggle.isOn);
    }

    public void OnRainToggle(UnityEngine.UI.Toggle toggle)
    {
        weather.SetActive(toggle.isOn);
        terrain.SetActive(!toggle.isOn);
        snow.SetActive(!toggle.isOn);
        rain.SetActive(toggle.isOn);
    }
}
