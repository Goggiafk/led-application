using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PresetLEDControll : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private GameObject layoutprefab;
     public List<LayoutScript> layoutList;
    public bool isLoadingPreset = false;

    void Start()
    {
        if (!PlayerPrefs.HasKey("savesQuantity"))
        {
            PlayerPrefs.SetInt("savesQuantity", 0);
        }
        int quantity = PlayerPrefs.GetInt("savesQuantity");
        if (!(quantity - 1 < 0))
        {
            for (int i = 0; i < quantity; i++)
            {
                AddObject(i);
            }
        }
    }

    public void AddObject(int id)
    {
        var layoutM = Instantiate(layoutprefab, content.GetComponent<RectTransform>());
        layoutM.GetComponent<LayoutScript>().id = id;
        layoutList.Add(layoutM.GetComponent<LayoutScript>());
        
        layoutList[id] = layoutM.GetComponent<LayoutScript>();
        if ((id - 1) > -1)
        {
            layoutList[id - 1].DownButton.gameObject.SetActive(true);
        }
        if (id - 1 < 0)
            layoutM.GetComponent<LayoutScript>().UpButton.gameObject.SetActive(false);
        if (id + 1 >= PlayerPrefs.GetInt("savesQuantity") - 1)
            layoutM.GetComponent<LayoutScript>().DownButton.gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
