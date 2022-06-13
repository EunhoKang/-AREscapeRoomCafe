using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ItemExplan : MonoBehaviour
{
    public TextMeshProUGUI Description;
    

    void Start()
    {
        Description.text = "아이템 설명을 보시려면 \n 마커를 클릭하세요";
    }
    // Update is called once per frame

    public void A_button()
    {
        Description.text = "고양이들이 A를 그리며 앉아있다. \n\n A는 ASCII코드로 65라고한다";
    }

    public void B_button()
    {
        Description.text = "비오는 날에는 우산을 쓰는 것이 좋다. \n\n 우산을 몇번 써야 비가 그칠까?";
    }
    public void C_button()
    {
        Description.text = "Kevin이 가장 싫어하는 유령이다. \n\n 그가 유령을 마주쳤을 때를 생각해보아라";
    }
    public void D_button()
    {
        Description.text = "D모양으로 흩어진 퍼즐조각이다. \n\n 4번째 빈칸은 D이다.";
    }
    public void E_button()
    {
        Description.text = "E버튼";
    }
}
