using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public void Close()
    {
        Destroy(gameObject);
    }
}