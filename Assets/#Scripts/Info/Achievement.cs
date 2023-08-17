using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievement : MonoBehaviour
{
    public Text title;
    public GameObject mask;
    public List<Achievement_Item> itemList;

    private string category = "";

    public void SetCategory(string _name)
    {
        category = _name;
        title.text = _name + " (" + GetNow() + " / " + GetMax() + ")";
    }

    public int GetNow()
    {
        return itemList.Count;
    }

    public int GetMax()
    {
        return GameManager._instance.GetCategoryCount(GetCategory());
    }

    public string GetCategory()
    {
        return category;
    }
}
