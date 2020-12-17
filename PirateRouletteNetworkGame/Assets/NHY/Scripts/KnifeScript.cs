using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//생성되면 앞으로 가다가 column에 닿으면 멈춤
public class KnifeScript : MonoBehaviour
{
    public float speed = 3;
    bool meetCol = false;

    // Update is called once per frame
    void Update()
    {
        if (meetCol == false)
            transform.position += -transform.forward * speed * Time.deltaTime;   // 닿지 않으면 앞으로 감

    }

}
