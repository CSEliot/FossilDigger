using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles all tile manipulation, this includes movement.
/// Top Left is [0,0], downwards and right are positive directions.
/// </summary>
public class BoardMan : MonoBehaviour {


    private bool gamePaused;

    public CharCon Player;
    public GameManager Game;
    
    public int Seed;
    public bool UseSeed;

    #region Tile Organization - Active
    /// <summary>
    /// from [0 - 6], [0 - inf) storing what item spawned and where, generated procedurally.
    /// </summary>
    public List<Item.Type[]> ItemBoardTypeHistory;
    /// <summary>
    /// All the panel objects on the board from [0 - 6], [0 - 6]
    /// </summary>
    public List<GameObject[]> ItemBoardObjs;
    public GameObject TopRow; //For enabling top row buttons later on.
    /// <summary>
    /// Board Y Position where moving past stops moving and instead moves board Up.
    /// </summary>
    public int AdjustUpThreshold;
    /// <summary>
    /// Board Y Position where moving past stops moving and instead moves board Down.
    /// </summary>
    public int AdjustDownThreshold;
    #endregion

    #region Tile Organization - Passive
    public float XPosStartWorldSpace;
    public float YPosStartWorldSpace;
    public int TotalRows;
    public int TotalCol;
    private int totalPlayableCol;
    public GameObject ItemPrefab;
    public Sprite[] Earth;
    public Sprite[] Background;
    #endregion

    #region Position Tracking
    /// <summary>
    /// Position in world. From [1 - 7], [1 - inf)  
    /// </summary>
    public Vector2 TileWorldPos;
    /// <summary>
    /// Position on absolute grid. From [0 - 7], [0 - 7]
    /// </summary>
    private Vector2 boardLocalPos;
    public GameObject StartingTile;
    #endregion

    [System.Serializable]
    public enum Direction
    {
        North,
        East,
        South,
        West,
        None
    }
    //TODO: Make the top row just a none type of items instead of nothing.

	// Use this for initialization
	void Start () {
        //Debug.Log("Welp!");
        ItemBoardTypeHistory = new List<Item.Type[]>();
        ItemBoardObjs = new List<GameObject[]>();
        Arrange();
        //Debug.Log("YES");
        if (UseSeed)
            Random.InitState(Seed);

        TileWorldPos = StartingTile.GetComponent<Tile>().Pos;
        boardLocalPos = TileWorldPos;
        totalPlayableCol = TotalCol - 2;
        gamePaused = false;
    }
	
	// Update is called once per frame
	void Update () {
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
            //Background Tiles
            transform.GetChild(y).GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosCol, yPosCol);

            //Items
            Item.Type[] tempArrayA = new Item.Type[7];
            ItemBoardTypeHistory.Add(tempArrayA);
            GameObject[] tempArrayB = new GameObject[7];
            ItemBoardObjs.Add(tempArrayB);

            for (int x = 0; x < TotalCol; x++)
            {
                //Background tiles.
                transform.GetChild(y).GetChild(x).GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosRow, yPosRow);
                transform.GetChild(y).GetChild(x).GetComponent<Tile>().Pos = new Vector2(x, y);
                xPosRow += 100f;
                
                //Items
                if(x > 0 && x < TotalCol - 1)
                {
                    GameObject itemTemp = Instantiate(ItemPrefab, transform.GetChild(y).GetChild(x), false) as GameObject;
                    Item.Type tempItemType;
                    if (y == 0)
                    {
                        tempItemType = Item.Type.None;
                    }
                    else
                    {
                        Random.InitState(x * y);
                        tempItemType = SpawnMan.SpawnOnChance(Random.Range(0f, 100f));
                    }
                    itemTemp.GetComponent<Item>().SetType(tempItemType);
                    ItemBoardTypeHistory[y][x - 1] = tempItemType;
                    ItemBoardObjs[y][x - 1] = itemTemp;
                }
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

        if (gamePaused)
            return;

        Direction targetDirection = GetDirection(TargetPos);
        Item.Type targetItem; 
        switch (targetDirection)
        {
            case Direction.North:
                //Top-most row, can't go further.
                if (TileWorldPos.y == 1)
                    return;
                //Only adjust tiles up
                TileWorldPos += new Vector2(0, -1);
                targetItem = ItemBoardObjs[(int)boardLocalPos.y][(int)boardLocalPos.x].GetComponent<Item>().ItemType;//ItemBoardTypeHistory[(int)TileWorldPos.y][(int)TileWorldPos.x - 1];
                if (boardLocalPos.y <= AdjustDownThreshold && !(TileWorldPos.y < AdjustDownThreshold))
                {
                    //Only adjust if we're not on the edge.
                    adjustTilesDown();
                    Game.Update(targetDirection, targetItem, true);
                }
                else
                {
                    boardLocalPos += new Vector2(0, -1);
                    Game.Update(targetDirection, targetItem, false);
                }
                break;
            case Direction.South:
                //no board checks necessary for south, we can go south forever
                TileWorldPos += new Vector2(0, 1);
                targetItem = ItemBoardTypeHistory[(int)TileWorldPos.y][(int)TileWorldPos.x - 1];
                if (boardLocalPos.y >= AdjustUpThreshold)
                {
                    adjustTilesUp();
                    Game.Update(targetDirection, targetItem, true);
                }
                else
                {
                    boardLocalPos += new Vector2(0, 1);
                    Game.Update(targetDirection, targetItem, false);
                }
                break;
            case Direction.East:
                TileWorldPos += new Vector2(1, 0);
                boardLocalPos += new Vector2(1, 0);
                targetItem = ItemBoardTypeHistory[(int)TileWorldPos.y][(int)TileWorldPos.x - 1];
                Game.Update(targetDirection, targetItem);
                break;
            case Direction.West:
                TileWorldPos += new Vector2(-1, 0);
                boardLocalPos += new Vector2(-1, 0);
                targetItem = ItemBoardTypeHistory[(int)TileWorldPos.y][(int)TileWorldPos.x - 1];
                Game.Update(targetDirection, targetItem);
                break;
            case Direction.None:
                CBUG.Do("Player clicked on ship's location!");
                break;
            default:
                CBUG.Error("Bad Direction given!" + targetDirection.ToString());
                break;
        }
    }

    /// <summary>
    /// Generate direction based on old and target board location.
    /// </summary>
    /// <param name="NewPos">Target position from board given by clicked tile.</param>
    /// <returns> Direction enum in N, E, W, or S. </returns>
    public Direction GetDirection(Vector2 NewPos)
    {
        Vector2 oldPos = boardLocalPos;
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void PlaceItem(int x, int y)
    {

    }

    /// <summary>
    /// Tiles moving up means player moving down.
    /// </summary>
    private void adjustTilesUp()
    {
        //Digging down into a new row
        // Math: If the Current World Pos + distance from nongenerated bottom edge is more than
        //       the currently generated list of rows, then make more rows for us to traverse on.
        int newRow = (int)TileWorldPos.y + ((TotalRows - 1) - AdjustUpThreshold); //New Row standardized
        if (newRow >= (ItemBoardTypeHistory.Count - 1))
        {
            //make new row
            buildNewRow();
        }
        for(int y = 0; y < TotalRows; y++)
        {
            for(int col = 0; col < totalPlayableCol; col++)
            {
                Random.InitState((y + (int)TileWorldPos.y - AdjustUpThreshold) * col);
                Item.Type tempItemType = SpawnMan.SpawnOnChance(Random.Range(0f, 100f));
                ItemBoardObjs[y][col].GetComponent<Item>().SetType(tempItemType);
                //ItemBoardObjs[y][col].GetComponent<Item>().SetType(ItemBoardTypeHistory[y + (int)TileWorldPos.y - AdjustUpThreshold][col]);
            }
        }
        if(TileWorldPos.y >= AdjustDownThreshold)
        {
            // Top edge being itemless ends whenever we move down, adjusting tiles up, pushing items into itemless area.
            for (int x = 0; x < totalPlayableCol; x++)
            {
                TopRow.transform.GetChild(x + 1).GetComponent<Button>().interactable = true;
            }
        }
    }

    /// <summary>
    /// Tiles moving down means player moving up.
    /// </summary>
    private void adjustTilesDown()
    {
        for (int y = 0; y < TotalRows; y++)
        {
            for (int col = 0; col < totalPlayableCol; col++)
            {
                {
                    Random.InitState((y + ((int)TileWorldPos.y - (AdjustDownThreshold))) * col);
                    Item.Type tempItemType = SpawnMan.SpawnOnChance(Random.Range(0f, 100f));
                    ItemBoardObjs[y][col].GetComponent<Item>().SetType(tempItemType);//ItemBoardTypeHistory[y + ((int)TileWorldPos.y - (AdjustDownThreshold))][col]);
                }
            }
        }
        //Set y row buttons to uninteractable if we reach the top level.
        if (TileWorldPos.y <= AdjustDownThreshold)
        {
            for (int x = 0; x < totalPlayableCol; x++)
            {
                TopRow.transform.GetChild(x + 1).GetComponent<Button>().interactable = false;
            }
        }
    }

    private void buildNewRow()
    {
        Item.Type[] tempArray = new Item.Type[totalPlayableCol];
        ItemBoardTypeHistory.Add(tempArray);
        int newRow = (int)TileWorldPos.y + ((TotalRows - 1) - AdjustUpThreshold);
        for (int col = 0; col < totalPlayableCol; col++)
        {
            Random.InitState(newRow * col);
            Item.Type tempItemType = SpawnMan.SpawnOnChance(Random.Range(0f, 100f));
            ItemBoardTypeHistory[newRow][col] = tempItemType;
        }
    }

    public bool GamePaused {
        //get {
        //    return gamePaused;
        //}

        set {
            gamePaused = value;
        }
    }
}
