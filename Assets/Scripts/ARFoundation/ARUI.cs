using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ARUI : MonoBehaviour
{
    public void Sample(){
        var sample=new SampleData(AuthManager.manager.auth.CurrentUser.DisplayName, new List<double>(){0.1f,0.2f});
        RealTimeDataManager.manager.PostObject<SampleData>($"rank/{AuthManager.manager.auth.CurrentUser.DisplayName}", sample,
            () => {}, Debug.Log);
    }
}
