using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour {

    public Sprite[] GFXs;
    private Type itemType;
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

    public Type ItemType {
        get {
            Type tempType = itemType;
            ItemType = Type.None;
            return tempType;
        }

        set {
            itemType = value;
            if (value == Type.None)
            {
                Img.enabled = false;
            }
            else
            {
                Img.enabled = true;
                Img.sprite = GFXs[((int)itemType) - 1];
                //Damage = DamagePerType[((int)itemType) - 1];
                //ExpYield = ExpYieldPerType[(int)itemType - 1];
            }
        }
    }
}
