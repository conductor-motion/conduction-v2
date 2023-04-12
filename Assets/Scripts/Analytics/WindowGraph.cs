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
    //reference to tooltip game object
    TooltipScript tooltipScript;
    [SerializeField] GameObject tooltip;
    //reference to sprites used for points
    [SerializeField] private Sprite BlackDotSprite;
    [SerializeField] private Sprite WhiteDotSprite;
    //reference to error pop up panel
    public GameObject ErrorPopUpPanel;

    SpriteRenderer axisComponent;
    GameObject axis;

    private List<float> yVals = new List<float>();
    
    private void Awake() {
       graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();
       Xtemp = graphContainer.Find("Xtemp").GetComponent<RectTransform>();
       Ytemp = graphContainer.Find("Ytemp").GetComponent<RectTransform>();
       DashXTemp = graphContainer.Find("DashXTemp").GetComponent<RectTransform>();
       DashYTemp = graphContainer.Find("DashYTemp").GetComponent<RectTransform>();
       
       tooltipScript = GameObject.Find("Tooltip").GetComponent<TooltipScript>();
       tooltipScript = tooltip.GetComponent<TooltipScript>();

       axis = GameObject.FindWithTag("Axis");
       axisComponent = axis.GetComponent<SpriteRenderer>();
       axisComponent.enabled = false; 

       displayGraph();
    }
    //removes axis lines that show in build mode
     void OnDestroy()
    {
        axisComponent.enabled = true;
    }

    //Gets the duration of the video using the frame count and creates labels for the x-axis
    public string GetTimeValues(int totalCount) {
        int counter = 1;
        for(int i=1; i<yVals.Count; i++) {
            counter++;
        }
        
        int durationSeconds = (counter/16);
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
    
    //Parses data.json to get the yVals
    private void ParseData(string s) {
        JSONNode n = JSON.Parse(s);
        n = n["frames"];

        for(int i=0; i<n.Count; i++) {
            yVals.Add(n[i]["data"][1]["yVal"]);
        }
    }

    /*Calculates tempo by counting the downbeat every 2 seconds(32 frames because 15-16fps)
    and multiply by 30 to get the BPM to store in the tempo list*/
    private List<int> tempoTracker() {
        int downbeat = 0;
        int counter = 1;
        int tempoCalc = 0;
        List<int> tempo = new List<int>();

        for(int i=1; i<yVals.Count-16; i++) {
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

    //Is called if user ticked the metronome in the recording scene
    void createMetronomeLine(List<int> tempo, float x_size, float yMax, float graphHeight, string[] arr, Func<float, string> findYAxisLabel = null) {
         GameObject prevDotGameObjMetronome = null;
         
         if(findYAxisLabel == null) {
            findYAxisLabel = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
        }
        
        for(int i= 0; i<tempo.Count; i++) {
            float x_pos = i*x_size + x_size;
            float y_pos = (MainManager.Instance.tempoBeat/yMax)*graphHeight;
            
            GameObject dotGameObj = createPoint(new Vector2(x_pos, y_pos), 1);

            string tooltip_x = arr[i];
            string tooltip_y = findYAxisLabel( MainManager.Instance.tempoBeat);
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
    }

    
    private void displayGraph(Func<float, string> findYAxisLabel = null) {
        float x_size = 50f;
        float graphHeight = graphContainer.sizeDelta.y;
        List<int> tempo = new List<int>();
        int counter = 1;

       //store reference to prev game obj
       GameObject prevDotGameObjConductor = null;
       //Get data.json file from MainManager
       FileInfo fileInfo = new FileInfo(MainManager.Instance.dirPath);
       string inputData = fileInfo.DirectoryName;
       inputData = Path.Combine(inputData, "data.json");


        string json = File.ReadAllText(inputData);

        ParseData(json);
        for(int i=1; i<yVals.Count; i++) {
            counter++;
        }
        //If video is less than 2 seconds, tempo tracker will not work
        if(counter < 32) {
            Debug.Log("counter: " + counter);
            ErrorPopUpPanel.SetActive(true);
        }

        else {
            tempo = tempoTracker();

            for(int i=0; i<tempo.Count; i++) {
                Debug.Log(tempo[i]);
            }


            float yMax = tempo[0];
            float yMin = tempo[0];

            
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

            if(yMax <  MainManager.Instance.tempoBeat) {
                yMax =  MainManager.Instance.tempoBeat;
            }

            if(yMin <  MainManager.Instance.tempoBeat) {
                yMin =  MainManager.Instance.tempoBeat;
            }
            
            string s = GetTimeValues(tempo.Count);
            string[] arr = s.Split(' ');

            if( !(arr[arr.Length-1].Equals("3:00")) ) {
                //if video is at 3 mins, the graph will go off screen
                //shrink graph?
                //a performance can be up to 20 minutes?
                if(arr[arr.Length-1].Equals("3:02") || 
                arr[arr.Length-1].Equals("3:04") ||
                arr[arr.Length-1].Equals("3:08") ||
                arr[arr.Length-1].Equals("3:10")) {
                        x_size -= 5f;
                    }

                if(arr[arr.Length-1].Equals("3:12") ||
                arr[arr.Length-1].Equals("3:14") ||
                arr[arr.Length-1].Equals("3:16") ||
                arr[arr.Length-1].Equals("3:18")) {
                        x_size -=10f;
                    }

                if(arr[arr.Length-1].Equals("3:20") ||
                arr[arr.Length-1].Equals("3:22") ||
                arr[arr.Length-1].Equals("3:24") || 
                arr[arr.Length-1].Equals("3:26")) {
                        x_size -= 15f;
                    }
                
                if(arr[arr.Length-1].Equals("3:28") || 
                arr[arr.Length-1].Equals("3:30") || 
                arr[arr.Length-1].Equals("3:32") ||
                arr[arr.Length-1].Equals("3:34")) {
                        x_size -= 20f;
                    }

                if(arr[arr.Length-1].Equals("3:36") ||
                arr[arr.Length-1].Equals("3:38") ||
                arr[arr.Length-1].Equals("3:40") ||
                arr[arr.Length-1].Equals("3:42")) {
                    x_size -= 25f;
                }

                if(arr[arr.Length-1].Equals("3:44") ||
                arr[arr.Length-1].Equals("3:46") ||
                arr[arr.Length-1].Equals("3:48") ||
                arr[arr.Length-1].Equals("3:50")) {
                    x_size -= 30f;
                }

                if(arr[arr.Length-1].Equals("3:52") ||
                arr[arr.Length-1].Equals("3:54") ||
                arr[arr.Length-1].Equals("3:56") ||
                arr[arr.Length-1].Equals("3:58")) {
                    x_size -= 35f;
                }

                if(arr[arr.Length-1].Equals("4:00") ||
                arr[arr.Length-1].Equals("4:02") ||
                arr[arr.Length-1].Equals("4:04") ||
                arr[arr.Length-1].Equals("4:06")) {
                    x_size -= 40f;
                }
                
                if(arr[arr.Length-1].Equals("4:08") ||
                arr[arr.Length-1].Equals("4:10") ||
                arr[arr.Length-1].Equals("4:12") ||
                arr[arr.Length-1].Equals("4:14")) {
                    x_size -= 45f;
                }
                
                if(arr[arr.Length-1].Equals("4:16") ||
                arr[arr.Length-1].Equals("4:18") ||
                arr[arr.Length-1].Equals("4:20") ||
                arr[arr.Length-1].Equals("4:22")) {
                    x_size -= 50f;
                }
                
                if(arr[arr.Length-1].Equals("4:24") ||
                arr[arr.Length-1].Equals("4:26") ||
                arr[arr.Length-1].Equals("4:28") ||
                arr[arr.Length-1].Equals("4:30")) {
                    x_size -= 55f;
                }
                
                if(arr[arr.Length-1].Equals("4:32") ||
                arr[arr.Length-1].Equals("4:34") ||
                arr[arr.Length-1].Equals("4:36") ||
                arr[arr.Length-1].Equals("4:38")) {
                    x_size -= 60f;
                }
                
                if(arr[arr.Length-1].Equals("4:40") ||
                arr[arr.Length-1].Equals("4:42") ||
                arr[arr.Length-1].Equals("4:44") ||
                arr[arr.Length-1].Equals("4:46")) {
                    x_size -= 65f;
                }
                
                if(arr[arr.Length-1].Equals("4:48") ||
                arr[arr.Length-1].Equals("4:50") ||
                arr[arr.Length-1].Equals("4:52") ||
                arr[arr.Length-1].Equals("4:54")) {
                    x_size -= 70f;
                }
                
                if(arr[arr.Length-1].Equals("4:56") ||
                arr[arr.Length-1].Equals("4:58") ||
                arr[arr.Length-1].Equals("5:00") ||
                arr[arr.Length-1].Equals("5:02")) {
                    x_size -= 75f;
                }
                
                if(arr[arr.Length-1].Equals("5:04") ||
                arr[arr.Length-1].Equals("5:06") ||
                arr[arr.Length-1].Equals("5:08") ||
                arr[arr.Length-1].Equals("5:10")) {
                    x_size -= 80f;
                }

                if(arr[arr.Length-1].Equals("5:12") ||
                arr[arr.Length-1].Equals("5:14") ||
                arr[arr.Length-1].Equals("5:16") ||
                arr[arr.Length-1].Equals("5:18")) {
                    x_size -= 85f;
                }
                
                if(arr[arr.Length-1].Equals("5:20") ||
                arr[arr.Length-1].Equals("5:22") ||
                arr[arr.Length-1].Equals("5:24") ||
                arr[arr.Length-1].Equals("5:26")) {
                    x_size -= 90f;
                }
                
                if(arr[arr.Length-1].Equals("5:28") ||
                arr[arr.Length-1].Equals("5:30") ||
                arr[arr.Length-1].Equals("5:32") ||
                arr[arr.Length-1].Equals("5:34")) {
                    x_size -= 95f;
                }
                
                if(arr[arr.Length-1].Equals("5:36") ||
                arr[arr.Length-1].Equals("5:38") ||
                arr[arr.Length-1].Equals("5:40") ||
                arr[arr.Length-1].Equals("5:42")) {
                    x_size -= 100f;
                }
                
                if(arr[arr.Length-1].Equals("5:44") ||
                arr[arr.Length-1].Equals("5:48") ||
                arr[arr.Length-1].Equals("5:50") ||
                arr[arr.Length-1].Equals("5:52")) {
                    x_size -= 105f;
                }
            
            //max was 10 mins
            }

            //Metronome line
            if(MainManager.Instance.metronomePlay) {
                createMetronomeLine(tempo, x_size, yMax, graphHeight, arr, findYAxisLabel = null);
            }
            

            if(findYAxisLabel == null) {
                findYAxisLabel = delegate (float _f) { return Mathf.RoundToInt(_f).ToString(); };
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

    }

    //Creates a black or white point for the conductor or metronome line
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
    
    //Connects each point to one another
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
