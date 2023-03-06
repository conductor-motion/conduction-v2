using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System.IO;
using System.Linq;
using SimpleJSON;

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

    
    //maybe remove static
    private static List<float> yVals = new List<float>();
   

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
       
    
    
       displayGraph((int _i) => "0:" + (_i+5));

    }

    
 
    public static string GetTimeValues(int totalCount) {
        int counter = 0;
        for(int i=1; i<yVals.Count; i++) {
            counter++;
        }
        //number between 29 and 29.9
        int durationSeconds = (int)(Math.Ceiling(counter/29.75));//24;
        //Debug.Log(durationSeconds);
        
        string timeString = "";
        for (int seconds = 5; seconds <= durationSeconds; seconds += 5) {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;
            string time = minutes + ":" + remainingSeconds.ToString("D2");
            if (timeString.Length > 0) {
               // timeString += ", ";
               timeString += " ";
            } 
            timeString += time;
        }
        
       Debug.Log(timeString);
       return timeString; 
    }





   
    private static FileInfo GetNewestFile(DirectoryInfo directory)
        {
            return directory.GetFiles()
                .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }

    private void ParseData(string s) {
        JSONNode n = JSON.Parse(s);
        n = n["frames"];

        for(int i=0; i<n.Count; i++) {
            for(int j=0; j<n[j].Count; j++) {
                yVals.Add(n[i]["data"][j]["yVal"]);
            }
        }
    }

    private List<int> tempoTracker() {
        int downbeat = 0;
        int counter = 1;
        int tempoCalc = 0;
        List<int> tempo = new List<int>();

        for(int i=1; i<yVals.Count; i++) {
            if(yVals[i] < yVals[i-1]) {
                if(i != yVals.Count-1)
                    if(yVals[i] < yVals[i+1]) {
                        downbeat++;
                    }
            }
            if(counter%150 == 0) {
                tempoCalc = (downbeat*12);
                tempo.Add(tempoCalc);
                downbeat = 0;
            }
            counter++;
        }
        
        return tempo;
    }

    //to display graph 
    //would probably have to change parameters later
    private void displayGraph(Func<int, string> findXAxisLabel = null, Func<float, string> findYAxisLabel = null) {
        float x_size = 50f;
       
        List<int> tempo = new List<int>();


        float graphHeight = graphContainer.sizeDelta.y;

        //store reference to prev game obj
        GameObject prevDotGameObj = null;

        GameObject prevEx = null;

        FileInfo newestFile = GetNewestFile(new DirectoryInfo(@"Assets/Conduction/Data"));
        //Debug.Log(newestFile.Name);
        string json = File.ReadAllText(newestFile.FullName);
        //Debug.Log(json);

        ParseData(json);

        /*for(int i=0; i<yVals.Count; i++) {
            Debug.Log(yVals[i]);
        }*/

        tempo = tempoTracker();

        for(int i=0; i<tempo.Count; i++) {
            Debug.Log(tempo[i]);
        }

        float yMax = tempo[0];
        float yMin = tempo[0];

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

        foreach(int val in tempo) {
            if(val > yMax) {
                yMax = val;
            }
            if(val < yMin) {
                yMin = val;
            }
            
        }

        if(yMax < metronomeStorage.GetTempo()) {
            yMax = metronomeStorage.GetTempo();
        }

        if(yMin < metronomeStorage.GetTempo()) {
            yMin = metronomeStorage.GetTempo();
        }
        
        string s = GetTimeValues(tempo.Count);
        string[] arr = s.Split(' ');

        if( !(arr[arr.Length-1].Equals("3:00")) && !(arr[arr.Length-1].Equals("2:55")) ) {
            //if video is at 3 mins, the graph will go off screen
            //shrink graph?
            //a performance can be up to 20 minutes?
            if(arr[arr.Length-1].Equals("3:55")  || arr[arr.Length-1].Equals("4:00") || arr[arr.Length-1].Equals("3:55") || arr[arr.Length-1].Equals("4:05") || arr[arr.Length-1].Equals("4:10") || arr[arr.Length-1].Equals("4:15")) {
                x_size -= 13f;
            }

            if(arr[arr.Length-1].Equals("5:00")) {
                x_size -= -12f;
            }
            //x_size -= 20f;
            //if vid is at 6 mins? or every three mins? keep shrinking
        }

        //Metronome graph
        //loop till however many y values

        for(int i= 0; i<tempo.Count; i++) {
            float x_pos = i*x_size + x_size;
            float y_pos = (metronomeStorage.GetTempo()/yMax)*graphHeight;
            //change dot color for metronome
            GameObject dotGameObj = createPoint(new Vector2(x_pos, y_pos));

            string tooltip_x = arr[i];
            string tooltip_y = findYAxisLabel(metronomeStorage.GetTempo());
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


            
            

        for(int i=0; i<tempo.Count; i++) {
            //find pos for both x and y values 

            //X-axis - Minutes
            //distance b/w each point on x axis
            float x_pos = i*x_size + x_size;

            //Y-axis - BPM
            //temp val is only being used to calculate the y pos
            float y_pos = (/*(*/tempo[i] /*- yMin)*/ / /*(*/yMax /*- yMin)*/)*graphHeight;
            //position in the graph it'll be in
            GameObject dotGameObj = createPoint(new Vector2(x_pos, y_pos));
            //Debug.Log("x position: " + x_pos + "\ny position: " + y_pos);

            string tooltip_x = arr[i];
            string tooltip_y = findYAxisLabel(tempo[i]);
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
            X_label.GetComponent<Text>().text = arr[i];
           
            //X_label.GetComponent<Text>().text = //GetTimeValues(tempo.Count); //findXAxisLabel(i);

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
