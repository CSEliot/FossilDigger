using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

    public Sprite[] GFXs;
    public Type ItemType;
    public int[] ExpYieldPerType;
    public int[] DamagePerType;
    public Image Img;
    public int ExpYield;
    public int Damage;

    public enum Type
    {
        Fossil,
        Energy,
        Damage
    };

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetType(Item.Type ItemType)
    {
        Img.sprite = GFXs[(int)ItemType];
        Damage = DamagePerType[(int)ItemType];
        ExpYield = ExpYieldPerType[(int)ItemType];
        this.ItemType = ItemType;
    }
}
