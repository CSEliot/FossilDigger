using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles all tile manipulation, this includes movement.
/// Top Left is [0,0], downwards and right are positive directions.
/// </summary>
public class TileMan : MonoBehaviour {

    public Sprite[] Earth;
    public Sprite[] Background;

    public GameObject ItemPrefab;
    public CharCon Player;

    public int TotalRows;
    public int TotalCol;

    public int Seed;
    public bool UseSeed;

    public List<Item.Type[]> TileMemoryList;

    public float XPosStartWorldSpace;
    public float YPosStartWorldSpace;

    public GameObject TopRow; //For enabling top row buttons later on.

    private Vector2 boardPos; //Position on absolute grid. From [0 - 7], [0 - 7]

    [System.Serializable]
    public enum Direction
    {
        North,
        East,
        South,
        West,
        None
    }

	// Use this for initialization
	void Start () {
        //Debug.Log("Welp!");
        Arrange();
        //Debug.Log("YES");
        if (UseSeed)
            Random.InitState(Seed);

        TileMemoryList = new List<Item.Type[]>();

        boardPos = CharCon.GetPos();
    }
	
	// Update is called once per frame
	void Update () {
        //Arrange();
	}

    /// <summary>
    /// Arrange tile objects properly.
    /// </summary>
    private void Arrange()
    {
        float xPosRow = 112f; //Relative per row to parent
        float yPosRow = -50f; //Relative per row to parent
        float xPosCol = 0f + XPosStartWorldSpace; //World space
        float yPosCol = 0f + YPosStartWorldSpace; // World space
        for(int y = 0; y < TotalRows; y++)
        {
            transform.GetChild(y).GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosCol, yPosCol);
            for (int x = 0; x < TotalCol; x++)
            {
                transform.GetChild(y).GetChild(x).GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosRow, yPosRow);
                transform.GetChild(y).GetChild(x).GetComponent<Tile>().Pos = new Vector2(x, y);
                if(x > 0 && x < TotalCol - 1 && y > 0)
                {
                    GameObject itemTemp = Instantiate(ItemPrefab, transform.GetChild(y).GetChild(x), false) as GameObject;
                    itemTemp.GetComponent<Item>().SetType((Item.Type)Random.Range(0, 2));
                }
                //CBUG.Do("Moving: X: " + xPosRow + " Y: " + yPosRow);
                //CBUG.Do("It is now: " +
                //transform.GetChild(y).GetChild(x).GetComponent<RectTransform>().anchoredPosition.x +
                //", " +
                //transform.GetChild(y).GetChild(x).GetComponent<RectTransform>().anchoredPosition.y
                //);
                xPosRow += 100f;
            }
            xPosRow = 112f;
            yPosCol -= 100f;
        }
    }

    /// <summary>
    /// Adjusts tiles and calls Character Move CharCon.Move script.
    /// </summary>
    /// <param name="TargetPos"></param>
    public void Move(Vector2 TargetPos)
    {
        Direction targetDirection = GetDirection(TargetPos);
        switch (targetDirection)
        {
            case Direction.North:
                if (CharCon.GetPos().y == 1)
                    return;
                    boardPos += new Vector2(0, -1);
                if (boardPos.y == 1)
                    adjustTilesUp();
                break;
            case Direction.East:
                if (CharCon.GetPos().x == TotalCol - 1)
                    return;
                break;
            case Direction.South:
                if (CharCon.GetPos().y == TotalRows - 1)
                    adjustTilesDown();
                break;
            case Direction.West:
                if (CharCon.GetPos().x == 1)
                    return;
                break;
            default:
                break;
        }
        CharCon.Move(targetDirection);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void PlaceItem(int x, int y)
    {

    }

    private void adjustTilesUp()
    {
        //Set y row buttons to uninteractable
    }

    private void adjustTilesDown()
    {
        //Set y row buttons to interactable
    }

    public Direction GetDirection(Vector2 NewPos)
    {
        Vector2 oldPos = CharCon.GetPos();
        if (NewPos.x > oldPos.x)
            return Direction.East;
        else if (NewPos.x < oldPos.x)
            return Direction.West;
        else if (NewPos.y > oldPos.y)
            return Direction.South;
        else if (NewPos.y < oldPos.y)
            return Direction.North;
        else
            return Direction.None;
    }
    
}
