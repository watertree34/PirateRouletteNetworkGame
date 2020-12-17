﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//마우스가 구멍위에 있으면 초록색으로 바뀌고 만약 클릭하면 칼생성
public class HoleScript : MonoBehaviour
{
    Renderer holeRenderer;
    Color originColor;

    public GameObject knife;

    bool Selected;
    void Start()
    {
        holeRenderer = gameObject.GetComponent<Renderer>();
        originColor = holeRenderer.material.color;

    }

    private void OnMouseEnter()  // 마우스 들어가면 초록색
    {
        if (Selected == false)
            holeRenderer.material.color = Color.green;
    }

    private void OnMouseExit() // 마우스 나오면 원래색
    {
        if (Selected == false)
            holeRenderer.material.color = originColor;
    }

    private void OnMouseDown()   // 마우스 클릭하면
    {
        if (Selected == false)
        {
            GameObject nowKnife = Instantiate(knife);  // 칼 생기기
            holeRenderer.material.color = Color.blue;  //파란색 바꿈
            nowKnife.transform.position = transform.position;  // 칼 위치
            nowKnife.transform.forward = -transform.up;  //칼 방향
            nowKnife.transform.position += nowKnife.transform.forward *1;  //살짝 앞에 위치
            Selected = true;  // 더 선택안되게
        }
    }

}
