using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawRecordingInfoLine : MonoBehaviour
{
    public static Material lineMaterial;

    public static int lineLength = 50;

    // Boilerplate for weird material system Unity uses for UI elements
    static void CreateLineMaterial()
    {
        // If none provided (default)
        if (!lineMaterial)
        {
            // Use an internal shader that works well for the purpose of curved lines
            Shader shader = Shader.Find("Hidden/Internal-Colored");

            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;

            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Called after normal UI rendering is completed
    public void OnDrawGizmos()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);
        GL.PushMatrix();
        // Initialize transofmration matrix for drawing
        GL.MultMatrix(transform.localToWorldMatrix);
        // Perform the actual drawing
        GL.Begin(GL.LINES);
        GL.Vertex3(0,0,0);
        for (int i = 0; i < lineLength; i++)
        {
            float y = Mathf.Sqrt(i);
            GL.Color(Color.red);
            GL.Vertex3(i,y,y);
        }
        GL.End();
        GL.PopMatrix();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
