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

    private BoardMan _BoardMan;

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
        _BoardMan = BoardMan.GetSelf();
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
            }
        }
    }
}
