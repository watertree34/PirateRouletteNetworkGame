using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionColumnScript : MonoBehaviour
{
    int nowNum = 0;
    public int personNum = -1;
    
    public int randomNum = 0;
    public GameObject person;
    Rigidbody rb;
    public GameObject gameOver;
    public GameObject pass;

    public Image edge;
    public Image noActive;

    public Client cl;

    private GameObject nowKnife;

  

    private void Start()
    {
        randomNum = Random.Range(1, 18);   // 랜덤 숫자 정하기
        rb = person.gameObject.GetComponent<Rigidbody>();  //해적의 리지드바디
        gameOver.SetActive(false);
        pass.SetActive(false);
        edge.enabled = false;
        noActive.enabled = false;
    }
    private void Update()
    {
        if (personNum != cl.clientID)
        {
            edge.enabled = false;
            noActive.enabled = true;
        }
        else
        {
            edge.enabled = true;
            noActive.enabled = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Knife")  // 칼이 들어올때
        {
            KnifePlus(other.gameObject);
        }
    }

    public void KnifePlus(GameObject nowKnife)  // 칼 추가 함수
    {
        this.nowKnife = nowKnife;
        nowKnife.GetComponent<KnifeScript>().enabled = false;  // 움직임 코드를 멈추고
        nowNum++;  //숫자 증가
        ActiveScript.Instance.active = true;  // 칼이 다 들어가면 다른거 선택가능

        if (nowNum == randomNum)   // 만약 현재 숫자가 랜덤숫자면 
        {
        
            ActiveScript.Instance.active = false;  // 끝나면 다른거 선택불가
            cl.Killed();
        }
        else if(cl.turn == personNum)
        {
     
            cl.TurnPass();
        }
    }

    public IEnumerator Pass(int turn)  //통과 효과
    {
        personNum = turn;
        pass.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        pass.SetActive(false);
    }

    public void Finish()  // 종료 효과
    {
        // 해적 튀어나가기
        nowKnife.GetComponent<Renderer>().material.color = Color.red;  // 칼 색깔 빨강으로 바뀜

        rb.AddForce(Vector3.up * 100 + (-Vector3.right * 50));

        gameOver.SetActive(true);

        StartCoroutine(CoFinish());
    }

    IEnumerator CoFinish()
    {
        yield return new WaitForSeconds(1f);
        
        Application.Quit();
    }
}
