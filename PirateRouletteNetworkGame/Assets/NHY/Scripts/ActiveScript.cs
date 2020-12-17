using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//n번째 플레이어의 조작을 제어
public class ActiveScript : MonoBehaviour
{
    public static ActiveScript Instance;
    public bool active; //선택을 활성화해주는 변수


    private void Awake()
    {
        Instance = this;
        active = true;
    }
}
