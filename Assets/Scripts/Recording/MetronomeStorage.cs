using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Metronome data structure

struct metronomeInfo
{
    private string name;
    private float tempo;
    private int timeSigLow;
    private int timeSigUp;
    private int measures;
    private bool pickups;
    private int pickupUp;
    private int pickupLow;
    private AudioClip metLowBeat;
    private AudioClip metUpBeat;

    #region Sets
    public void SetName(string newName)
    {
        name = newName;
    }

    public void SetTempo(float newTempo)
    {
        tempo = newTempo;
    }

    public void SetTimeSigLow(int newTimeSigLow)
    {
        timeSigLow = newTimeSigLow;
    }

    public void SetTimeSigUp(int newTimeSigUp)
    {
        timeSigUp = newTimeSigUp;
    }

    public void SetMeasures(int newMeasures)
    {
        measures = newMeasures;
    }

    public void SetPickups(bool newPickups)
    {
        pickups = newPickups;
    }

    public void SetPickupUp(int newPickupUp)
    {
        pickupUp = newPickupUp;
    }

    public void SetPickupLow(int newPickupLow)
    {
        pickupLow = newPickupLow;
    }

    public void SetMetLowBeat(AudioClip newMetLowBeat)
    {
        metLowBeat = newMetLowBeat;
    }

    public void SetMetUpBeat(AudioClip newMetUpBeat)
    {
        metUpBeat = newMetUpBeat;
    }
    #endregion

    #region Gets
    public string GetName()
    {
        return name;
    }

    public float GetTempo()
    {
        MainManager.Instance.tempoBeat = tempo;
        return tempo;
    }

    public int GetTimeSigLow()
    {
        return timeSigLow;
    }

    public int GetTimeSigUp()
    {
        return timeSigUp;
    }

    public int GetMeasures()
    {
        return measures;
    }

    public bool GetPickups()
    {
        return pickups;
    }

    public int GetPickupUp()
    {
        return pickupUp;
    }

    public int GetPickupLow()
    {
        return pickupLow;
    }

    public AudioClip GetMetLowBeat()
    {
        return metLowBeat;
    }

    public AudioClip GetMetUpBeat()
    {
        return metUpBeat;
    }
    #endregion
}

public class MetronomeStorage : MonoBehaviour
{
    metronomeInfo met;

#region publicVariables
    public string title;
    public float tempo;
    public int timeSigUp; //time signature is split between an upper number and lower number, up and low correspond accordingly
    public int timeSigLow;
    public int measures;
    public bool pickups; //if there are additional beats at the beginning of a song that aren't a full measrue
    public int pickupUp; //pickup time signature upper and lower
    public int pickupLow;
    public AudioClip metLowBeat;
    public AudioClip metUpBeat;
    #endregion

    #region sets
    public void Initialize()
    {
        met.SetName(title);
        met.SetTempo(tempo);
        met.SetTimeSigLow(timeSigLow);
        met.SetTimeSigUp(timeSigUp);
        met.SetMeasures(measures);
        met.SetPickups(pickups);
        met.SetPickupUp(pickupUp);
        met.SetPickupLow(pickupLow);
        met.SetMetLowBeat(metLowBeat);
        met.SetMetUpBeat(metUpBeat);
    }
    
#endregion

    #region gets
    public double getBeatTime()
    {
        //60/tempo because tempo is beats per minute, 60 seconds a minute
        return 60 / met.GetTempo();
    }

    public double getSongLengthInSeconds()
    {
        //gets duration of song by multiplying how long it is by how long a beat is
        return getBeatTime() * met.GetMeasures() * met.GetTimeSigUp();
    }

    public string GetName()
    {
        return met.GetName();
    }

    public float GetTempo()
    {
        return met.GetTempo();
    }

    public int GetTimeSigUp()
    {
        return met.GetTimeSigUp();
    }

    public int GetTimeSigLow()
    {
        return met.GetTimeSigLow();
    }

    public bool GetIfPickups()
    {
        return met.GetPickups();
    }

    public int GetPickupUp()
    {
        return met.GetPickupUp();
    }

    public int GetPickupLow()
    {
        return met.GetPickupLow();
    }
    #endregion

    #region Sets
    public void SetName(string name)
    {
        met.SetName(name);
    }

    public void SetTempo(float tempo)
    {
        met.SetTempo(tempo);
    }

    public void SetTimeSigUp(int timeSigUp)
    {
        met.SetTimeSigUp(timeSigUp);
    }

    public void SetTimeSigLow(int timeSigLow)
    {
        met.SetTimeSigLow(timeSigLow);
    }

    public void SetIfPickups(bool pickups)
    {
        met.SetPickups(pickups);
    }

    public void SetPickupUp(int pickupUp)
    {
        met.SetPickupUp(pickupUp);
    }

    public void SetPickupLow(int pickupLow)
    {
        met.SetPickupLow(pickupLow);
    }
    #endregion
}

