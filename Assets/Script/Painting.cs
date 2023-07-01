using System;
using UnityEngine;

public class Painting : MonoBehaviour
{
    public Camera c;
    public int allowable_Value;

    private bool bIsClick;
    private bool bIsCollision;
    private Collider2D col;
    private Color[,] pixel;

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
        //�̹����� �߽� a, ���� ����� ������ b, ���콺 �������� ��ġ c
        //���콺��ǥ-(Ÿ�� ��ǥ-(Ÿ�� ũ��))/Ÿ�� ũ�� = Ŭ���� �̹����� �ȼ� ��ġ(%)

        Vector2 imagePos = gameObject.transform.position;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject target = gameObject.transform.GetChild(0).gameObject;
        //Vector2 imageSize = new Vector2(target.GetComponent<SpriteRenderer>().sprite.texture.width, target.GetComponent<SpriteRenderer>().sprite.texture.height);
        Vector2 imageSize = gameObject.GetComponent<BoxCollider2D>().size * gameObject.transform.localScale;
        pixel = new Color[target.GetComponent<SpriteRenderer>().sprite.texture.width, target.GetComponent<SpriteRenderer>().sprite.texture.height];

        Color referenceColor;


        //�̹����� �ȼ� x,y ��ǥ �����Ͽ� ����
        Color[] colorArr = target.GetComponent<SpriteRenderer>().sprite.texture.GetPixels(0);
        int count = 0;
        for (int x = 0; x < target.GetComponent<SpriteRenderer>().sprite.texture.width; x++)
        {
            for (int y = 0; y < target.GetComponent<SpriteRenderer>().sprite.texture.height; y++)
            {
                pixel[x, y] = colorArr[count];
                count++;
            }
        }

        Debug.Log(mousePos);
        Vector2 temp = (mousePos - (imagePos - (imageSize / 2))) / imageSize;
        Debug.Log(temp);
        referenceColor = pixel[(int)(temp.x * target.GetComponent<SpriteRenderer>().sprite.texture.width), (int)(temp.y * target.GetComponent<SpriteRenderer>().sprite.texture.width)];

        Debug.Log(referenceColor);
    }

    private void OnTriggerEnter2D(Collider2D _col)
    {
        col = _col;
        bIsCollision = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        bIsCollision = false;
    }
}
