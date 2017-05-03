using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tile : MonoBehaviour {

    public Vector2 Pos;
    public Button Button;

	// Use this for initialization
	void Start () {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(() => this.Move());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move()
    {
        GameObject.FindGameObjectWithTag("TileMan").GetComponent<BoardMan>().Move(Pos);
    }

}
