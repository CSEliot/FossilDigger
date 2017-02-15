using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMan : MonoBehaviour {

    public Sprite[] Earth;
    public Sprite[] Background;

    public int TotalRows;
    public int TotalCol;

	// Use this for initialization
	void Start () {
        Debug.Log("Welp!");
        Arrange();
        Debug.Log("YES");
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
        float xPosRow = 112f;
        float yPosRow = -50f;
        float xPosCol = 0f;
        float yPosCol = 0f;
        for(int y = 0; y < TotalRows; y++)
        {
            transform.GetChild(y).GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosCol, yPosCol);
            for (int x = 0; x < TotalCol; x++)
            {
                transform.GetChild(y).GetChild(x).GetComponent<RectTransform>().anchoredPosition = new Vector2(xPosRow, yPosRow);
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
}
