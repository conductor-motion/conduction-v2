using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void setName(string newName)
    {
        name = newName;
    }

    public void setTempo(float newTempo)
    {
        tempo = newTempo;
    }

    public void setTimeSigLow(int newTimeSigLow)
    {
        timeSigLow = newTimeSigLow;
    }

    public void setTimeSigUp(int newTimeSigUp)
    {
        timeSigUp = newTimeSigUp;
    }

    public void setMeasures(int newMeasures)
    {
        measures = newMeasures;
    }

    public void setPickups(bool newPickups)
    {
        pickups = newPickups;
    }

    public void setPickupUp(int newPickupUp)
    {
        pickupUp = newPickupUp;
    }

    public void setPickupLow(int newPickupLow)
    {
        pickupLow = newPickupLow;
    }

    public void setMetLowBeat(AudioClip newMetLowBeat)
    {
        metLowBeat = newMetLowBeat;
    }

    public void setMetUpBeat(AudioClip newMetUpBeat)
    {
        metUpBeat = newMetUpBeat;
    }
    #endregion

    #region Gets
    public string getName()
    {
        return name;
    }

    public float getTempo()
    {
        return tempo;
    }

    public int getTimeSigLow()
    {
        return timeSigLow;
    }

    public int getTimeSigUp()
    {
        return timeSigUp;
    }

    public int getMeasures()
    {
        return measures;
    }

    public bool getPickups()
    {
        return pickups;
    }

    public int getPickupUp()
    {
        return pickupUp;
    }

    public int getPickupLow()
    {
        return pickupLow;
    }

    public AudioClip getMetLowBeat()
    {
        return metLowBeat;
    }

    public AudioClip getMetUpBeat()
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
    public void initialize()
    {
        met.setName(title);
        met.setTempo(tempo);
        met.setTimeSigLow(timeSigLow);
        met.setTimeSigUp(timeSigUp);
        met.setMeasures(measures);
        met.setPickups(pickups);
        met.setPickupUp(pickupUp);
        met.setPickupLow(pickupLow);
        met.setMetLowBeat(metLowBeat);
        met.setMetUpBeat(metUpBeat);
    }
    
#endregion

    #region gets
    public double getBeatTime()
    {
        //60/tempo because tempo is beats per minute, 60 seconds a minute
        return 60 / met.getTempo();
    }

    public double getSongLengthInSeconds()
    {
        //gets duration of song by multiplying how long it is by how long a beat is
        return getBeatTime() * met.getMeasures() * met.getTimeSigUp();
    }

    public string getName()
    {
        return met.getName();
    }

    public float getTempo()
    {
        return met.getTempo();
    }

    public int getTimeSigUp()
    {
        return met.getTimeSigUp();
    }

    public int getTimeSigLow()
    {
        return met.getTimeSigLow();
    }

    public bool getIfPickups()
    {
        return met.getPickups();
    }

    public int getPickupUp()
    {
        return met.getPickupUp();
    }

    public int getPickupLow()
    {
        return met.getPickupLow();
    }
    #endregion

    #region Sets
    public void setName(string name)
    {
        met.setName(name);
    }

    public void setTempo(float tempo)
    {
        met.setTempo(tempo);
    }

    public void setTimeSigUp(int timeSigUp)
    {
        met.setTimeSigUp(timeSigUp);
    }

    public void setTimeSigLow(int timeSigLow)
    {
        met.setTimeSigLow(timeSigLow);
    }

    public void setIfPickups(bool pickups)
    {
        met.setPickups(pickups);
    }

    public void setPickupUp(int pickupUp)
    {
        met.setPickupUp(pickupUp);
    }

    public void setPickupLow(int pickupLow)
    {
        met.setPickupLow(pickupLow);
    }
    #endregion
}

