using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public Camera c;
    public float allowable_Value;   //페인트툴 색 차이 허용치
    public Color paintColor;

    private bool bIsClick;
    private bool bIsCollision;
    private Collider2D col;

    private Color[,] pixel;         //이미지의 컬러값을 담은 배열
    private Color referenceColor;   //바꿀 색의 기준점
    private List<int> openList;     //탐색해야하는 좌표
    private bool[,] vist;           //색을 바꿔줄 좌표

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bIsClick = true;
        }

        if (Input.GetMouseButtonUp(0) && bIsClick == true && bIsCollision)
        {
            bIsClick = false;
            painting(col.gameObject);
        }

        this.transform.position = c.ScreenToWorldPoint((Input.mousePosition + new Vector3(0, 0, 10)));
    }



    private void painting(GameObject gameObject)
    {
        //이미지의 중심 a, 가장 가까운 꼭짓점 b, 마우스 포인터의 위치 c
        //마우스좌표-(타겟 좌표-(타겟 크기))/타겟 크기 = 클릭한 이미지의 픽셀 위치(%)

        Vector2 imagePos = gameObject.transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject target = gameObject.transform.GetChild(0).gameObject;
        Vector2 imageSize = gameObject.GetComponent<BoxCollider2D>().size * gameObject.transform.localScale;
        pixel = new Color[target.GetComponent<SpriteRenderer>().sprite.texture.width, target.GetComponent<SpriteRenderer>().sprite.texture.height];

        var targetTexture = target.GetComponent<SpriteRenderer>().sprite.texture;
        Texture2D blankTexture = new Texture2D(targetTexture.width, targetTexture.height, TextureFormat.RGBA32, false);
        Graphics.CopyTexture(targetTexture, blankTexture);
        Sprite blankSprite = Sprite.Create(blankTexture, new Rect(0, 0, targetTexture.width, targetTexture.height), new Vector2(0.5f, 0.5f));
        gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().sprite = blankSprite;

        //이미지의 픽셀 x,y 좌표 구분하여 저장
        Color[] colorArr = target.GetComponent<SpriteRenderer>().sprite.texture.GetPixels(0);
        int count = 0;
        for (int y = 0; y < target.GetComponent<SpriteRenderer>().sprite.texture.height; y++)
        {
            for (int x = 0; x < target.GetComponent<SpriteRenderer>().sprite.texture.width; x++)
            {
                pixel[x, y] = colorArr[count];
                count++;
            }
        }
        vist = new bool[target.GetComponent<SpriteRenderer>().sprite.texture.width, target.GetComponent<SpriteRenderer>().sprite.texture.height];

        Vector2 temp = (mousePos - (imagePos - (imageSize / 2))) / imageSize;
        temp.x *= target.GetComponent<SpriteRenderer>().sprite.texture.width;
        temp.y *= target.GetComponent<SpriteRenderer>().sprite.texture.height;
        referenceColor = pixel[(int)(temp.x), (int)(temp.y)];

        bfs((int)temp.x, (int)temp.y);

        for (int i1 = 0; i1 < vist.GetLength(0); i1++)
        {
            for (int i2 = 0; i2 < vist.GetLength(1); i2++)
            {
                if (vist[i1, i2])
                {
                    target.GetComponent<SpriteRenderer>().sprite.texture.SetPixel(i1, i2, paintColor);
                }
            }
        }
        target.GetComponent<SpriteRenderer>().sprite.texture.Apply();
    }

    private void bfs(int _x, int _y)
    {
        openList = new List<int>();
        openList.Add(_x);
        openList.Add(_y);
        vist[_x, _y] = true;

        //무한루프 예외처리용 변수
        int count = 0;

        while (openList.Count > 0)
        {
            explore(openList[0] - 1, openList[1]);
            explore(openList[0], openList[1] - 1);
            explore(openList[0] + 1, openList[1]);
            explore(openList[0], openList[1] + 1);

            count++;

            //이미지 사이즈보다 탐색이 길때의 예외처리
            if (count > pixel.GetLength(0) * pixel.GetLength(1))
            {
                break;
            }
            

            openList.RemoveAt(0);
            openList.RemoveAt(0);
        }

        openList.Clear();
    }

    //탐색
    private void explore(int _x, int _y)
    {
        if (colorComparison(pixel[_x, _y]) > allowable_Value || _x <= 0 || _y <= 0 || _x >= pixel.GetLength(0) - 1 || _y >= pixel.GetLength(1) - 1 || vist[_x, _y])
        { return; }

        vist[_x, _y] = true;
        openList.Add(_x);
        openList.Add(_y);

        return;
    }

    //시작점의 색과 비교
    private float colorComparison(Color _c)
    {
        return math.abs(_c.r - referenceColor.r) + math.abs(_c.g - referenceColor.g) + math.abs(_c.b - referenceColor.b) + math.abs(_c.a - referenceColor.a);
    }

    private void OnTriggerEnter2D(Collider2D _col)
    {
        col = _col;
        bIsCollision = true;
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        bIsCollision = false;
    }

}
