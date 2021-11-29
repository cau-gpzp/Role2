using UnityEngine;
using System.Collections;

public class WheelRota : MonoBehaviour {
	
	public Vector3 rotateSpeeds = new Vector3(0.0f, 0.0f, 0.0f);
	private bool CheckRotate;
	void Update () {
		CheckRotate = true;
		{
			if(CheckRotate){
				transform.Rotate(rotateSpeeds * Time.smoothDeltaTime);
			}

		}
	}
}