using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleToneMapping : MonoBehaviour {

    public UnityStandardAssets.ImageEffects.Tonemapping tonemapping;

    public void ToggleTonemapping(UnityEngine.UI.Toggle toggle)
    {
        tonemapping.enabled = toggle.isOn;
    }
}
