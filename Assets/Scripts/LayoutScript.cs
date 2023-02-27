using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutScript : MonoBehaviour
{
    [SerializeField] private GameObject matrixArea;
    [SerializeField] private GameObject layoutPixel;
    public GameObject[,] matrixOfPixels;
    public int id;
    void Start()
    {
        //StartCoroutine(Timer(0.25f, () => { 
        matrixOfPixels = new GameObject[(int)GameManager.Instance.drawScript.matrix.x, (int)GameManager.Instance.drawScript.matrix.y];
        string arg = PlayerPrefs.GetString("saveData" + id);
        Debug.Log(arg);
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
                Color rgb = new Color(r/255, g/255, b/255);
                matrixOfPixels[i, j].gameObject.GetComponent<LayoutPixelScript>().PaintPixel(rgb);
            }
        }
        //}));
    }

    public void LoadOnBadge()
    {
        StartCoroutine(GameManager.Instance.ledController.LoadStuff(id));
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
