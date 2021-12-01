using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankProperties : MonoBehaviour
{
    public Rect rect;
    public Camera shoulderCamera;

    public void SetRect(Rect r) {
        rect = shoulderCamera.rect = r;
    }
}
