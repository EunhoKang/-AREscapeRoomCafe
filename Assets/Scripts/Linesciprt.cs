using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Linesciprt : MonoBehaviour
{
    public Image lineImg;
    private Vector3 startPos;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lineImg.gameObject.SetActive(true);
            startPos = Input.mousePosition;
            lineImg.transform.position = startPos;

        }
        if (Input.GetMouseButton(0))
        {
            Vector3 myPos = Input.mousePosition;
            lineImg.transform.localScale = new Vector2(Vector3.Distance(myPos, startPos), 1);
            lineImg.transform.localRotation = Quaternion.Euler(0, 0,
                AngleInDeg(startPos, myPos));

        }
        if (Input.GetMouseButtonUp(0))
        {
            lineImg.gameObject.SetActive(false);
        }
    }

    public static float AngleInRad(Vector3 vec1, Vector3 vec2)
    {
        return Mathf.Atan2(vec2.y - vec1.y, vec2.x - vec1.x);
    }

    public static float AngleInDeg(Vector3 vec1, Vector3 vec2)
    {
        return AngleInRad(vec1, vec2) * 180 / Mathf.PI;
    }
}
