using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class ImageRevealerWithDrag : MonoBehaviour
{

    public GameObject revealObj;
    public newDrag dragObj;
    public UnityEvent onSolved;
    public int r = 50;

    [Range(0f, 100f)]
    public float solvedPercentage = 100f;

    RectTransform rectTransform;
    Texture2D texture;
    Color32[] originalPixels;
    bool solved = false;

    void Start()
    {

        rectTransform = GetComponent<RectTransform>();

        RawImage img = GetComponent<RawImage>();
        Texture2D tex = (Texture2D)img.texture;
        revealObj.GetComponent<RawImage>().texture = tex;
        originalPixels = tex.GetPixels32();
        texture = new Texture2D(tex.width, tex.height);

        Color32[] pixels = texture.GetPixels32();

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color32(0, 0, 0, 0);

        }
        texture.SetPixels32(pixels);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;

        img.texture = texture;




    }
    public void OnDrag(PointerEventData eventData)
    {
        if (solved)
        {

            return;
        }


        Vector2 localCursor = new Vector2();

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localCursor))
        {

            Debug.Log(localCursor);
            Vector2 pivotCancelledCursor = new Vector2(localCursor.x - rectTransform.rect.x, localCursor.y - rectTransform.rect.y);

            if (WithinRange(pivotCancelledCursor.x, rectTransform.rect.width) && WithinRange(pivotCancelledCursor.y, rectTransform.rect.height))
            {

                Vector2 normalizedCursor = new Vector2(pivotCancelledCursor.x / rectTransform.rect.width, pivotCancelledCursor.y / rectTransform.rect.height);
                Vector2 coordinateImage = new Vector2(texture.width * normalizedCursor.x, texture.height * normalizedCursor.y);

                int x, y, px, nx, py, ny, d;
                Color32[] tempArray = texture.GetPixels32();

                int cx = (int)coordinateImage.x;
                int cy = (int)coordinateImage.y;

                for (x = 0; x <= r; x++)
                {
                    d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));

                    for (y = 0; y <= d; y++)
                    {
                        px = cx + x;
                        nx = cx - x;
                        py = cy + y;
                        ny = cy - y;

                        if (WithinRange(py, texture.height) && WithinRange(px, texture.width))
                        {
                            tempArray[py * texture.width + px] = Color.white;
                        }
                        if (WithinRange(py, texture.height) && WithinRange(nx, texture.width))
                        {
                            tempArray[py * texture.width + nx] = Color.white;
                        }
                        if (WithinRange(ny, texture.height) && WithinRange(px, texture.width))
                        {
                            tempArray[ny * texture.width + px] = Color.white;
                        }
                        if (WithinRange(ny, texture.height) && WithinRange(nx, texture.width))
                        {
                            tempArray[ny * texture.width + nx] = Color.white;
                        }


                        //tempArray[py * texture.width + px] = Color.white;
                        //tempArray[py * texture.width + nx] = Color.white;
                        //tempArray[ny * texture.width + px] = Color.white;
                        //tempArray[ny * texture.width + nx] = Color.white;

                    }
                }

                texture.SetPixels32(tempArray);
                texture.Apply();

            }

        }
    }

    bool WithinRange(float val, float range)
    {

        if (val >= 0 && val < range)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (solved)
        {
            return;
        }

        Color32[] tempArray = texture.GetPixels32();
        int totalAlphaPixelsCount = 0;
        int revealedCount = 0;


        for (int i = 0; i < originalPixels.Length; i++)
        {

            Color32 pixel = originalPixels[i];
            if (pixel.a == 255)
            {
                totalAlphaPixelsCount++;
                if (tempArray[i].a == 255)
                {
                    revealedCount++;

                }
            }
        }

        float revealedPercentage = revealedCount * 100 / totalAlphaPixelsCount;

        if (revealedPercentage >= solvedPercentage)
        {
            solved = true;

            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = Color.white;


            }

            texture.SetPixels32(tempArray);
            texture.Apply();

            onSolved?.Invoke();

        }


    }

    private void Update()
    {
        if (solved) {
            return;
        }

        Color32[] tempArray;
        if (dragObj.isDragging) {
            Vector2 localCursor = new Vector2();

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, Camera.main, out localCursor))
            {

                Debug.Log(localCursor);
                Vector2 pivotCancelledCursor = new Vector2(localCursor.x - rectTransform.rect.x, localCursor.y - rectTransform.rect.y);

                if (WithinRange(pivotCancelledCursor.x, rectTransform.rect.width) && WithinRange(pivotCancelledCursor.y, rectTransform.rect.height))
                {

                    Vector2 normalizedCursor = new Vector2(pivotCancelledCursor.x / rectTransform.rect.width, pivotCancelledCursor.y / rectTransform.rect.height);
                    Vector2 coordinateImage = new Vector2(texture.width * normalizedCursor.x, texture.height * normalizedCursor.y);

                    int x, y, px, nx, py, ny, d;
                    tempArray = texture.GetPixels32();

                    int cx = (int)coordinateImage.x;
                    int cy = (int)coordinateImage.y;

                    for (x = 0; x <= r; x++)
                    {
                        d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));

                        for (y = 0; y <= d; y++)
                        {
                            px = cx + x;
                            nx = cx - x;
                            py = cy + y;
                            ny = cy - y;

                            if (WithinRange(py, texture.height) && WithinRange(px, texture.width))
                            {
                                tempArray[py * texture.width + px] = Color.white;
                            }
                            if (WithinRange(py, texture.height) && WithinRange(nx, texture.width))
                            {
                                tempArray[py * texture.width + nx] = Color.white;
                            }
                            if (WithinRange(ny, texture.height) && WithinRange(px, texture.width))
                            {
                                tempArray[ny * texture.width + px] = Color.white;
                            }
                            if (WithinRange(ny, texture.height) && WithinRange(nx, texture.width))
                            {
                                tempArray[ny * texture.width + nx] = Color.white;
                            }


                            //tempArray[py * texture.width + px] = Color.white;
                            //tempArray[py * texture.width + nx] = Color.white;
                            //tempArray[ny * texture.width + px] = Color.white;
                            //tempArray[ny * texture.width + nx] = Color.white;

                        }
                    }

                    texture.SetPixels32(tempArray);
                    texture.Apply();

                }

            }


        }

        tempArray = texture.GetPixels32();
        int totalAlphaPixelsCount = 0;
        int revealedCount = 0;


        for (int i = 0; i < originalPixels.Length; i++)
        {

            Color32 pixel = originalPixels[i];
            if (pixel.a == 255)
            {
                totalAlphaPixelsCount++;
                if (tempArray[i].a == 255)
                {
                    revealedCount++;

                }
            }
        }

        float revealedPercentage = revealedCount * 100 / totalAlphaPixelsCount;

        if (revealedPercentage >= solvedPercentage)
        {
            solved = true;

            for (int i = 0; i < tempArray.Length; i++)
            {
                tempArray[i] = Color.white;


            }

            texture.SetPixels32(tempArray);
            texture.Apply();

            onSolved?.Invoke();

        }



    }


}

