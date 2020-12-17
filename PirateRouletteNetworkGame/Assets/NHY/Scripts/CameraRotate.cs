using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    float now_y_Angle;

    private void Start()
    {
        now_y_Angle = 0;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, now_y_Angle, transform.eulerAngles.z);
    }
    public void OnClickLeft()  // 왼쪽 버튼 클릭
    {
        now_y_Angle += 40;  // 40도씩 회전
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, now_y_Angle, transform.eulerAngles.z);
    }

    public void OnClickRight()  // 오른쪽 버튼 클릭
    {
        now_y_Angle -= 40; // -40도씩 회전
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, now_y_Angle, transform.eulerAngles.z);
    }
}
