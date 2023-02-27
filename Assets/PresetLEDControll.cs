using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PresetLEDControll : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject layoutprefab;

    void Start()
    {
        if (!PlayerPrefs.HasKey("savesQuantity"))
        {
            PlayerPrefs.SetInt("savesQuantity", 0);
        }
        int quanity = PlayerPrefs.GetInt("savesQuantity");
        for (int i = 0; i < quanity; i++)
        {
            AddObject(i);
        }
    }

    public void AddObject(int id)
    {
        var layoutM = Instantiate(layoutprefab, content.GetComponent<RectTransform>()).GetComponent<LayoutScript>().id = id;
    }

    void Update()
    {
        
    }
}
