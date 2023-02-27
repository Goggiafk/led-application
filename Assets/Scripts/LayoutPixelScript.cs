using UnityEngine;
using UnityEngine.EventSystems; // 1
using UnityEngine.UI;

public class LayoutPixelScript : MonoBehaviour
{
    Image sprite;
    public Vector2 matrixPosition;
    public Color rgb;

    void Awake()
    {
        sprite = GetComponent<Image>();
    }

    public Vector2 GetPixelPosition()
    {
        return matrixPosition;
    }

    public void SetMatrixPosition(int x, int y)
    {
        matrixPosition.x = x;
        matrixPosition.y = y;
    }

    public void PaintPixel(Color color)
    {
        rgb = color;
        sprite.color = color;
    }

    public void LightPixel(Color color)
    {
        sprite.color = color;
        GameManager.Instance.ledController.PaintPixel(((int)matrixPosition.x), (int)matrixPosition.y, color);
    }

    //public void LightPixelRGB(int o)
    //{
    //    sprite.color = Color.black;
    //    GameManager.Instance.ledController.PaintPixel(((int)matrixPosition.x), (int)matrixPosition.y, GameManager.Instance.drawScript.fcp.color);
    //}

    //public void OnPointerClick(PointerEventData eventData)
    //{

    //}
    //public void OnDrag(PointerEventData eventData)
    //{
    //    print("I'm being dragged!");
    //    target = Color.magenta;
    //}

    //public void OnPointerEnter(PointerEventData eventData)
    //{
    //    target = Color.green;
    //}
}
