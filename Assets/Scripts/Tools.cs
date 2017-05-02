using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour {

    public Sprite k;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// For delaying a function call by a single frame.
    /// </summary>
    /// <param name="call"></param>
    /// <returns></returns>
    public static IEnumerator DelayAction(UnityEngine.Events.UnityAction call)
    {
        //CURRENTLY DOESN"T WORK.
        yield return 0;
        call();
    }
}
