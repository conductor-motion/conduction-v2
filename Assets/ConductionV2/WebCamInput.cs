using System.Text;
using UnityEditor.Media;
using UnityEngine;
using System.IO;

public class WebCamInput : MonoBehaviour
{
    [SerializeField] string webCamName;
    [SerializeField] Vector2 webCamResolution = new Vector2(1920, 1080);
    [SerializeField] Texture staticInput;

    MediaEncoder encoder;
    VideoTrackAttributes videoAttr;
    AudioTrackAttributes audioAttr;

    string fullPath;
    bool didRecord = false;

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
        if (staticInput == null)
        {
            webCamTexture = new WebCamTexture(webCamName, (int)webCamResolution.x, (int)webCamResolution.y);
            webCamTexture.Play();
        }

        inputRT = new RenderTexture((int)webCamResolution.x, (int)webCamResolution.y, 0);


        videoAttr = new VideoTrackAttributes
        {
            frameRate = new MediaRational(50),
            width = (uint)inputRT.width,
            height = (uint)inputRT.height,
            includeAlpha = false
        };

        audioAttr = new AudioTrackAttributes
        {
            sampleRate = new MediaRational(48000),
            channelCount = 2,
            language = "fr"
        };
        string dateString = System.DateTime.Now.ToString("s").Replace(":", "-");
        fullPath = Application.dataPath + "/Conduction/Data/" + dateString.Remove(dateString.Length-3);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
        encoder = new MediaEncoder(fullPath+"/video.mp4", videoAttr, audioAttr);
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

        if (MocapRecorderOurs.isRecording)
        {
            Texture2D tex = new Texture2D(webCamTexture.width, webCamTexture.height);
            tex.SetPixels(webCamTexture.GetPixels());

            encoder.AddFrame(tex);
            didRecord = true;
        }
    }

    void OnDestroy()
    {
        if (webCamTexture != null) Destroy(webCamTexture);
        if (inputRT != null) Destroy(inputRT);

        encoder.Dispose();

        if(!didRecord)
        {
            File.Delete(fullPath + "/video.mp4");
            try
            {
                Directory.Delete(fullPath);
            }
            catch { }
        }
    }
}
