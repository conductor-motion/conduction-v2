using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct metronomeInfo
{
    public string name;
    public float tempo;
    public int timeSigLow;
    public int timeSigUp;
    public int measures;
    public bool pickups;
    public int pickupUp;
    public int pickupLow;
    public AudioClip metLowBeat;
    public AudioClip metUpBeat;
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
    public void initialize()
    {
        met.name = title;
        met.tempo = tempo;
        met.timeSigLow = timeSigLow;
        met.timeSigUp = timeSigUp;
        met.measures = measures;
        met.pickups = pickups;
        met.pickupUp = pickupUp;
        met.pickupLow = pickupLow;
        met.metLowBeat = metLowBeat;
        met.metUpBeat = metUpBeat;
    }
    
#endregion

    #region gets
    public double getBeatTime()
    {
        //60/tempo because tempo is beats per minute, 60 seconds a minute
        return 60 / met.tempo;
    }

    public double getSongLengthInSeconds()
    {
        //gets duration of song by multiplying how long it is by how long a beat is
        return getBeatTime() * met.measures * met.timeSigUp;
    }

    public string getName()
    {
        return met.name;
    }

    public float getTempo()
    {
        return met.tempo;
    }

    public int getTimeSigUp()
    {
        return met.timeSigUp;
    }

    public int getTimeSigLow()
    {
        return met.timeSigLow;
    }

    public bool getIfPickups()
    {
        return met.pickups;
    }

    public int getPickupUp()
    {
        return met.pickupUp;
    }

    public int getPickupLow()
    {
        return met.pickupLow;
    }
    #endregion
}
