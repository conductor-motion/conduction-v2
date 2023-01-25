using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mediapipe.BlazePose;

public class PoseVisuallizer3D1 : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] WebCamInput webCamInput;
    [SerializeField] RawImage inputImageUI;
    [SerializeField] Shader shader;
    [SerializeField, Range(0, 1)] float humanExistThreshold = 0.5f;

    Material material;
    BlazePoseDetecter detecter;
    
    Transform objToPickUp;
    public Animator animator;

    Vector3 rightHand;
    Vector3 leftHand;

    public Transform nose;
    public Transform leftEye;
    public Transform rightEye;
    public Transform torso;
    public Transform rightShoulder;
    public Transform leftShoulder;
    public Transform upperLeftArm;
    public Transform upperRightArm;
    public Transform lowerRightArm;
    public Transform lowerLeftArm;

    public Transform rightWrist;
    
    public Transform leftWrist;
    public Transform upperLeftLeg;
    public Transform upperRightLeg;
    public Transform lowerLeftLeg;
    public Transform lowerRightLeg;
    public Transform rightAnkle;
    public Transform leftAnkle;

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


    void Start(){
        material = new Material(shader);
        detecter = new BlazePoseDetecter();
        //animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        //float reach = animator.GetFloat("RightHandReach");
        // Vector3 temp = new Vector3();
        // temp.x = detecter.GetPoseWorldLandmark(16).x;
        // temp.y = detecter.GetPoseWorldLandmark(16).y;
        // temp.z = detecter.GetPoseWorldLandmark(16).z;
        // rightHand = temp;
        //
        // temp.x = detecter.GetPoseWorldLandmark(15).x;
        // temp.y = detecter.GetPoseWorldLandmark(15).y;
        // temp.z = detecter.GetPoseWorldLandmark(15).z;
        // leftHand = temp;
        //
        // animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        // animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        // animator.SetIKPosition(AvatarIKGoal.RightHand, rightHand);
        // animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHand);
    }

    void Update(){
        //mainCamera.transform.RotateAround(Vector3.zero, Vector3.up, 0.1f);
    }

    void LateUpdate(){
        inputImageUI.texture = webCamInput.inputImageTexture;

        // Predict pose by neural network model.
        detecter.ProcessImage(webCamInput.inputImageTexture);

        // Output landmark values(33 values) and the score whether human pose is visible (1 values).
        for(int i = 0; i < detecter.vertexCount + 1; i++){
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
            //Debug.LogFormat("{0}: {1}", i, detecter.GetPoseWorldLandmark(i)[0]);
        }
        
        Vector3 temp = new Vector3();
        // temp.x = detecter.GetPoseWorldLandmark(0).x;
        // temp.y = detecter.GetPoseWorldLandmark(0).y;
        // temp.z = detecter.GetPoseWorldLandmark(0).z;
        // nose.position = temp;
        //
        // temp.x = detecter.GetPoseWorldLandmark(2).x;
        // temp.y = detecter.GetPoseWorldLandmark(2).y;
        // temp.z = detecter.GetPoseWorldLandmark(2).z;
        // leftEye.position = temp;
        //
        // temp.x = detecter.GetPoseWorldLandmark(5).x;
        // temp.y = detecter.GetPoseWorldLandmark(5).y;
        // temp.z = detecter.GetPoseWorldLandmark(5).z;
        // rightEye.position = temp;

        // float x = 0;
        // x += detecter.GetPoseWorldLandmark(12).x;
        // x += detecter.GetPoseWorldLandmark(11).x;
        // x += detecter.GetPoseWorldLandmark(24).x;
        // x += detecter.GetPoseWorldLandmark(23).x;
        // x /= 4;
        //
        // float y = 0;
        // y += detecter.GetPoseWorldLandmark(12).y;
        // y += detecter.GetPoseWorldLandmark(11).y;
        // y += detecter.GetPoseWorldLandmark(24).y;
        // y += detecter.GetPoseWorldLandmark(23).y;
        // y /= 4;
        //
        // float z = 0;
        // z += detecter.GetPoseWorldLandmark(12).z;
        // z += detecter.GetPoseWorldLandmark(11).z;
        // z += detecter.GetPoseWorldLandmark(24).z;
        // z += detecter.GetPoseWorldLandmark(23).z;
        // z /= 4;
        //
        // Vector3 center = new Vector3();
        // center.x = x;
        // center.y = y;
        // center.z = z;
        // torso.position = center;
        //
        // temp.x = detecter.GetPoseWorldLandmark(12).x;
        // temp.y = detecter.GetPoseWorldLandmark(12).y;
        // temp.z = detecter.GetPoseWorldLandmark(12).z;
        // rightShoulder.position = temp;
        //         
        // temp.x = detecter.GetPoseWorldLandmark(11).x;
        // temp.y = detecter.GetPoseWorldLandmark(11).y;
        // temp.z = detecter.GetPoseWorldLandmark(11).z;
        // leftShoulder.position = temp;
        //         
        // temp.x = detecter.GetPoseWorldLandmark(24).x;
        // temp.y = detecter.GetPoseWorldLandmark(24).y;
        // temp.z = detecter.GetPoseWorldLandmark(24).z;
        // upperLeftLeg.position = temp;
        //         
        // temp.x = detecter.GetPoseWorldLandmark(23).x;
        // temp.y = detecter.GetPoseWorldLandmark(23).y;
        // temp.z = detecter.GetPoseWorldLandmark(23).z;
        // upperRightLeg.position = temp;
        //
        // temp.x = detecter.GetPoseWorldLandmark(13).x;
        // temp.y = detecter.GetPoseWorldLandmark(13).y;
        // temp.z = detecter.GetPoseWorldLandmark(13).z;
        // lowerLeftArm.position = temp;
        //
        // temp.x = detecter.GetPoseWorldLandmark(14).x;
        // temp.y = detecter.GetPoseWorldLandmark(14).y;
        // temp.z = detecter.GetPoseWorldLandmark(14).z;
        // lowerRightArm.position = temp;
        //
        //
        // rightHand.x = detecter.GetPoseWorldLandmark(15).x;
        // rightHand.y = detecter.GetPoseWorldLandmark(15).y;
        // rightHand.z = detecter.GetPoseWorldLandmark(15).z;
        //
        // leftHand.x = detecter.GetPoseWorldLandmark(16).x;
        // leftHand.y = detecter.GetPoseWorldLandmark(16).y;
        // leftHand.z = detecter.GetPoseWorldLandmark(16).z;
        //
        // temp.x = detecter.GetPoseWorldLandmark(15).x;
        // temp.y = detecter.GetPoseWorldLandmark(15).y;
        // temp.z = detecter.GetPoseWorldLandmark(15).z;
        // leftWrist.position = temp;
        //
        // //animator.SetIKPosition(AvatarIKGoal.RightHand, temp);
        //
        // temp.x = detecter.GetPoseWorldLandmark(16).x;
        // temp.y = detecter.GetPoseWorldLandmark(16).y;
        // temp.z = detecter.GetPoseWorldLandmark(16).z;
        // rightWrist.position = temp;
        //
        // //animator.SetIKPosition(AvatarIKGoal.LeftHand, temp);
        
        Debug.Log("---");
    } 

    void OnRenderObject(){
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

    void OnApplicationQuit(){
        // Must call Dispose method when no longer in use.
        detecter.Dispose();
    }
}
