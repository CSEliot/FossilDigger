using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharCon : MonoBehaviour {

    public Sprite MyLook;
    public Image Img;
    public int Energy; //Health
    public int Exp;
    public int DepthInYears;
    //public int DepthInFeet;
    public int Score;
    public int MoveSpeed;
    public int Level;
    public Vector2 TilePos; //From [1 - 6], [0 - inf)  
    private Vector2 CanvasPos; // Position in Canvas Space

    public GameObject StartingTile;

    private TileMan _TileMan; // "_SingleTon" Notation

    // Use this for initialization
    void Start ()
    {
        _TileMan = GameObject.FindGameObjectWithTag("TileMan").GetComponent<TileMan>();
        //transform.SetParent(StartingTile.transform);
        //StartCoroutine(DelayAction());
        //GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 00f);
        TilePos = StartingTile.GetComponent<Tile>().Pos;
        CanvasPos = GetComponent<RectTransform>().anchoredPosition;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static Vector2 GetPos()
    {
        return GameObject.FindGameObjectWithTag("Player").GetComponent<CharCon>().TilePos;
    }


    public static void Move(TileMan.Direction TargetDirection)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharCon>()._Move(TargetDirection);
    }

    public void _Move(TileMan.Direction TargetDirection)
    {
        switch (TargetDirection)
        {
            case TileMan.Direction.North:
                //if (TilePos.y == 1)
                //    return false;
                //Change Rotation
                transform.localRotation = Quaternion.Euler(0f, 0f, 360f);
                //Move
                TilePos.Set(TilePos.x, TilePos.y - 1);
                CanvasPos.Set(CanvasPos.x, CanvasPos.y + 100f);
                GetComponent<RectTransform>().anchoredPosition = CanvasPos;
                break;
            case TileMan.Direction.East:
                //if (TilePos.x == _TileMan.TotalCol - 1)
                //    return false;
                //Change Rotation
                transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
                //Move
                TilePos.Set(TilePos.x + 1, TilePos.y);
                CanvasPos.Set(CanvasPos.x + 100f, CanvasPos.y);
                GetComponent<RectTransform>().anchoredPosition = CanvasPos;
                break;
            case TileMan.Direction.South:
                //No pos check, south can go down infinitely

                //Change Rotation
                transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                //Move
                TilePos.Set(TilePos.x, TilePos.y + 1);
                CanvasPos.Set(CanvasPos.x, CanvasPos.y - 100f);
                GetComponent<RectTransform>().anchoredPosition = CanvasPos;
                break;
            case TileMan.Direction.West:
                //if (TilePos.x == 1)
                //    return false;
                //Change Rotation
                transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                //Move
                TilePos.Set(TilePos.x - 1, TilePos.y);
                CanvasPos.Set(CanvasPos.x - 100f, CanvasPos.y);
                GetComponent<RectTransform>().anchoredPosition = CanvasPos;
                break;
            default:
                break;
        }
    }

    public IEnumerator DelayAction()
    {
        yield return null;
        transform.SetSiblingIndex(1);
    }
}
