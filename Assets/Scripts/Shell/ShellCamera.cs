using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellCamera : MonoBehaviour
{
    public Camera shellCamera;

    public void SetRect(Rect r) {
        shellCamera.rect = r;
    }
}
