using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPanel : MonoBehaviour
{
    public void OnButtonClick(GameObject tool)
    {
        tool.SetActive(!tool.activeSelf);
    }
}