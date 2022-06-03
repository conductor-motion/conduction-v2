using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//if doing music in 6/8 or smoething like that, convert it from quarternote bpm to dotted quarternote bpm and from 6/8 to 2/4

public class BeatChecker : MonoBehaviour
{
    public bool playExtraBeatLength = true; //variable that determines whether things in 7/8 for example will play the and of 3 or have it be a long 3
    public bool metMode = true; //setting that switches from metMode (doesn't check for the end of the song and continuously loops a set measure) to songMode (can be set up to have a specific length, tempo changes, time sig changes, the works)

    private MetronomeStorage metronome;
    private double beatLength;
    private int nextBeat;
    private float previousBeatTime;
    private float nextBeatTime;
    private bool playBeat; //variable anything that involves rhythm will look at to know if it should move
    private bool finishedPickup; //variable that is used in determining if the beat will be based on the pickup time sig or the remainder of the song's time sig
    private bool play = false;

    private void Start()
    {
        //grabs the song
        metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<MetronomeStorage>();

        //ensures that the song starts, gets the current time to establish how long until the next beat
        metronome.initialize();
        beatLength = metronome.getBeatTime();
        nextBeat = 0; //which beat comes next
        finishedPickup = !metronome.getIfPickups();
    }
    private void Update()
    {
        if (beatCheck())
        {
            if (!finishedPickup) //checks if there is a pickup and plays it accordingly, using the same stuff as normal, jsut replacing which time sig data it's snagging
            {
                nextBeat += (metronome.getPickupLow() / 4);
                beatLength = metronome.getBeatTime();
                //When the next beat should loop back to the begining
                if (nextBeat > metronome.getPickupUp() && playExtraBeatLength)
                {
                    //set to true to exit the pickup state and enter normal music state
                    finishedPickup = true;

                    //reset next beat to be beat 1   
                    nextBeat = 1;

                    //this funky equation is used for any odd time signatures that end in less than full beats
                    if (metronome.getPickupLow() != 4)
                    {
                        //equation is the length of a beat * (4*timeUp)/(timeLow) - 3
                        //found by simplifying (low/4 - (low-up))/(low/4)
                        beatLength = metronome.getBeatTime() * (((4 * (float)metronome.getPickupUp()) / (float)metronome.getPickupLow()) - 3);
                    }
                }
                else if (nextBeat > metronome.getTimeSigUp())
                {
                    //set to true to exit the pickup state and enter normal music state
                    finishedPickup = true;

                    //reset next beat to be beat 1   
                    nextBeat = 1;

                    if (metronome.getTimeSigLow() != 4)
                    {
                        //same equation as above, but this time working to add in the previous beat to the ordeal
                        beatLength = metronome.getBeatTime() + metronome.getBeatTime() * (((4 * (float)metronome.getPickupUp()) / (float)metronome.getPickupLow()) - 3);
                    }
                }

                previousBeatTime = nextBeatTime;
                nextBeatTime = (float)(previousBeatTime + beatLength);

                playBeat = true;
            }

            else
            {
                nextBeat += (metronome.getTimeSigLow() / 4);
                beatLength = metronome.getBeatTime();

                //When the next beat should loop back to the begining
                if (nextBeat > metronome.getTimeSigUp() && playExtraBeatLength)
                {
                    //reset next beat to be beat 1   
                    nextBeat = 1;

                    //same equations as above
                    if (metronome.getTimeSigLow() != 4)
                        beatLength = metronome.getBeatTime() * (((4 * (float)metronome.getTimeSigUp()) / (float)metronome.getTimeSigLow()) - 3);
                }

                else if (nextBeat > metronome.getTimeSigUp())
                {
                    //reset next beat to be beat 1   
                    nextBeat = 1;

                    //you know the deal
                    if (metronome.getTimeSigLow() != 4)
                        beatLength = metronome.getBeatTime() + metronome.getBeatTime() * (((4 * (float)metronome.getTimeSigUp()) / (float)metronome.getTimeSigLow()) - 3);
                }

                previousBeatTime = nextBeatTime;
                nextBeatTime = (float)(previousBeatTime + beatLength);

                playBeat = true;
            }
        }

        else
            playBeat = false;
    }

    private bool beatCheck()
    {
        if (play)
        {
            if (Time.time >= nextBeatTime)
                return true;
            else
                return false;
        }

        else
            return false;
    }

    private int calculateNextBeat(int beat)
    {
        return beat + (metronome.getTimeSigLow() / 4);
    }

    public bool getPlayBeat()   // Returns true if a beat is happening now
    {
        return playBeat;
    }

    public float getNextBeat()  //  Returns the delta time of the next beat
    {
        return nextBeatTime;
    }

    public float getNextBeatRelative() // Returns the time from when the function is called to the next beat
    {
        return nextBeatTime - Time.time;
    }

    public bool isFirstBeat()
    {
        return nextBeat == calculateNextBeat(1);
    }

    public void startMetronome()
    {
        play = true;
        previousBeatTime = Time.time; //the time that the last beat played was played
        nextBeatTime = Time.time; //when the next beat should be played
    }
}

