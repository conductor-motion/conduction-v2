using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.BlazePose;
using System.IO;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Xml.Linq;
using UnityEngine.UIElements;
using UnityEngine.Video;

//Based on blazepose
public class PoseVisuallizer3D : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] WebCamInput webCamInput;
    [SerializeField] RawImage inputImageUI;
    [SerializeField] Shader shader;
    [SerializeField, Range(0, 1)] float humanExistThreshold = 0.5f;

    Material material;
    BlazePoseDetecter detecter;

    private FileStream dataFile;
    private StreamWriter writer;
    string filePath;
    string dirPath;
    int frameIndex = 0;
    Frames frames = new Frames();
    List<HandMovementData> data = new List<HandMovementData>();
    public static bool showLines;

    Transform objToPickUp;

    // Hooked up to avatar upper arms
    public Transform leftBicep;
    public Transform rightBicep;
    // Hooked up to avatar lower arms
    public Transform leftForearm;
    public Transform rightForearm;

    // Lines count of body's topology.
    const int BODY_LINE_NUM = 35;
    // Pairs of vertex indices of the lines that make up body's topology.
    // Defined by the figure in https://google.github.io/mediapipe/solutions/pose.
    readonly List<Vector4> linePair = new List<Vector4>{
        new Vector4(0, 1), new Vector4(1, 2), new Vector4(2, 3), new Vector4(3, 7), new Vector4(0, 4),
        new Vector4(4, 5), new Vector4(5, 6), new Vector4(6, 8), new Vector4(9, 10), new Vector4(11, 12),
        new Vector4(11, 13), new Vector4(13, 15), new Vector4(15, 17), new Vector4(17, 19), new Vector4(19, 15),
        new Vector4(15, 21), new Vector4(12, 14), new Vector4(14, 16), new Vector4(16, 18), new Vector4(18, 20),
        new Vector4(20, 16), new Vector4(16, 22), new Vector4(11, 23), new Vector4(12, 24), new Vector4(23, 24),
        new Vector4(23, 25), new Vector4(25, 27), new Vector4(27, 29), new Vector4(29, 31), new Vector4(31, 27),
        new Vector4(24, 26), new Vector4(26, 28), new Vector4(28, 30), new Vector4(30, 32), new Vector4(32, 28)
    };


    void Awake()
    {
        showLines = false;
    }

    void Start()
    {
        material = new Material(shader);
        detecter = new BlazePoseDetecter();

        //Only create a data file if a new recording or new upload
        if (SceneManager.GetActiveScene().name == "RecordingPage" || MainManager.Instance.newUpload == true)
            CreateDataFile();
        mainCamera = Camera.main;
        webCamInput = FindObjectOfType<WebCamInput>();
        inputImageUI = GameObject.Find("RawImage").GetComponent<RawImage>();
    }

    void LateUpdate()
    {
        inputImageUI.texture = webCamInput.inputImageTexture;

        // Predict pose by neural network model.
        detecter.ProcessImage(webCamInput.inputImageTexture);
        FrameData frame = new FrameData(frameIndex);

        // Output landmark values(33 values) and the score whether human pose is visible (1 values).
        for (int i = 0; i < detecter.vertexCount + 1; i++)
        {
            /*
            0~32 index datas are pose world landmark.
            Check below Mediapipe document about relation between index and landmark position.
            https://google.github.io/mediapipe/solutions/pose#pose-landmark-model-blazepose-ghum-3d
            Each data factors are
            x, y and z: Real-world 3D coordinates in meters with the origin at the center between hips.
            w: The score of whether the world landmark position is visible ([0, 1]).
        
            33 index data is the score whether human pose is visible ([0, 1]).
            This data is (score, 0, 0, 0).
            */
            //Add the data to a class to be later transformed into json for analytics
            if (((RecordingController.isRecording && SceneManager.GetActiveScene().name == "RecordingPage") || MainManager.Instance.newUpload == true) && (i == 15 || i == 16))
            {
                frame.data.Add(new HandMovementData(i, detecter.GetPoseWorldLandmark(i).x, detecter.GetPoseWorldLandmark(i).y, detecter.GetPoseWorldLandmark(i).z, detecter.GetPoseWorldLandmark(i).w));
            }
        }
        if ((RecordingController.isRecording && SceneManager.GetActiveScene().name == "RecordingPage") || MainManager.Instance.newUpload == true)
        {
            frames.frames.Add(frame);
            frameIndex++;
        }

        Vector3 temp = new Vector3();
        Vector3 offset = new Vector3();

        // The following code moves the farms on the avatar with angle relative to blazepose landmarks.
        // Right bicep
        temp = detecter.GetPoseWorldLandmark(14) -
               detecter.GetPoseWorldLandmark(12);
        rightBicep.rotation = Quaternion.LookRotation(temp) * Quaternion.Euler(Vector3.right * 90);
        
        // Left bicep
        temp = detecter.GetPoseWorldLandmark(13) - 
               detecter.GetPoseWorldLandmark(11);
        leftBicep.rotation = Quaternion.LookRotation(temp) * Quaternion.Euler(Vector3.right * 90);
        
        // Right forearm
        temp = detecter.GetPoseWorldLandmark(16) - 
               detecter.GetPoseWorldLandmark(14);
        rightForearm.rotation = Quaternion.LookRotation(temp) * Quaternion.Euler(Vector3.right * 90);
        
        // Left forearm
        temp = detecter.GetPoseWorldLandmark(15) - 
               detecter.GetPoseWorldLandmark(13);
        leftForearm.rotation = Quaternion.LookRotation(temp) * Quaternion.Euler(Vector3.right * 90);
    }

    void OnRenderObject()
    {
        if (showLines) { 
            // Use predicted pose world landmark results on the ComputeBuffer (GPU) memory.
            material.SetBuffer("_worldVertices", detecter.worldLandmarkBuffer);
            // Set pose landmark counts.
            material.SetInt("_keypointCount", detecter.vertexCount);
            material.SetFloat("_humanExistThreshold", humanExistThreshold);
            material.SetVectorArray("_linePair", linePair);
            material.SetMatrix("_invViewMatrix", mainCamera.worldToCameraMatrix.inverse);

            // Draw 35 world body topology lines.
            material.SetPass(2);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, BODY_LINE_NUM);

            // Draw 33 world landmark points.
            material.SetPass(3);
            Graphics.DrawProceduralNow(MeshTopology.Triangles, 6, detecter.vertexCount);
        }
    }
    
    
    void OnDestroy()
    {
        detecter.Dispose();
        if (SceneManager.GetActiveScene().name == "RecordingPage" || MainManager.Instance.newUpload == true)
        {
            //Write json file
            string json = JsonUtility.ToJson(frames, true);
            writer.Write(json);
            MainManager.Instance.setNewUpload(false);
            writer.Close();

            //If no frames were recorded, delete the json file
            if (frames.length() == 0)
            {
                File.Delete(filePath);
                try
                {
                    Directory.Delete(dirPath);
                }
                catch { }
            }
        }
    }
    void CreateDataFile()
    {
        //To avoid recursively burying the file if there's already a directory saved in the main manager rather than the datetime string
        if (!MainManager.Instance.newUpload && MainManager.Instance.dirPath.Length <= 16)
        {
            dirPath = Directory.GetCurrentDirectory() + "/Data/" + MainManager.Instance.dirPath;
            MainManager.Instance.SetDirPath(dirPath);
        } else
        {
            dirPath = MainManager.Instance.dirPath.Substring(0, MainManager.Instance.dirPath.LastIndexOf("/"));
        }

        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);

        }
        //If there are two videos recorded within a minute
        //No handling for more than two videos because that is extremely unlikely
        else if (!MainManager.Instance.newUpload) 
        {
            dirPath += "-2";
            Directory.CreateDirectory(dirPath);
        }
        filePath = dirPath + "/data.json";
        writer = new StreamWriter(filePath, true);
        writer.AutoFlush = true; //Make sure all data is dumped from streamWriter before changing scenes
    }
    // Toggles BlazePose lines on and off.
    public void ToggleLines()
    {
        showLines = !showLines;
    }

}
