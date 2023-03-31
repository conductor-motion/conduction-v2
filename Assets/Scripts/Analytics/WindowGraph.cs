using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UITtooltip.Utils;

public class WindowGraph : MonoBehaviour
{
    //reference to graph container
    private RectTransform graphContainer;
    //reference to X and Y labels
    private RectTransform Xtemp;
    private RectTransform Ytemp;
    //reference to X and Y axis lines
    private RectTransform DashXTemp;
    private RectTransform DashYTemp;

    TooltipScript tooltipScript;
    [SerializeField] GameObject tooltip;

    public GameObject metronome;
    MetronomeStorage metronomeStorage;

    [SerializeField] private Sprite BlackDotSprite;
    [SerializeField] private Sprite WhiteDotSprite;

    public static bool graphCounter = true;

    private static List<float> yVals = new List<float>();
   
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
       
      // if(graphCounter) {
        displayGraph();
       // graphCounter = false;
      // }

      /* else {
        if(!graphCounter) {
            displayGraph();
        }
       }
      */
       
       
    }

    public static string GetTimeValues(int totalCount) {
        int counter = 1;
        for(int i=1; i<yVals.Count; i++) {
            counter++;
        }
        
        int durationSeconds = /*(int)*/(/*Math.*/(counter/16));
        Debug.Log("duration seconds:" + durationSeconds);
        
        string timeString = "";
        for (int seconds = 2; seconds <= durationSeconds; seconds += 2) {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;
            string time = minutes + ":" + remainingSeconds.ToString("D2");
            if (timeString.Length > 0) {
               timeString += " ";
            } 
            timeString += time;
        }
        
       Debug.Log(timeString);
       return timeString; 
    }
   
    private static FileInfo GetNewestFile(DirectoryInfo directory) {
            return directory.GetFiles()
                .Union(directory.GetDirectories().Select(d => GetNewestFile(d)))
                .OrderByDescending(f => (f == null ? DateTime.MinValue : f.LastWriteTime))
                .FirstOrDefault();
        }

    private void ParseData(string s) {
        JSONNode n = JSON.Parse(s);
        n = n["frames"];

        for(int i=0; i<n.Count; i++) {
            yVals.Add(n[i]["data"][1]["yVal"]);
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
            if(counter%32 == 0) {
                tempoCalc = downbeat*30;
                tempo.Add(tempoCalc);
                downbeat = 0;
            }
            counter++;
        }
        
        return tempo;
    }

    
    private void displayGraph(Func<float, string> findYAxisLabel = null) {
        float x_size = 50f;
        float graphHeight = graphContainer.sizeDelta.y;
        List<int> tempo = new List<int>();

        //store reference to prev game obj
        GameObject prevDotGameObjConductor = null;
        GameObject prevDotGameObjMetronome = null;

        FileInfo currentFile = new FileInfo(MainManager.Instance.dirPath.Substring(0, MainManager.Instance.dirPath.LastIndexOf("/")) + "/data.json");
        //Debug.Log(newestFile.Name);
        string json = File.ReadAllText(currentFile.FullName);
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

        if( !(arr[arr.Length-1].Equals("3:00")) ) {
            //if video is at 3 mins, the graph will go off screen
            //shrink graph?
            //a performance can be up to 20 minutes?
            if(arr[arr.Length-1].Equals("3:05") || 
               arr[arr.Length-1].Equals("3:10") ||
               arr[arr.Length-1].Equals("3:15") ||
               arr[arr.Length-1].Equals("3:20")) {
                    x_size -= 5f;
                }

            if(arr[arr.Length-1].Equals("3:25") ||
               arr[arr.Length-1].Equals("3:30") ||
               arr[arr.Length-1].Equals("3:35") ||
               arr[arr.Length-1].Equals("3:40")) {
                    x_size -=10f;
                }

            if(arr[arr.Length-1].Equals("3:45") ||
               arr[arr.Length-1].Equals("3:50") ||
               arr[arr.Length-1].Equals("3:55") || 
               arr[arr.Length-1].Equals("4:00")) {
                    x_size -= 15f;
                }
            
            if(arr[arr.Length-1].Equals("4:05") || 
               arr[arr.Length-1].Equals("4:10") || 
               arr[arr.Length-1].Equals("4:15") ||
               arr[arr.Length-1].Equals("4:20")) {
                    x_size -= 20f;
                }

            if(arr[arr.Length-1].Equals("4:25") ||
               arr[arr.Length-1].Equals("4:30") ||
               arr[arr.Length-1].Equals("4:35") ||
               arr[arr.Length-1].Equals("4:40")) {
                x_size -= 25f;
               }

            if(arr[arr.Length-1].Equals("4:45") ||
               arr[arr.Length-1].Equals("4:50") ||
               arr[arr.Length-1].Equals("4:55") ||
               arr[arr.Length-1].Equals("5:00")) {
                x_size -= 30f;
               }

            if(arr[arr.Length-1].Equals("5:05") ||
               arr[arr.Length-1].Equals("5:10") ||
               arr[arr.Length-1].Equals("5:15") ||
               arr[arr.Length-1].Equals("5:20")) {
                x_size -= 35f;
              }

            if(arr[arr.Length-1].Equals("5:25") ||
               arr[arr.Length-1].Equals("5:30") ||
               arr[arr.Length-1].Equals("5:35") ||
               arr[arr.Length-1].Equals("5:40")) {
                x_size -= 40f;
               }
            
            if(arr[arr.Length-1].Equals("5:45") ||
               arr[arr.Length-1].Equals("5:50") ||
               arr[arr.Length-1].Equals("5:55") ||
               arr[arr.Length-1].Equals("6:00")) {
                x_size -= 45f;
               }
            
            if(arr[arr.Length-1].Equals("6:05") ||
               arr[arr.Length-1].Equals("6:10") ||
               arr[arr.Length-1].Equals("6:15") ||
               arr[arr.Length-1].Equals("6:20")) {
                x_size -= 50f;
               }
            
            if(arr[arr.Length-1].Equals("6:25") ||
               arr[arr.Length-1].Equals("6:30") ||
               arr[arr.Length-1].Equals("6:35") ||
               arr[arr.Length-1].Equals("6:40")) {
                x_size -= 55f;
               }
            
            if(arr[arr.Length-1].Equals("6:45") ||
               arr[arr.Length-1].Equals("6:50") ||
               arr[arr.Length-1].Equals("6:55") ||
               arr[arr.Length-1].Equals("7:00")) {
                x_size -= 60f;
               }
            
            if(arr[arr.Length-1].Equals("7:05") ||
               arr[arr.Length-1].Equals("7:10") ||
               arr[arr.Length-1].Equals("7:15") ||
               arr[arr.Length-1].Equals("7:20")) {
                x_size -= 65f;
               }
            
            if(arr[arr.Length-1].Equals("7:25") ||
               arr[arr.Length-1].Equals("7:30") ||
               arr[arr.Length-1].Equals("7:35") ||
               arr[arr.Length-1].Equals("7:40")) {
                x_size -= 70f;
               }
            
            if(arr[arr.Length-1].Equals("7:45") ||
               arr[arr.Length-1].Equals("7:50") ||
               arr[arr.Length-1].Equals("7:55") ||
               arr[arr.Length-1].Equals("8:00")) {
                x_size -= 75f;
               }
            
            if(arr[arr.Length-1].Equals("8:05") ||
               arr[arr.Length-1].Equals("8:10") ||
               arr[arr.Length-1].Equals("8:15") ||
               arr[arr.Length-1].Equals("8:20")) {
                x_size -= 80f;
               }

            if(arr[arr.Length-1].Equals("8:25") ||
               arr[arr.Length-1].Equals("8:30") ||
               arr[arr.Length-1].Equals("8:35") ||
               arr[arr.Length-1].Equals("8:40")) {
                x_size -= 85f;
               }
            
            if(arr[arr.Length-1].Equals("8:45") ||
               arr[arr.Length-1].Equals("8:50") ||
               arr[arr.Length-1].Equals("8:55") ||
               arr[arr.Length-1].Equals("9:00")) {
                x_size -= 90f;
               }
            
            if(arr[arr.Length-1].Equals("9:05") ||
               arr[arr.Length-1].Equals("9:10") ||
               arr[arr.Length-1].Equals("9:15") ||
               arr[arr.Length-1].Equals("9:20")) {
                x_size -= 95f;
               }
            
            if(arr[arr.Length-1].Equals("9:25") ||
               arr[arr.Length-1].Equals("9:30") ||
               arr[arr.Length-1].Equals("9:35") ||
               arr[arr.Length-1].Equals("9:40")) {
                x_size -= 100f;
               }
            
            if(arr[arr.Length-1].Equals("9:45") ||
               arr[arr.Length-1].Equals("9:50") ||
               arr[arr.Length-1].Equals("9:55") ||
               arr[arr.Length-1].Equals("10:00")) {
                x_size -= 105f;
               }
           
           
        }

        //Metronome line
        for(int i= 0; i<tempo.Count; i++) {
            float x_pos = i*x_size + x_size;
            float y_pos = (metronomeStorage.GetTempo()/yMax)*graphHeight;
            
            GameObject dotGameObj = createPoint(new Vector2(x_pos, y_pos), 1);

            string tooltip_x = arr[i];
            string tooltip_y = findYAxisLabel(metronomeStorage.GetTempo());
            string tooltip_text = "(" + tooltip_x + "," + tooltip_y + ")";

           TooltipUI dotButtonUI = dotGameObj.AddComponent<TooltipUI>();

            dotButtonUI.MouseOverOnce += () => {
                TooltipScript.static_displayTooltip(tooltip_text);
            };

            dotButtonUI.MouseOutOnce += () => {
                TooltipScript.static_hideTooltip();
            };

            if(prevDotGameObjMetronome != null) {
                drawDotLine(prevDotGameObjMetronome.GetComponent<RectTransform>().anchoredPosition, dotGameObj.GetComponent<RectTransform>().anchoredPosition, new Color(1,1,1, .5f));
            }
            prevDotGameObjMetronome = dotGameObj;

        } 

        //Conductor line
        for(int i=0; i<tempo.Count; i++) {
            float x_pos = i*x_size + x_size;
            float y_pos = (tempo[i] / yMax)*graphHeight;
            
            GameObject dotGameObj = createPoint(new Vector2(x_pos, y_pos));

            string tooltip_x = arr[i];
            string tooltip_y = findYAxisLabel(tempo[i]);
            string tooltip_text = "(" + tooltip_x + "," + tooltip_y + ")";

           TooltipUI dotButtonUI = dotGameObj.AddComponent<TooltipUI>();

            dotButtonUI.MouseOverOnce += () => {
                TooltipScript.static_displayTooltip(tooltip_text);
            };

            dotButtonUI.MouseOutOnce += () => {
                TooltipScript.static_hideTooltip();
            };

            //if it's not the first dot
            if(prevDotGameObjConductor != null) {
                drawDotLine(prevDotGameObjConductor.GetComponent<RectTransform>().anchoredPosition, dotGameObj.GetComponent<RectTransform>().anchoredPosition, new Color(0,0,0, .5f));
            }
            prevDotGameObjConductor = dotGameObj;

           
            //graph height = x_size*.9f?
            //graph width = xsize

            //X axis separator
            RectTransform X_label = Instantiate(Xtemp);
            X_label.SetParent(graphContainer, false);
            X_label.gameObject.SetActive(true);
            X_label.anchoredPosition = new Vector2(x_pos, -20f);
            X_label.GetComponent<Text>().text = arr[i];

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

    private GameObject createPoint(Vector2 anchoredPosition, int dotColor = 0) {
        GameObject gameObject = new GameObject("point", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);

        if(dotColor == 1) {
            gameObject.GetComponent<Image>().sprite = WhiteDotSprite;
        }
        else {
            gameObject.GetComponent<Image>().sprite = BlackDotSprite;
        }
        
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
        gameObject.GetComponent<Image>().color = c;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 direction = (dot2 - dot1).normalized;
        float dist = Vector2.Distance(dot1, dot2);
        //anchor to lower left
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        rectTransform.sizeDelta = new Vector2(dist, 3f);
        
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
