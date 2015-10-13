using UnityEngine;
using System.Collections;

public class ExitAppOnButton : MonoBehaviour
{

    public string buttonName = "Cancel";

    void Update()
    {
        if (Input.GetButtonDown(buttonName))
            Application.Quit();
    }
}