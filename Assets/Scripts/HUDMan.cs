using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDMan : MonoBehaviour {

    public GameObject HealthBar;
    public GameObject EnergyBar;
    public GameObject DepthUI;
    public GameObject AbsoluteAgeUI;

    public Image HealthBarFill;
    private Text healthBarText;
    public Image EnergyBarFill;
    private Text energyBarText;
    private Text depthText;
    private Text absoluteAgeText;

	// Use this for initialization
	void Awake () {
        healthBarText = HealthBar.GetComponentInChildren<Text>();
        energyBarText = EnergyBar.GetComponentInChildren<Text>();
        depthText = DepthUI.GetComponentInChildren<Text>();
        absoluteAgeText = AbsoluteAgeUI.GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    #region Static Call Hooks
    public static void SetHealth(int Numerator, int Denominator)
    {
        GameObject.FindGameObjectWithTag("HUDMan").GetComponent<HUDMan>()._setHealth(Numerator, Denominator);
    }
    public static void SetEnergy(int Numerator, int Denominator)
    {
        GameObject.FindGameObjectWithTag("HUDMan").GetComponent<HUDMan>()._setEnergy(Numerator, Denominator);
    }
    public static void SetDepth(float Depth)
    {
        GameObject.FindGameObjectWithTag("HUDMan").GetComponent<HUDMan>()._setDepth(Depth);
    }
    public static void SetAbsoluteAge(float Age)
    {
        GameObject.FindGameObjectWithTag("HUDMan").GetComponent<HUDMan>()._setAbsoluteAge(Age);
    }
    #endregion

    #region Static Call Helpers
    private void _setHealth(int numerator, int denominator)
    {
        HealthBarFill.fillAmount = ((float)numerator) / ((float)denominator);
        healthBarText.text = numerator.ToString() + " / " + denominator.ToString();
    }
    private void _setEnergy(int numerator, int denominator)
    {
        EnergyBarFill.fillAmount = ((float)numerator) / ((float)denominator);
        energyBarText.text = numerator.ToString() + " / " + denominator.ToString();
    }
    private void _setDepth(float depth)
    {
        depthText.text = depth.ToString() + " Feet Deep";
    }
    private void _setAbsoluteAge(float age)
    {
        //absoluteAgeText.text = age.ToString() + " Years Ago";
    }
    //TODO FINISH THIS
    #endregion
}
