using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UseofSpaceV2 : MonoBehaviour
{
    public Template templatePrefab;
    // Start is called before the first frame update
    void Start()
    {
        Displaytemplate();
    }
    void Displaytemplate()
    {
        Template newTemplate = Instantiate(templatePrefab) as Template;
        newTemplate.transform.SetParent(transform,false);
        newTemplate.LgoodTR.text = "7";
        newTemplate.LgoodTL.text = "6";
        newTemplate.LgoodBR.text = "11";
        newTemplate.LgoodBL.text = "10";
        newTemplate.LbadTTL.text = "2";
        newTemplate.LbadTLC.text = "1";
        newTemplate.LbadTLL.text = "5";
        newTemplate.LbadBLL.text = "9";
        newTemplate.LbadBLC.text = "13";
        newTemplate.LbadBBL.text = "14";
        newTemplate.LbadTTR.text = "3";
        newTemplate.LbadTRC.text = "4";
        newTemplate.LbadTRR.text = "8";
        newTemplate.LbadBRR.text = "12";
        newTemplate.LbadBRC.text = "16";
        newTemplate.LbadBBR.text = "15";
        newTemplate.RgoodTR.text = "7";
        newTemplate.RgoodTL.text = "6";
        newTemplate.RgoodBR.text = "11";
        newTemplate.RgoodBL.text = "10";
        newTemplate.RbadTTL.text = "2";
        newTemplate.RbadTLC.text = "1";
        newTemplate.RbadTLL.text = "5";
        newTemplate.RbadBLL.text = "9";
        newTemplate.RbadBLC.text = "13";
        newTemplate.RbadBBL.text = "14";
        newTemplate.RbadTTR.text = "3";
        newTemplate.RbadTRC.text = "4";
        newTemplate.RbadTRR.text = "8";
        newTemplate.RbadBRR.text = "12";
        newTemplate.RbadBRC.text = "16";
        newTemplate.RbadBBR.text = "15";


}
}
