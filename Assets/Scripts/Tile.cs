using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Tile : MonoBehaviour {

    public Vector2 Pos;
    public Button Button;

    public Image MyBG;

	// Use this for initialization
	void Start () {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(() => this.Move());
        
        MyBG = GetComponent<Image>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Move()
    {
        GameObject.FindGameObjectWithTag("BoardMan").GetComponent<BoardMan>().Move(Pos);
    }

}
