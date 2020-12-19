using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    int i = 0;
    bool isFake = false;
    public PlayerScript player;
    private IntelligenceContainer intel;

    // Start is called before the first frame update
    void Start()
    {

        Application.targetFrameRate = 25;
        QualitySettings.vSyncCount = 0;

        //intel = player.GetComponentInChildren<IntelligenceContainer>();
    }

    // Update is called once per frame
    void Update()
    {
        //i++;
        //if(i > 200) {
        //    i = 0;
        //    if (isFake == true) intel.SetIntelligenceType(IntelligenceContainer.IntelligenceType.USER);
        //    else intel.SetIntelligenceType(IntelligenceContainer.IntelligenceType.ARTIFICIAL);

        //    isFake = !isFake;
        //}

    }
}
