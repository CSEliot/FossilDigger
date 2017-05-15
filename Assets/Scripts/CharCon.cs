using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Character Controller for handing all actionable player functions.
/// Controllers are for "doing" and thus do not store any gameplay data.
/// </summary>
public class CharCon : MonoBehaviour {

    public Sprite MyLook;
    public Image Img;
    private Vector2 CanvasPos; // Position in Canvas Space

    //private BoardMan _BoardMan; // "_SingleTon" Notation

    // Use this for initialization
    void Start ()
    {
        //_BoardMan = GameObject.FindGameObjectWithTag("TileMan").GetComponent<BoardMan>();
        //transform.SetParent(StartingTile.transform);
        //StartCoroutine(DelayAction());
        //GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 00f);

        CanvasPos = GetComponent<RectTransform>().anchoredPosition;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public static void Move(BoardMan.Direction TargetDirection)
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<CharCon>()._Move(TargetDirection);
    }

    public void _Move(BoardMan.Direction TargetDirection)
    {
        switch (TargetDirection)
        {
            case BoardMan.Direction.North:
                //Change Rotation
                transform.localRotation = Quaternion.Euler(0f, 0f, 360f);
                //Move
                CanvasPos.Set(CanvasPos.x, CanvasPos.y + 100f);
                GetComponent<RectTransform>().anchoredPosition = CanvasPos;
                break;
            case BoardMan.Direction.East:
                //if (TilePos.x == _TileMan.TotalCol - 1)
                //    return false;
                //Change Rotation
                transform.localRotation = Quaternion.Euler(0f, 0f, 270f);
                //Move
                CanvasPos.Set(CanvasPos.x + 100f, CanvasPos.y);
                GetComponent<RectTransform>().anchoredPosition = CanvasPos;
                break;
            case BoardMan.Direction.South:
                //Change Rotation
                transform.localRotation = Quaternion.Euler(0f, 0f, 180f);
                //Move
                CanvasPos.Set(CanvasPos.x, CanvasPos.y - 100f);
                GetComponent<RectTransform>().anchoredPosition = CanvasPos;
                break;
            case BoardMan.Direction.West:
                //if (TilePos.x == 1)
                //    return false;
                //Change Rotation
                transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
                //Move
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
