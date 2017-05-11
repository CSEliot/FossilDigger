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
        None,
        Fossil,
        Energy,
        Damage,
        EBoost,
        HBoost
    };

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetType(Item.Type ItemType)
    {
        if(ItemType == Type.None)
        {
            Img.enabled = false;
        }
        else
        {
            Img.enabled = true;
            Img.sprite = GFXs[((int)ItemType) - 1];
            Damage = DamagePerType[((int)ItemType) - 1];
            ExpYield = ExpYieldPerType[(int)ItemType - 1];
            this.ItemType = ItemType;
        }
    }
}
