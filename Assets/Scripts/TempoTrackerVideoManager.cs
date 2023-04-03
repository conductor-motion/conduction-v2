using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.UI;

public class TempoTrackerVideoManager : MonoBehaviour, IDragHandler, IPointerDownHandler
{

    public GameObject WebCamInput;
    VideoPlayer webCamInput; 
    public Image progress;

   // public VideoCapture videoCapture;

    public Text currMins;
    public Text currSecs;
    public Text totalMins;
    public Text totalSecs;

    void Awake() {
        WebCamInput = GameObject.Find("WebCamInput");
        if(WebCamInput) {
           webCamInput = WebCamInput.gameObject.GetComponent<VideoPlayer>();

            if(webCamInput.isLooping) {
                webCamInput.isLooping = false;
            }

            if(webCamInput.isPlaying) {
                webCamInput.Pause();
                webCamInput.time = 0.0f;
            }

        } 



    }
    
    void Update()
    {
        if(webCamInput.frameCount > 0)
            progress.fillAmount = (float)webCamInput.frame / (float)webCamInput.frameCount;
        if(webCamInput.isPlaying)
            SetCurrentTimeUI();
        SetTotalTimeUI();
    }
    public void OnDrag(PointerEventData eventData)
    {
        TrySkip(eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        TrySkip(eventData);
    }
    private void SkipToPercent(float pct)
    {
        var frame = webCamInput.frameCount * pct;
        webCamInput.frame = (long)frame;
    }
    private void TrySkip(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            progress.rectTransform, eventData.position, Camera.main, out localPoint))
        {
            float pct = Mathf.InverseLerp(progress.rectTransform.rect.xMin, progress.rectTransform.rect.xMax, localPoint.x);
            SkipToPercent(pct);
        }
    }
    public void VideoPlayerPause()
    {
        if(webCamInput != null)
            webCamInput.Pause();
    }
    public void VideoPlayerPlay()
    {
        if(webCamInput != null)
            webCamInput.Play();  
    }

    void SetCurrentTimeUI() {
        string mins = Mathf.Floor((int) webCamInput.time / 60).ToString("00");
        string secs = ((int)webCamInput.time % 60).ToString("00");

        currMins.text = mins;
        currSecs.text = secs;
    }

    void SetTotalTimeUI() {
        string mins = Mathf.Floor((int) webCamInput.length / 60).ToString("00");
        string secs = ((int)webCamInput.length % 60).ToString("00");

        totalMins.text = mins;
        totalSecs.text = secs;
    }

  
}

