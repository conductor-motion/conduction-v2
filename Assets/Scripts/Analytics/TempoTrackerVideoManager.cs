using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using UnityEngine.UI;

public class TempoTrackerVideoManager : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    VideoPlayer videoPlayer;
    public Image progress;

    public Text currMins;
    public Text currSecs;
    public Text totalMins;
    public Text totalSecs;

    void Awake() {
        videoPlayer = GetComponent<VideoPlayer>();
        if(videoPlayer) {
            videoPlayer.url = MainManager.Instance.dirPath;
            
            if(videoPlayer.isLooping) {
                videoPlayer.isLooping = false;
            }

            if(videoPlayer.isPlaying) {
                videoPlayer.Pause();
                videoPlayer.time = 0.0f;
            }
        }


    }
    
    void Update()
    {
        if(videoPlayer.frameCount > 0)
            progress.fillAmount = (float)videoPlayer.frame / (float)videoPlayer.frameCount;
        if(videoPlayer.isPlaying)
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
        var frame = videoPlayer.frameCount * pct;
        videoPlayer.frame = (long)frame;
    } 
    //Video scrubbing
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
        if(videoPlayer != null)
            videoPlayer.Pause();
    }
    public void VideoPlayerPlay()
    {
        if(videoPlayer != null)
            videoPlayer.Play();  
    }
    //To display current time
    void SetCurrentTimeUI() 
    {
        string mins = Mathf.Floor((int)videoPlayer.time / 60).ToString("00");
        string secs = ((int)videoPlayer.time % 60).ToString("00");

        currMins.text = mins;
        currSecs.text = secs;
    }
    //To display the total time
    void SetTotalTimeUI() 
    {
        string mins = Mathf.Floor((int)videoPlayer.length / 60).ToString("00");
        string secs = ((int)videoPlayer.length % 60).ToString("00");

        totalMins.text = mins;
        totalSecs.text = secs;
    }

  
}

