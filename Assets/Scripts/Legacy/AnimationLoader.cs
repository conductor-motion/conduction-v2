using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// Loads animations from files and returns a built legacy AnimationClip
public class AnimationLoader
{
    private static GameObject avatar;
    private static Dictionary<GameObject, string> avatarComponents = new Dictionary<GameObject, string>();

    // Given the the path to a file, use the contents to create a new AnimationClip
    // Needs an avatar to build curves for from the serialized data
    public static AnimationClip LoadExistingClip(GameObject _avatar, string relPath)
    {
        AnimationClip reconstructedClip = new AnimationClip();
        reconstructedClip.legacy = true;

        Dictionary<string, AnimationCurve> reconstructedCurves = new Dictionary<string, AnimationCurve>();

        // Using the provided avatar, create a list of parts through recursion
        avatar = _avatar;
        avatarComponents.Clear();
        MapAvatar(avatar, "");

        // Initialize the reconstructed curves with the provided avatar
        foreach (GameObject obj in avatarComponents.Keys)
        {
            reconstructedCurves.Add(obj.name + ".x", new AnimationCurve());
            reconstructedCurves.Add(obj.name + ".y", new AnimationCurve());
            reconstructedCurves.Add(obj.name + ".z", new AnimationCurve());
            reconstructedCurves.Add(obj.name + ".Qx", new AnimationCurve());
            reconstructedCurves.Add(obj.name + ".Qy", new AnimationCurve());
            reconstructedCurves.Add(obj.name + ".Qz", new AnimationCurve());
            reconstructedCurves.Add(obj.name + ".Qw", new AnimationCurve());
        }

        // TODO: decompress whatever is given first
        FileStream animFile = new FileStream(Path.Combine(Application.streamingAssetsPath, relPath + ".anim"), FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();

        Dictionary<string, List<(float, float)>> serializableCurves = (Dictionary<string, List<(float, float)>>)bf.Deserialize(animFile);

        // Using similar methods in CreateLegacyAnimClip, recreate the clip
        foreach (KeyValuePair<string, List<(float, float)>> data in serializableCurves)
        {
            for (int k = 0; k < data.Value.Count; k++)
            {
                Keyframe key = new Keyframe(data.Value[k].Item1, data.Value[k].Item2);
                reconstructedCurves[data.Key].AddKey(key);
            }
        }

        // Now, essentially a copy-paste of the wall of text in our original CreateLegacyAnimClip
        foreach (KeyValuePair<string, AnimationCurve> data in reconstructedCurves)
        {
            GameObject originalObj = null;
            if (GameObject.Find(data.Key.Substring(0, data.Key.Length - 3)))
            {
                originalObj = GameObject.Find(data.Key.Substring(0, data.Key.Length - 3));
            }
            else if (GameObject.Find(data.Key.Substring(0, data.Key.Length - 2)))
            {
                originalObj = GameObject.Find(data.Key.Substring(0, data.Key.Length - 2));
            }

            string property = "";
            if (data.Key.Substring(data.Key.Length - 2) == "Qw")
            {
                property = "localRotation.w";
            }
            else if (data.Key.Substring(data.Key.Length - 2) == "Qz")
            {
                property = "localRotation.z";
            }
            else if (data.Key.Substring(data.Key.Length - 2) == "Qy")
            {
                property = "localRotation.y";
            }
            else if (data.Key.Substring(data.Key.Length - 2) == "Qx")
            {
                property = "localRotation.x";
            }
            else if (data.Key.Substring(data.Key.Length - 1) == "z")
            {
                property = "localPosition.z";
            }
            else if (data.Key.Substring(data.Key.Length - 1) == "y")
            {
                property = "localPosition.y";
            }
            else if (data.Key.Substring(data.Key.Length - 1) == "x")
            {
                property = "localPosition.x";
            }
            
            reconstructedClip.SetCurve(avatarComponents[originalObj], typeof(Transform), property, data.Value);
        }

        reconstructedClip.wrapMode = WrapMode.Loop;

        return reconstructedClip;
    }

    // Traverse the assigned avatarModel and recursively populate the avatarComponents dictionary for legacy anims
    static void MapAvatar(GameObject obj, string rel)
    {
        // First insert the current part and then call this on every child
        // Contains an exception for trails and the avatar root
        if (!obj.GetComponent<Animator>() && !obj.GetComponent<TrailRenderer>()) avatarComponents.Add(obj, rel);

        foreach (Transform child in obj.transform)
        {
            MapAvatar(child.gameObject, rel + (rel.Length == 0 ? "" : "/") + child.gameObject.name);
        }
    }
}
