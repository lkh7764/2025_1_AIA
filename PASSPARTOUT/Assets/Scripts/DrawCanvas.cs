using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;

public interface IGetCDrawCall
{

}

public class DrawCanvas : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    // raw image & texture
    private RawImage paintArea;
    private Texture2D canvasTex;
    private int texWidth;
    private int texHeight;

    // brush setting
    private Color brushColor;
    private int brushSize;

    // observer
    private Queue<Vector2> drawQueue;



    // life cycle
    private void Awake()
    {
        drawQueue = new Queue<Vector2>();
    }
    private void Start()
    {
        // raw image & texture
        paintArea = GetComponent<RawImage>();

        Rect rect = paintArea.rectTransform.rect;
        texWidth = (int)rect.width;
        texHeight = (int)rect.height;


        InitBrushSetting();
    }
    private void Update()
    {
        if (drawQueue.Count < 1) return;
        
        while (drawQueue.Count > 0)
        {
            Vector2 localPos = drawQueue.Dequeue();
            drawAtLocalPosition(localPos);
        }

        canvasTex.Apply();
    }



    // draw
    private void draw(PointerEventData eventData)
    {
        Vector2 localPos;
        var rt = paintArea.rectTransform;

        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rt, eventData.position, eventData.pressEventCamera, out localPos)) return;

        float halfW = texWidth * 0.5f;
        float halfH = texHeight * 0.5f;
        int x = Mathf.Clamp((int)(localPos.x + halfW), 0, canvasTex.width - 1);
        int y = Mathf.Clamp((int)(localPos.y + halfH), 0, canvasTex.height - 1);

        int sx, sy;
        for (int i = -brushSize; i <= brushSize; ++i)
        {
            for (int j = -brushSize; j <= brushSize; ++j)
            {
                sx = x + i;
                sy = y + j;
                if (sx < 0 || sx >= canvasTex.width || sy < 0 || sy >= canvasTex.height) continue;

                canvasTex.SetPixel(sx, sy, brushColor);
            }
        }

        canvasTex.Apply();
    }
    public void OnPointerDown(PointerEventData eventData)   
    {
        //draw(eventData); 
        enqueueLocalPos(eventData);
    }
    public void OnDrag(PointerEventData eventData)          
    {
        //draw(eventData); 
        enqueueLocalPos(eventData);
    }
    private void enqueueLocalPos(PointerEventData eventData)
    {
        Vector2 localPos;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(paintArea.rectTransform, 
            eventData.position, eventData.pressEventCamera, out localPos)) return;

        drawQueue.Enqueue(localPos);
    }
    private void drawAtLocalPosition(Vector2 localPos)
    {
        float halfW = texWidth * 0.5f;
        float halfH = texHeight * 0.5f;
        int x = Mathf.Clamp((int)(localPos.x + halfW), 0, canvasTex.width - 1);
        int y = Mathf.Clamp((int)(localPos.y + halfH), 0, canvasTex.height - 1);

        int sx, sy;
        for (int i = -brushSize; i <= brushSize; ++i)
        {
            for (int j = -brushSize; j <= brushSize; ++j)
            {
                sx = x + i;
                sy = y + j;
                if (sx < 0 || sx >= canvasTex.width || sy < 0 || sy >= canvasTex.height) continue;

                canvasTex.SetPixel(sx, sy, brushColor);
            }
        }
    }




    // initiate
    public void InitBrushSetting(int dBrushSize = 4)
    {
        ChangeBrushColor(Color.white);
        ChangeBrushSize(dBrushSize);
    }



    // change brush setting
    public void ChangeBrushColor(Color color)
    {
        brushColor = color;
    }
    public void ChangeBrushSize(int size)
    {
        brushSize = size;
    }


    
    public void AllocateNewTexture(Texture2D tex) { canvasTex = tex; }
}
