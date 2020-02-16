using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static HelperClass;

public class MenuSelect : MonoBehaviour
{
    public int Index;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(GoToTarget);
    }
    void GoToTarget()
    {       
        CustomMenuManager.Instance.SetMenu(Index);        
    }
}
