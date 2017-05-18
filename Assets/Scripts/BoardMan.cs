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
    private bool itemsUpdatePaused;

    public CharCon Player;
    public GameManager Game;
    
    public int Seed;
    public bool UseSeed;

    #region Tile Organization - Active
    /// <summary>
    /// from [0 - 6], [0 - inf) storing what item spawned and where, generated procedurally.
    /// </summary>
    //public List<Item.Type[]> ItemBoardTypeHistory;
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
    private List<List<int>> usedItemHistory;
    #endregion

    #region Tile Organization - Passive
    public float XPosStartWorldSpace;
    public float YPosStartWorldSpace;
    public int TotalRows;
    private int deepestRowGenerated;
    public int TotalCol;
    private int totalPlayableCol;
    public GameObject ItemPrefab;
    public Sprite[] Earth;
    public Sprite[] Background;
    #endregion

    #region Position Tracking
    /// <summary>
    /// Position in world. From [0 - 6], [0 - inf). 
    /// </summary>
    public Vector2 TileWorldPos;
    /// <summary>
    /// Position on absolute grid. From [0 - 6], [0 - 6]
    /// </summary>
    private Vector2 boardLocalPos;
    public GameObject StartingTile;
    #endregion

    //TODO: TELEPORT FUNCTIONALITY FOR LOSING ALL HEALTH OR ENERGY OR GETTING Q CORRECT.

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
        //ItemBoardTypeHistory = new List<Item.Type[]>();
        ItemBoardObjs = new List<GameObject[]>();
        usedItemHistory = new List<List<int>>();
        deepestRowGenerated = 0;
        Arrange();
        //Debug.Log("YES");
        if (UseSeed)
            Random.InitState(Seed);

        TileWorldPos = StartingTile.GetComponent<Tile>().Pos + new Vector2(-1, 0);
        boardLocalPos = TileWorldPos;
        totalPlayableCol = TotalCol - 2;
        gamePaused = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("k"))
        {
            TeleportDistance(10);
        }
        if (Input.GetKeyDown("l"))
        {
            TeleportDistance(3);
        }
        if (Input.GetKeyDown("i"))
        {
            TeleportDistance(-10);
        }
        if (Input.GetKeyDown("o"))
        {
            TeleportDistance(-3);
        }
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
            //Item.Type[] tempArrayA = new Item.Type[7];
            //ItemBoardTypeHistory.Add(tempArrayA);
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
                    itemTemp.GetComponent<Item>().ItemType = tempItemType;
                    //ItemBoardTypeHistory[y][x - 1] = tempItemType;
                    ItemBoardObjs[y][x - 1] = itemTemp;
                }
            }
            xPosRow = 112f;
            yPosCol -= 100f;
            deepestRowGenerated++;
            usedItemHistory.Add(new List<int>());
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
                targetItem = ItemBoardObjs[(int)boardLocalPos.y - 1][(int)boardLocalPos.x].GetComponent<Item>().ItemType;//ItemBoardTypeHistory[(int)TileWorldPos.y][(int)TileWorldPos.x - 1];
                if (boardLocalPos.y <= AdjustDownThreshold && !(TileWorldPos.y < AdjustDownThreshold))
                {
                    //Only adjust if we're not on the edge.
                    updateTiles();
                    if (itemsUpdatePaused)
                    {
                        Game.Update(targetDirection, Item.Type.None, true);
                    }
                    else
                    {
                        updateItems();
                        Game.Update(targetDirection, targetItem, true);
                    }
                }
                else
                {
                    boardLocalPos += new Vector2(0, -1);
                    Game.Update(targetDirection, targetItem, false);
                    usedItemHistory[(int)TileWorldPos.y].Add((int)TileWorldPos.x);
                }
                break;
            case Direction.South:
                //no board checks necessary for south, we can go south forever
                TileWorldPos += new Vector2(0, 1);
                targetItem = ItemBoardObjs[(int)boardLocalPos.y + 1][(int)boardLocalPos.x].GetComponent<Item>().ItemType;//ItemBoardTypeHistory[(int)TileWorldPos.y][(int)TileWorldPos.x - 1];
                if (boardLocalPos.y >= AdjustUpThreshold)
                {
                    updateTiles();
                    if (itemsUpdatePaused)
                    {
                        Game.Update(targetDirection, Item.Type.None, true);
                    }
                    else
                    {
                        updateItems();
                        Game.Update(targetDirection, targetItem, true);
                    }
                }
                else
                {
                    boardLocalPos += new Vector2(0, 1);
                    Game.Update(targetDirection, targetItem, false);
                    usedItemHistory[(int)TileWorldPos.y].Add((int)TileWorldPos.x);
                }
                break;
            case Direction.East:
                TileWorldPos += new Vector2(1, 0);
                boardLocalPos += new Vector2(1, 0);
                targetItem = ItemBoardObjs[(int)boardLocalPos.y][(int)boardLocalPos.x].GetComponent<Item>().ItemType;//ItemBoardTypeHistory[(int)TileWorldPos.y][(int)TileWorldPos.x - 1];
                Game.Update(targetDirection, targetItem);
                usedItemHistory[(int)TileWorldPos.y].Add((int)TileWorldPos.x);
                break;
            case Direction.West:
                TileWorldPos += new Vector2(-1, 0);
                boardLocalPos += new Vector2(-1, 0);
                targetItem = ItemBoardObjs[(int)boardLocalPos.y][(int)boardLocalPos.x].GetComponent<Item>().ItemType;//ItemBoardTypeHistory[(int)TileWorldPos.y][(int)TileWorldPos.x - 1];
                Game.Update(targetDirection, targetItem);
                usedItemHistory[(int)TileWorldPos.y].Add((int)TileWorldPos.x);
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
        NewPos += new Vector2(-1, 0); //Tiles overcorrect for background space.
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
    /// Tiles moving down means player moving up.
    /// </summary>
    private void updateTiles()
    {
        int newRow = (int)TileWorldPos.y + ((TotalRows - 1) - AdjustUpThreshold); //New Row standardized
        if (newRow >= deepestRowGenerated)
        {
            deepestRowGenerated++;
            usedItemHistory.Add(new List<int>());
        }
        usedItemHistory[(int)TileWorldPos.y].Add((int)TileWorldPos.x);
        //Set y row buttons to uninteractable if we reach the top level.
        if (TileWorldPos.y <= AdjustDownThreshold)
        {
            for (int x = 0; x < totalPlayableCol; x++)
            {
                TopRow.transform.GetChild(x + 1).GetComponent<Button>().interactable = false;
                //ItemBoardObjs[0][x].GetComponent<Item>().ItemType = Item.Type.None;
            }
        }
        else
        {
            // Top edge being itemless ends whenever we move down, adjusting tiles up, pushing items into itemless area.
            for (int x = 0; x < totalPlayableCol; x++)
            {
                TopRow.transform.GetChild(x + 1).GetComponent<Button>().interactable = true;
            }
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

    /// <summary>
    /// Move Up/Down/Left/Right relative to current position.
    /// </summary>
    /// <param name="worldY">Tile World Position</param>
    /// <param name="worldX">Tile World Position</param>
    public void TeleportDistance(int worldY, int worldX = 0)
    {
        itemsUpdatePaused = true;
        if (worldX != 0)
        {
            Vector2 horizTarget = new Vector2(worldX > 0 ? 99f : -99f, boardLocalPos.y);
            for (; worldX > 0; worldX--)
                Move(horizTarget);
            
        }
        if (worldY != 0)
        {
            Vector2 vertTarget = new Vector2(boardLocalPos.x + 1, worldY > 0 ? 99f : -99f);
            for (; worldY > 0; worldY--)
                Move(vertTarget);
        }
        itemsUpdatePaused = false;
        updateItems();

        ////Edge Case Not Handled: X + Pos > or < Bounds
        ////Edge Case: Not TPing past 0
        //if (worldY + TileWorldPos.y < 0)
        //    worldY = (int)TileWorldPos.y * -1;

        //int signY = worldY < 0 ? -1 : 1;
        //int signX = worldX < 0 ? -1 : 1;

        //if (worldX != 0)
        //{
        //    for (int x = 0; x < worldX; x++)
        //    {
        //        TileWorldPos += new Vector2(0, 1 * signX);
        //        Game.Update(worldX > 0 ? Direction.East : Direction.West, Item.Type.None, true);
        //    }
        //}

        //if (worldY != 0)
        //{
        //    for (int y = 0; y < worldY; y++)
        //    {
        //        TileWorldPos += new Vector2(0, 1 * signY);
        //        int newRow = (int)TileWorldPos.y + ((TotalRows - 1) - AdjustUpThreshold); //New Row standardized
        //        if (newRow >= deepestRowGenerated)
        //        {
        //            deepestRowGenerated++;
        //            usedItemHistory.Add(new List<int>());
        //        }
        //        Game.Update(worldY > 0 ? Direction.East : Direction.West, Item.Type.None, true);
        //    }
        //}

        //updateItems();


    }

    /// <summary>
    /// Arrange items based on player location, relative to tile's unique world position.
    /// </summary>
    private void updateItems()
    {
        for (int boardY = 0; boardY < TotalRows; boardY++)
        {
            for (int boardX = 0; boardX < totalPlayableCol; boardX++)
            {
                int worldY = boardY + (int)TileWorldPos.y - (int)boardLocalPos.y;
                int worldX = boardX;
                if (usedItemHistory[worldY].Contains(worldX))
                {
                    ItemBoardObjs[boardY][boardX].GetComponent<Item>().ItemType = Item.Type.None;
                }
                else
                {
                    Random.InitState(worldY * (worldX + 1));
                    Item.Type tempItemType = SpawnMan.SpawnOnChance(Random.Range(0f, 100f));
                    ItemBoardObjs[boardY][boardX].GetComponent<Item>().ItemType = tempItemType;//ItemBoardTypeHistory[y + ((int)TileWorldPos.y - (AdjustDownThreshold))][col]);
                }
            }
        }
        if (TileWorldPos.y <= AdjustDownThreshold - 1)
        {
            for (int x = 0; x < totalPlayableCol; x++)
            {
                ItemBoardObjs[0][x].GetComponent<Item>().ItemType = Item.Type.None;
            }
        }
    }

}

//private void buildNewRow()
//{
//    //Item.Type[] tempArray = new Item.Type[totalPlayableCol];
//    //ItemBoardTypeHistory.Add(tempArray);
//    int newRow = (int)TileWorldPos.y + ((TotalRows - 1) - AdjustUpThreshold);
//    for (int col = 0; col < totalPlayableCol; col++)
//    {
//        Random.InitState(newRow * col);
//        Item.Type tempItemType = SpawnMan.SpawnOnChance(Random.Range(0f, 100f));
//        //ItemBoardTypeHistory[newRow][col] = tempItemType;
//    }
//}

///// <summary>
///// Tiles moving up means player moving down.
///// </summary>
//private void adjustTilesUp()
//{
//    //Digging down into a new row
//    // Math: If the Current World Pos + distance from nongenerated bottom edge is more than
//    //       the currently generated list of rows, then make more rows for us to traverse on.

//    usedItemHistory[(int)TileWorldPos.y].Add((int)TileWorldPos.x);
//    for(int boardY = 0; boardY < TotalRows; boardY++)
//    {
//        for(int boardX = 0; boardX < totalPlayableCol; boardX++)
//        {
//            int worldY = boardY + (int)TileWorldPos.y - (TotalRows - 1) + 1;
//            int worldX = boardX;
//            if (usedItemHistory[worldY].Contains(worldX))
//            {
//                ItemBoardObjs[boardY][boardX].GetComponent<Item>().ItemType = Item.Type.None;
//            }
//            else
//            {
//                Random.InitState((boardY + (int)TileWorldPos.y - AdjustUpThreshold) * boardX);
//                Item.Type tempItemType = SpawnMan.SpawnOnChance(Random.Range(0f, 100f));
//                ItemBoardObjs[boardY][boardX].GetComponent<Item>().ItemType = tempItemType;
//            }
//            //ItemBoardObjs[y][col].GetComponent<Item>().SetType(ItemBoardTypeHistory[y + (int)TileWorldPos.y - AdjustUpThreshold][col]);
//        }
//    }

//}
