using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//마우스가 구멍위에 있으면 초록색으로 바뀌고 만약 클릭하면 칼생성
public class HoleScript : MonoBehaviour
{
    Renderer holeRenderer;
    Color originColor;

    public GameObject knife;

    bool Selected;

    public Client cl;
    public int idx;


    private CollisionColumnScript ccs;
    void Start()
    {
        holeRenderer = gameObject.GetComponent<Renderer>();
        originColor = holeRenderer.material.color;
        idx = transform.GetSiblingIndex();
        ccs = FindObjectOfType<CollisionColumnScript>();
    }

    private void OnMouseEnter()  // 마우스 들어가면 초록색
    {
        if (ccs.personNum != cl.clientID)
            return;
        
        if ((Selected == false)&& (ActiveScript.Instance.active ==true)) // 선택된적이 없고 선택 가능한상태이면
            holeRenderer.material.color = Color.green;
    }

    private void OnMouseExit() // 마우스 나오면 원래색
    {
        if (ccs.personNum != cl.clientID)
            return;

        if ((Selected == false) && (ActiveScript.Instance.active == true))
            holeRenderer.material.color = originColor;
    }

    private void OnMouseDown()   // 마우스 클릭하면
    {
        if (ccs.personNum != cl.clientID)
            return;

       cl.PutKnife(idx);
    }

    public void SpawnKnife()
    {
        if ((Selected == false) && (ActiveScript.Instance.active == true))
        {
            GameObject nowKnife = Instantiate(knife);  // 칼 생기기
            holeRenderer.material.color = Color.gray;  //회색 바꿈
            nowKnife.transform.position = transform.position;  // 칼 위치
            nowKnife.transform.forward = -transform.up;  //칼 방향
            nowKnife.transform.position += nowKnife.transform.forward *1;  //살짝 앞에 위치
            Selected = true;  // 더 선택안되게
            ActiveScript.Instance.active = false;  // 칼이 들어갈때까지 다른거 선택안되게
        }
    }
}
