using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System.IO;
using System.Linq;

public class WindowGraph : MonoBehaviour
{
    //to display each dot on graph
    [SerializeField] private Sprite dot_sprite;
    //get reference to graph container
    private RectTransform graphContainer;
    //reference to X and Y labels
    private RectTransform Xtemp;
    private RectTransform Ytemp;
    //reference to X and Y axis lines
    private RectTransform DashXTemp;
    private RectTransform DashYTemp;

    TooltipScript tooltipScript;
    //this obj is private but still visible within the editor
    [SerializeField] GameObject tooltip;

    public GameObject metronome;
    MetronomeStorage metronomeStorage;

    //awake gets called first, start gets called after
    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
        Xtemp = graphContainer.Find("Xtemp").GetComponent<RectTransform>();
        Ytemp = graphContainer.Find("Ytemp").GetComponent<RectTransform>();
        DashXTemp = graphContainer.Find("DashXTemp").GetComponent<RectTransform>();
        DashYTemp = graphContainer.Find("DashYTemp").GetComponent<RectTransform>();
       
        tooltipScript = GameObject.Find("Tooltip").GetComponent<TooltipScript>();
        tooltipScript = tooltip.GetComponent<TooltipScript>();

       
        metronome = GameObject.FindWithTag("Metronome");
        if(metronome) {
            metronomeStorage = metronome.GetComponent<MetronomeStorage>();
            //Debug.Log("tempo:" + metronomeStorage.tempo);
        } 
        
        

        
        //fix metronome so that it takes the value set by the user

        
       
        //y-values (will be changed later)
        List<int> tempVal = new List<int>() {0, 1, 2, 3, 4, 100, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 100, 18, 19, 20, 21, 22, 23, 24, 100, 26, 27, 28, 29, 30, 31, 32};
        
        FileInfo newestFile = GetNewestFile(new DirectoryInfo(@"Assets/Conduction/Data"));
        if(newestFile.Exists) {Debug.Log(newestFile.Name);}
        
        displayGraph(tempVal, (int _i) => "0:" + (_i+30));

    }

    private static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            return directory.GetFiles()
                .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }

    //to display graph 
    //would probably have to change parameters later
    private void displayGraph(List<int> tempVal, Func<int, string> findXAxisLabel = null, Func<float, string> findYAxisLabel = null) {
        float x_size = 50f;
        float yMax = tempVal[0]; //250f;
        float yMin = tempVal[0];



        float graphHeight = graphContainer.sizeDelta.y;

        //store reference to prev game obj
        GameObject prevDotGameObj = null;

        GameObject prevEx = null;

        if(findXAxisLabel == null) {
            findXAxisLabel = delegate (int _i) { return _i.ToString(); };
        }

         if(findYAxisLabel == null) {
            findYAxisLabel = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }
        
         //increase by 20% of the diff b/w max and min
        yMax = yMax + ((yMax - yMin) * 0.2f);
        //same thing but decrease
        yMin = yMin - ((yMax - yMin) * 0.2f);

        foreach(int val in tempVal) {
            if(val > yMax) {
                yMax = val;
            }
            if(val < yMin) {
                yMin = val;
            }
            
        }

        if(yMax < metronomeStorage.tempo) {
            yMax = metronomeStorage.tempo;
        }

        if(yMin < metronomeStorage.tempo) {
            yMin = metronomeStorage.tempo;
        }
        


        //Metronome graph
        //loop till however many y values

        for(int i= 0; i<tempVal.Count; i++) {
            float x_pos = i*x_size + x_size;
            float y_pos = (metronomeStorage.tempo/yMax)*graphHeight;
            //change dot color for metronome
            GameObject dotGameObj = createPoint(new Vector2(x_pos, y_pos));

            string tooltip_x = findXAxisLabel(i);
            string tooltip_y = findYAxisLabel(metronomeStorage.tempo);
            string tooltip_text = "(" + tooltip_x + "," + tooltip_y + ")";

            Button_UI dotButtonUI = dotGameObj.AddComponent<Button_UI>();

            dotButtonUI.MouseOverOnceFunc += () => {
                TooltipScript.static_displayTooltip(tooltip_text);
            };

            dotButtonUI.MouseOutOnceFunc += () => {
                TooltipScript.static_hideTooltip();
            };

            if(prevEx != null) {
                drawDotLine(prevEx.GetComponent<RectTransform>().anchoredPosition, dotGameObj.GetComponent<RectTransform>().anchoredPosition, new Color(1,1,1, .5f));
            }
            prevEx = dotGameObj;

        } 




        for(int i=0; i<tempVal.Count; i++) {
            //find pos for both x and y values 

            //X-axis - Minutes
            //distance b/w each point on x axis
            float x_pos = i*x_size + x_size;

            //Y-axis - BPM
            //temp val is only being used to calculate the y pos
            float y_pos = (/*(*/tempVal[i] /*- yMin)*/ / /*(*/yMax /*- yMin)*/)*graphHeight;
            //position in the graph it'll be in
            GameObject dotGameObj = createPoint(new Vector2(x_pos, y_pos));
            //Debug.Log("x position: " + x_pos + "\ny position: " + y_pos);

            string tooltip_x = findXAxisLabel(i);
            string tooltip_y = findYAxisLabel(tempVal[i]);
            string tooltip_text = "(" + tooltip_x + "," + tooltip_y + ")";

            Button_UI dotButtonUI = dotGameObj.AddComponent<Button_UI>();

            dotButtonUI.MouseOverOnceFunc += () => {
                TooltipScript.static_displayTooltip(tooltip_text);
            };

            dotButtonUI.MouseOutOnceFunc += () => {
                TooltipScript.static_hideTooltip();
            };

            //if it's not the first dot
            if(prevDotGameObj != null) {
                drawDotLine(prevDotGameObj.GetComponent<RectTransform>().anchoredPosition, dotGameObj.GetComponent<RectTransform>().anchoredPosition, new Color(0,0,0, .5f));
            }
            prevDotGameObj = dotGameObj;

           
            //graph height = x_size*.9f?
            //graph width = xsize

            //X axis separator
            RectTransform X_label = Instantiate(Xtemp);
            X_label.SetParent(graphContainer, false);
            X_label.gameObject.SetActive(true);
            X_label.anchoredPosition = new Vector2(x_pos, -20f);
            //change label here
            X_label.GetComponent<Text>().text = findXAxisLabel(i);

            RectTransform X_dash = Instantiate(DashXTemp);
            X_dash.SetParent(graphContainer, false);
            X_dash.gameObject.SetActive(true);
            X_dash.anchoredPosition = new Vector2(x_pos, 3f);

        } 

        int separate_count = 10;
        for(int i=0; i<=separate_count; i++) {
            RectTransform Y_label = Instantiate(Ytemp);
            Y_label.SetParent(graphContainer, false);
            Y_label.gameObject.SetActive(true);
            float normalizedVal = i*1f/separate_count;
            Y_label.anchoredPosition = new Vector2(-10f, normalizedVal*graphHeight);
            Y_label.GetComponent<Text>().text = findYAxisLabel(normalizedVal*yMax);

            RectTransform Y_dash = Instantiate(DashYTemp);
            Y_dash.SetParent(graphContainer, false);
            Y_dash.gameObject.SetActive(true);
            Y_dash.anchoredPosition = new Vector2(20f, normalizedVal*graphHeight);
           
        }

      
    }

    //function to create each point
    private GameObject createPoint(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("point", typeof(Image));
        //transform is the position, rotation, and scale of the obj
        //setting worldPositionStays to false means the gameObj will be positioned next to its new parent in the same way if that gameObj was next to its parent
        //instead of a world position, the world of the point gameObj centers around the graphContainer
        gameObject.transform.SetParent(graphContainer, false);
        //accessing the GameObject
        gameObject.GetComponent<Image>().sprite = dot_sprite;
        
        
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        //anchored position from what's recieved in the function
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11,11);
        //anchor to lower left 
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);

        return gameObject;
    }

    private void drawDotLine(Vector2 dot1, Vector2 dot2, Color c) {
        GameObject gameObject = new GameObject("dotLine", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = /*new Color(0,0,0, .5f)*/ c;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        //when normalized, a vector keeps the same direction but its length/magnitude is 1.0
        Vector2 direction = (dot2 - dot1).normalized;
        float dist = Vector2.Distance(dot1, dot2);
        //anchor to lower left
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        rectTransform.sizeDelta = new Vector2(dist, 3f);
        //maybe change up code here
        rectTransform.anchoredPosition = dot1 + direction * dist * .5f;
        rectTransform.localEulerAngles = new Vector3(0,0, findAngle(direction));

    }

    private float findAngle(Vector3 direction) {
        direction = direction.normalized;
        float a = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        if(a < 0) {
            a += 360;
        }
        return a;
    }
}
