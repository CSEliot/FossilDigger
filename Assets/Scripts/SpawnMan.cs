using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMan : MonoBehaviour {

    [Range(0, 100f)]
    public float Fossil;

    [Range(0, 100f)]
    public float Energy;

    [Range(0, 100f)]
    public float Damage;


    // Use this for initialization
    void Start () {
        gameObject.tag = "SpawnMan";
        //if (Fossil + Damage + Energy > 100f)
        //    CBUG.SrsError("Spawn chances total greater than 100!");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Item.Type SpawnOnChance(float RNGNumber)
    {
        return GameObject.FindGameObjectWithTag("SpawnMan").GetComponent<SpawnMan>()._spawnOnChance(RNGNumber);
        //switch (ItemType)
        //{
        //    case Item.Type.Fossil:
        //        if(RNGNumber )
        //        break;
        //    default:
        //        CBUG.SrsError("Bad ItemType given!");
        //        break;
        //}
    }

    private Item.Type _spawnOnChance(float rngNumber)
    {
        if (rngNumber < Fossil)
            return Item.Type.Fossil;
        else if (rngNumber < Energy)
            return Item.Type.Energy;
        else
            return Item.Type.Damage;
    }
}
