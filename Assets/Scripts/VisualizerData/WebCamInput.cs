using System.Text;
using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;

//Based off of BlazePose
public class WebCamInput : MonoBehaviour
{
    [SerializeField] string webCamName;
    [SerializeField] Vector2 webCamResolution = new Vector2(1920, 1080);
    public Texture staticInput;


    // Provide input image Texture.
    public Texture inputImageTexture
    {
        get
        {
            if (staticInput != null) return staticInput;
            return inputRT;
        }
    }

    WebCamTexture webCamTexture;
    RenderTexture inputRT;

    void Start()
    {
        //If there is no video provided
        if (staticInput == null)
        {
            //grab webcam name selected in main menu
            webCamName = MainManager.Instance.webCamName; 
            webCamTexture = new WebCamTexture(webCamName, (int)webCamResolution.x, (int)webCamResolution.y);
            webCamTexture.Play();
        }

        inputRT = new RenderTexture((int)webCamResolution.x, (int)webCamResolution.y, 0);

       
    }

    void Update()
    {

        if (staticInput != null) return;
        if (!webCamTexture.didUpdateThisFrame) return;

        var aspect1 = (float)webCamTexture.width / webCamTexture.height;
        var aspect2 = (float)inputRT.width / inputRT.height;
        var aspectGap = aspect2 / aspect1;

        var vMirrored = webCamTexture.videoVerticallyMirrored;
        var scale = new Vector2(aspectGap, vMirrored ? -1 : 1);
        var offset = new Vector2((1 - aspectGap) / 2, vMirrored ? 1 : 0);

        Graphics.Blit(webCamTexture, inputRT, scale, offset);


    }

    //Change volume of video
    public void OnValueChanged(float value)
    {
        UnityEngine.Video.VideoPlayer videoPlayer = GetComponent<UnityEngine.Video.VideoPlayer>();
        videoPlayer.SetDirectAudioVolume(0, value);
    }

    void OnDestroy()
    {
        if (webCamTexture != null) Destroy(webCamTexture);
        if (inputRT != null) Destroy(inputRT);
    }
}
