using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    float now_y_Angle;
    public Client cl;

    private void Start()
    {
        now_y_Angle = 0;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, now_y_Angle, transform.eulerAngles.z);
    }

    public void ChangeAngle(float yAngle)  // 해적통 돌리는 함수
    {

        now_y_Angle += yAngle;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, now_y_Angle, transform.eulerAngles.z);
    }

    public void OnClickLeft()  // 왼쪽 버튼 클릭
    {
        if (ActiveScript.Instance.active == true)
        {
            ChangeAngle(40);   // 40도씩 회전
            cl.SetCamPointValue(transform.eulerAngles.y);
        }
    }

    public void OnClickRight()  // 오른쪽 버튼 클릭
    {
        if (ActiveScript.Instance.active == true)
        {
            ChangeAngle(-40); // -40도씩 회전
            cl.SetCamPointValue(transform.eulerAngles.y);
        }
    }


}
