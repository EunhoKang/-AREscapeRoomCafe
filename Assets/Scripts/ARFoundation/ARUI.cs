using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ARUI : MonoBehaviour
{
    public void Sample(){
        var sample=new SampleData(AuthManager.manager.auth.CurrentUser.DisplayName, 123.45f);
        RealTimeDataManager.manager.PostObject<SampleData>($"rank/{AuthManager.manager.auth.CurrentUser.Email}/", sample,
            () => {}, Debug.Log);
    }
}
