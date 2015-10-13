using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour
{
	void Update()
	{
		if (Input.GetButtonDown("Jump"))
		    Application.LoadLevel("Level 2");
		   }

}

