using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionColumnScript : MonoBehaviour
{
    int nowNum = 0;

    int randomNum = 0;
    public GameObject person;
    Rigidbody rb;
    public GameObject gameOver;
    private void Start()
    {
        randomNum = Random.Range(0, 18);   // 랜덤 숫자 정하기
        rb = person.gameObject.GetComponent<Rigidbody>();  //해적의 리지드바디
        gameOver.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)  
    {
        if (other.tag == "Knife")  // 칼이 들어올때
        {
            other.gameObject.GetComponent<KnifeScript>().enabled = false;  // 움직임 코드를 멈추고
           
            nowNum++;  //숫자 증가
          
            if (nowNum == randomNum)   // 만약 현재 숫자가 랜덤숫자면 
            {
              
                other.gameObject.GetComponent<Renderer>().material.color = Color.red;  // 칼 색깔 빨강으로 바뀜


                // 해적 튀어나가기
                
                rb.AddForce(Vector3.up* 100+(-Vector3.right*50));

                gameOver.SetActive(true);

            }
        }
    }
}
