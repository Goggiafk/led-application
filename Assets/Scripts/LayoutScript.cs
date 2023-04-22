using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LayoutScript : MonoBehaviour
{
    [SerializeField] private GameObject matrixArea;
    [SerializeField] private GameObject layoutPixel;
    public Button LoadButton;
    public Button DeleteButton;
    public Button EditButton;
    public Button UpButton;
    public Button DownButton;
    public GameObject[,] matrixOfPixels;
    public int id;
    void Start()
    {
        DrawMatrix();
    }

    public void DrawMatrix()
    {
        matrixOfPixels = new GameObject[(int)GameManager.Instance.drawScript.matrix.x, (int)GameManager.Instance.drawScript.matrix.y];
        string arg = PlayerPrefs.GetString("saveData" + id);
        string[] splitArray = arg.Split(char.Parse("`"));
        int k = 0;
        for (int i = 0; i < GameManager.Instance.drawScript.matrix.x; i++)
        {
            for (int j = 0; j < GameManager.Instance.drawScript.matrix.y; j++)
            {

                GameObject pixelx = Instantiate(layoutPixel);
                pixelx.transform.parent = matrixArea.GetComponent<RectTransform>();
                matrixOfPixels[i, j] = pixelx;
                int x = int.Parse(splitArray[k]);
                k++;
                int y = int.Parse(splitArray[k]);
                k++;
                matrixOfPixels[i, j].gameObject.GetComponent<LayoutPixelScript>().SetMatrixPosition(x, y);
                matrixOfPixels[i, j].transform.localScale = Vector3.one;

                float r = float.Parse(splitArray[k]);
                k++;
                float g = float.Parse(splitArray[k]);
                k++;
                float b = float.Parse(splitArray[k]);
                k++;
                Color rgb = new Color(r / 255, g / 255, b / 255);
                matrixOfPixels[i, j].gameObject.GetComponent<LayoutPixelScript>().PaintPixel(rgb);
            }
        }
    }

    public void LoadOnBadge()
    {
        GameManager.Instance.presetLEDControll.isLoadingPreset = true;
        var layouts = FindObjectsOfType<LayoutScript>();
        foreach (var layout in layouts)
        {
            bool isActive = !GameManager.Instance.presetLEDControll.isLoadingPreset;
            layout.LoadButton.interactable = isActive;
            layout.DeleteButton.interactable = isActive;
            layout.EditButton.interactable = isActive;
            layout.UpButton.interactable = isActive;
            layout.DownButton.interactable = isActive;
            GameManager.Instance.bottomPanel.SetActive(isActive);
        }
        StartCoroutine(GameManager.Instance.ledController.LoadStuff(id));
    }

    public void DeleteLayout()
    {
        int quantity = PlayerPrefs.GetInt("savesQuantity");
        for (int i = id; i+1 < quantity; i++)
        {
            int u = i + 1;
            string data = PlayerPrefs.GetString("saveData" + u);
            PlayerPrefs.SetString("saveData" + i, data);
            GameManager.Instance.presetLEDControll.layoutList[i] = GameManager.Instance.presetLEDControll.layoutList[u];
            GameManager.Instance.presetLEDControll.layoutList[i].id--;
        }

        
        GameManager.Instance.presetLEDControll.layoutList.RemoveAt(quantity-1);
        Debug.Log(GameManager.Instance.presetLEDControll.layoutList.Count);
        //GameManager.Instance.presetLEDControll.layoutList
        PlayerPrefs.DeleteKey("saveData" + (quantity - 1));
        PlayerPrefs.SetInt("savesQuantity", quantity - 1);

        if (PlayerPrefs.GetInt("savesQuantity") != 0)
        {
            GameManager.Instance.presetLEDControll.layoutList[0].UpButton.gameObject.SetActive(false);

            GameManager.Instance.presetLEDControll.layoutList[PlayerPrefs.GetInt("savesQuantity") - 1].DownButton.gameObject.SetActive(false);

        }
        Destroy(this.gameObject);
    }

    public void EditLayout()
    {
        var manag = GameManager.Instance;

        GameManager.Instance.ledController.editId = id;

        for (int i = 0; i < manag.drawScript.matrix.x ; i++)
        {
            for (int j = 0; j < manag.drawScript.matrix.y; j++)
            {
                string arg = PlayerPrefs.GetString("saveData" + id);
                string[] splitArray = arg.Split(char.Parse("`"));
                int k = 0;
                var matrixLength = GameManager.Instance.drawScript.matrix;
                for (int l = 0; l < matrixLength.x; l++)
                {
                    for (int m = 0; m < matrixLength.y; m++)
                    {
                        int x = int.Parse(splitArray[k]);
                        k++;
                        int y = int.Parse(splitArray[k]);
                        k++;
                        float r = float.Parse(splitArray[k]);
                        k++;
                        float g = float.Parse(splitArray[k]);
                        k++;
                        float b = float.Parse(splitArray[k]);
                        k++;
                        manag.drawScript.matrixOfPixels[y, x].gameObject.GetComponent<PixelScript>().PaintPixel(new Color(r / 255, g / 255, b / 255));
                    }
                }
            }
        }

        manag.ledController.SetElementActive(3);
    }

        public void MoveInList(bool isUp)
    {
        var savedData = PlayerPrefs.GetString("saveData" + id);
        if (isUp)
        {
            int upId = id - 1;
            var savedUpData = PlayerPrefs.GetString("saveData" + upId);
            PlayerPrefs.SetString("saveData" + upId, savedData);
            PlayerPrefs.SetString("saveData" + id, savedUpData);
            GameManager.Instance.presetLEDControll.layoutList[upId].ClearMatrix();
            GameManager.Instance.presetLEDControll.layoutList[upId].DrawMatrix();
        }
        else
        {
            int downId = id + 1;
            var savedUpData = PlayerPrefs.GetString("saveData" + downId);
            PlayerPrefs.SetString("saveData" + downId, savedData);
            PlayerPrefs.SetString("saveData" + id, savedUpData);
            GameManager.Instance.presetLEDControll.layoutList[downId].ClearMatrix();
            GameManager.Instance.presetLEDControll.layoutList[downId].DrawMatrix();
        }

        ClearMatrix();
        DrawMatrix();
    }

    public void ClearMatrix()
    {
        for (int i = 0; i < GameManager.Instance.drawScript.matrix.x; i++)
        {
            for (int j = 0; j < GameManager.Instance.drawScript.matrix.y; j++)
            {
                Destroy(matrixOfPixels[i, j].gameObject);
            }
        }
    }

    private IEnumerator Timer(float time, System.Action action)
    {
        while (time > 0)
        {
            time -= Time.deltaTime;
            yield return null;
        }

        action();
    }
}
