using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.IO;
using System.Linq;
using System;

public class UseofSpaceV2 : MonoBehaviour
{
    public Template templatePrefab;
    // Start is called before the first frame update
    void Start()
    {
        FileInfo newestFile = new FileInfo(MainManager.Instance.dirPath.Substring(0, MainManager.Instance.dirPath.LastIndexOf("/")) + "/data.json");
        print(newestFile.FullName);
        string json = File.ReadAllText(newestFile.FullName);
        ParseData(json);
        Displaytemplate();
    }   

List<float> LeftyVals = new List<float>();
List<float> LeftxVals = new List<float>();
List<float> RightyVals = new List<float>();
List<float> RightxVals = new List<float>();

public void ParseData(string s)
{
    JSONNode n = JSON.Parse(s);
    n = n["frames"];
    for (int i = 0; i < n.Count; i++)
    {
        LeftyVals.Add(n[i]["data"][0]["yVal"]);
        LeftxVals.Add(n[i]["data"][0]["xVal"]);
        RightyVals.Add(n[i]["data"][1]["yVal"]);
        RightxVals.Add(n[i]["data"][1]["xVal"]);
    }
}
private static FileInfo GetNewestFile(DirectoryInfo directory)
{
        return directory.GetFiles()
            .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
            .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
            .FirstOrDefault();
}
// Checks and returns an int based on which zone the x and y value are on the Good/Green zones of the template 
public int isGoodZone(float x, float y)
{
        if ((x >= -.50 && x <= 0) && (y <= .50 && y >= 0)) // Green/Good Top Left 
            return 1;
        if ((x <= .50 && x >= 0) && (y <= .50 && y >= 0)) // Green/Good Top Right
            return 2;
        if ((x >= -.50 && x <= 0) && (y >= -.50 && y <= 0)) // Green/Good Bottom Left 
            return 3;
        if ((x <= .50 && x >= 0) && (y >= -.50 && y <= 0)) // Green/Good Bottom Right
            return 4;
        return 0;
}
// Checks and returns an int based on which zone the x and y value are on the corners of the template 
public int isExtremeCorners(float x, float y)
{
        if (x < -.50 && y > .50) //Top left corner
            return 1;
        if (x > .50 && y > .50) //Top right corner
            return 2;
        if (x < -.50 && y < -.50) //Bottom left corner
            return 3;
        if (x > .50 && y < -.50) //Bottom right corner
            return 4;
        return 0;
}
// Checks and returns an int based on which zone the x and y value are on the edges of the template 
public int isExtremeEdges(float x, float y)
{
        if ((x >= -.50 && x <= 0) && y > .50) //Top left upper edge 
            return 1;
        if (x < -.50 && (y <= .50 && y >= 0)) //Top left leftward edge 
            return 2;
        if ((x >= -.50 && x <= 0) && y < -.50) //Bottom left lower edge 
            return 3;
        if (x < -.50 && (y >= -.50 && y <= 0)) //Bottom left leftward edge 
            return 4;
        if ((x <= .50 && x >= 0) && y > .50) //Top right upper edge 
            return 5;
        if (x > .50 && (y <= .50 && y >= 0)) //Top right rightward edge 
            return 6;
        if ((x <= .50 && x >= 0) && y < -.50) // Bottom right lower edge 
            return 7;
        if (x > .50 && (y >= -.50 && y <= 0)) // Bottom right rightward edge 
            return 8;
        return 0;
}
public void Displaytemplate()
 {
        float totalframes = LeftyVals.Count;
        //Initializing frame counter values for the left and right hands
        float LbadTLCcnt = 0;
        float LbadBLCcnt = 0;
        float LbadTRCcnt = 0;
        float LbadBRCcnt = 0;

        float LbadTTLcnt = 0;
        float LbadTLLcnt = 0;
        float LbadBBLcnt = 0;
        float LbadBLLcnt = 0;
        float LbadTTRcnt = 0;
        float LbadTRRcnt = 0;
        float LbadBBRcnt = 0;
        float LbadBRRcnt = 0;

        float LgoodTLcnt = 0;
        float LgoodTRcnt = 0;
        float LgoodBLcnt = 0;
        float LgoodBRcnt = 0;

        float RbadTLCcnt = 0;
        float RbadBLCcnt = 0;
        float RbadTRCcnt = 0;
        float RbadBRCcnt = 0;

        float RbadTTLcnt = 0;
        float RbadTLLcnt = 0;
        float RbadBBLcnt = 0;
        float RbadBLLcnt = 0;
        float RbadTTRcnt = 0;
        float RbadTRRcnt = 0;
        float RbadBBRcnt = 0;
        float RbadBRRcnt = 0;

        float RgoodTLcnt = 0;
        float RgoodTRcnt = 0;
        float RgoodBLcnt = 0;
        float RgoodBRcnt = 0;
        // For loop to iterate though each the left hand and right hand values and counts the amount of frames for each zone on the template 
        for (int i = 0; i < totalframes; i++)
        {
            // Checking and counting for the left hand
            if (isExtremeCorners(LeftxVals[i], LeftyVals[i]) == 1) //Top left corner
                LbadTLCcnt++;
            if (isExtremeCorners(LeftxVals[i], LeftyVals[i]) == 2) //Top right corner
                LbadTRCcnt++;
            if (isExtremeCorners(LeftxVals[i], LeftyVals[i]) == 3) //Bottom left corner
                LbadBLCcnt++;
            if (isExtremeCorners(LeftxVals[i], LeftyVals[i]) == 4) //Bottom right corner
                LbadBRCcnt++;
            if (isExtremeEdges(LeftxVals[i], LeftyVals[i]) == 1) //Top left upper edge 
                LbadTTLcnt++;
            if (isExtremeEdges(LeftxVals[i], LeftyVals[i]) == 2) //Top left leftward edge 
                LbadTLLcnt++;
            if (isExtremeEdges(LeftxVals[i], LeftyVals[i]) == 3) //Bottom left lower edge
                LbadBBLcnt++;
            if (isExtremeEdges(LeftxVals[i], LeftyVals[i]) == 4) //Bottom left leftward edge
                LbadBLLcnt++;
            if (isExtremeEdges(LeftxVals[i], LeftyVals[i]) == 5) //Top right upper edge
                LbadTTRcnt++;
            if (isExtremeEdges(LeftxVals[i], LeftyVals[i]) == 6) //Top right rightward edge
                LbadTRRcnt++;
            if (isExtremeEdges(LeftxVals[i], LeftyVals[i]) == 7) // Bottom right lower edge
                LbadBBRcnt++;
            if (isExtremeEdges(LeftxVals[i], LeftyVals[i]) == 8) // Bottom right rightward edge 
                LbadBRRcnt++;
            if (isGoodZone(LeftxVals[i], LeftyVals[i]) == 1) // Green/Good Top Left
                LgoodTLcnt++;
            if (isGoodZone(LeftxVals[i], LeftyVals[i]) == 2) // Green/Good Top Right
                LgoodTRcnt++;
            if (isGoodZone(LeftxVals[i], LeftyVals[i]) == 3) // Green/Good Bottom Left
                LgoodBLcnt++;
            if (isGoodZone(LeftxVals[i], LeftyVals[i]) == 4) // Green/Good Bottom Right
                LgoodBRcnt++;
            // Checking and counting for the right hand 
            if (isExtremeCorners(RightxVals[i], RightyVals[i]) == 1) //Top left corner
                RbadTLCcnt++;
            if (isExtremeCorners(RightxVals[i], RightyVals[i]) == 2) //Top right corner
                RbadTRCcnt++;
            if (isExtremeCorners(RightxVals[i], RightyVals[i]) == 3) //Bottom left corner
                RbadBLCcnt++;
            if (isExtremeCorners(RightxVals[i], RightyVals[i]) == 4) //Bottom right corner
                RbadBRCcnt++;
            if (isExtremeEdges(RightxVals[i], RightyVals[i]) == 1) //Top left upper edge 
                RbadTTLcnt++;
            if (isExtremeEdges(RightxVals[i], RightyVals[i]) == 2) //Top left leftward edge 
                RbadTLLcnt++;
            if (isExtremeEdges(RightxVals[i], RightyVals[i]) == 3) //Bottom left lower edge
                RbadBBLcnt++;
            if (isExtremeEdges(RightxVals[i], RightyVals[i]) == 4) //Bottom left leftward edge
                RbadBLLcnt++;
            if (isExtremeEdges(RightxVals[i], RightyVals[i]) == 5) //Top right upper edge
                RbadTTRcnt++;
            if (isExtremeEdges(RightxVals[i], RightyVals[i]) == 6) //Top right rightward edge
                RbadTRRcnt++;
            if (isExtremeEdges(RightxVals[i], RightyVals[i]) == 7) // Bottom right lower edge
                RbadBBRcnt++;
            if (isExtremeEdges(RightxVals[i], RightyVals[i]) == 8) // Bottom right rightward edge 
                RbadBRRcnt++;
            if (isGoodZone(RightxVals[i], RightyVals[i]) == 1) // Green/Good Top Left
                RgoodTLcnt++;
            if (isGoodZone(RightxVals[i], RightyVals[i]) == 2) // Green/Good Top Right
                RgoodTRcnt++;
            if (isGoodZone(RightxVals[i], RightyVals[i]) == 3) // Green/Good Bottom Left
                RgoodBLcnt++;
            if (isGoodZone(RightxVals[i], RightyVals[i]) == 4) // Green/Good Bottom Right
                RgoodBRcnt++;
        }
        Template newTemplate = Instantiate(templatePrefab) as Template;
        newTemplate.transform.SetParent(transform,false);
        newTemplate.LgoodTR.text = ((LgoodTRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LgoodTL.text = ((LgoodTLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LgoodBR.text = ((LgoodBRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LgoodBL.text = ((LgoodBLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadTTL.text = ((LbadTTLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadTLC.text = ((LbadTLCcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadTLL.text = ((LbadTLLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadBLL.text = ((LbadBLLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadBLC.text = ((LbadBLCcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadBBL.text = ((LbadBBLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadTTR.text = ((LbadTTRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadTRC.text = ((LbadTRCcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadTRR.text = ((LbadTRRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadBRR.text = ((LbadBRRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadBRC.text = ((LbadBRCcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.LbadBBR.text = ((LbadBBRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RgoodTR.text = ((RgoodTRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RgoodTL.text = ((RgoodTLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RgoodBR.text = ((RgoodBRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RgoodBL.text = ((RgoodBLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadTTL.text = ((RbadTTLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadTLC.text = ((RbadTLCcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadTLL.text = ((RbadTLLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadBLL.text = ((RbadBLLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadBLC.text = ((RbadBLCcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadBBL.text = ((RbadBBLcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadTTR.text = ((RbadTTRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadTRC.text = ((RbadTRCcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadTRR.text = ((RbadTRRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadBRR.text = ((RbadBRRcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadBRC.text = ((RbadBRCcnt / totalframes) * 100).ToString("F0") + "%";
        newTemplate.RbadBBR.text = ((RbadBBRcnt / totalframes) * 100).ToString("F0") + "%";
}
}
