using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//if doing music in 6/8 or smoething like that, convert it from quarternote bpm to dotted quarternote bpm and from 6/8 to 2/4

public class BeatChecker : MonoBehaviour
{
    public Toggle UIToggle;
    public bool playExtraBeatLength = true; //variable that determines whether things in 7/8 for example will play the and of 3 or have it be a long 3
    public bool metMode = true; //setting that switches from metMode (doesn't check for the end of the song and continuously loops a set measure) to songMode (can be set up to have a specific length, tempo changes, time sig changes, the works)

    private MetronomeStorage metronome;
    private double beatLength;
    private float nextBeat;
    private float previousBeatTime;
    private float nextBeatTime;
    private bool playBeat; //variable anything that involves rhythm will look at to know if it should move
    private bool finishedPickup; //variable that is used in determining if the beat will be based on the pickup time sig or the remainder of the song's time sig
    public bool play = false;

    private void Start()
    {
        //grabs the song
        metronome = GameObject.FindGameObjectWithTag("Metronome").GetComponent<MetronomeStorage>();

        //ensures that the song starts, gets the current time to establish how long until the next beat
        metronome.Initialize();
        beatLength = metronome.getBeatTime();
        nextBeat = metronome.GetTimeSigLow() / 4f; //Returns what the next whole quarter note is in the measure e.g.: 1 2 3 4 in 4/4 1 2 3 4 in 9/8 because the 4 is a dotted 4 it doesn't count the extra quarter note. This can be toggled w/ play extrabeatlength
        finishedPickup = !metronome.GetIfPickups();
        if((metronome.GetTimeSigUp() % (metronome.GetTimeSigLow() / 4))!=0)
            Debug.Log(calculateExtraBeatSize());
    }
    private void Update()
    {
        if (beatCheck())
        {
            if (!finishedPickup) //checks if there is a pickup and plays it accordingly, using the same stuff as normal, jsut replacing which time sig data it's snagging
            {
                nextBeat += (metronome.GetPickupLow() / 4f);
                beatLength = metronome.getBeatTime();
                //When the next beat should loop back to the begining
                if (nextBeat > metronome.GetPickupUp() && playExtraBeatLength)
                {
                    //set to true to exit the pickup state and enter normal music state
                    finishedPickup = true;

                    //reset next beat to be beat 1   
                    if (metronome.GetPickupLow() < 4)
                    {
                        nextBeat = metronome.GetPickupLow() / 4f;
                    }
                    else
                    {
                        nextBeat = 1;
                    }

                    //this funky equation is used for any odd time signatures that end in less than full beats
                    if ((metronome.GetPickupUp() % (metronome.GetPickupLow() / 4f)) != 0)
                    {
                        beatLength = metronome.getBeatTime() * calculateExtraPickupBeatSize();
                    }
                }
                else if (nextBeat > metronome.GetPickupUp())
                {
                    //set to true to exit the pickup state and enter normal music state
                    finishedPickup = true;

                    //reset next beat
                    if(metronome.GetPickupLow() < 4)
                    {
                        nextBeat = metronome.GetPickupLow() / 4f;
                    }
                    else
                    {
                        nextBeat = 1;
                    }

                    if ((metronome.GetPickupUp() % (metronome.GetPickupLow()/4f)) != 0)
                    {
                        beatLength = metronome.getBeatTime() + metronome.getBeatTime() * calculateExtraPickupBeatSize();
                    }
                }

                previousBeatTime = nextBeatTime;
                nextBeatTime = (float)(previousBeatTime + beatLength);

                playBeat = true;
            }

            else
            {
                Debug.Log(nextBeat);
                nextBeat += metronome.GetTimeSigLow() / 4f;//3 5 7 9
                
                beatLength = metronome.getBeatTime();

                //When the next beat should loop back to the begining
                if (nextBeat > metronome.GetTimeSigUp())
                {
                    //reset next beat
                    if (metronome.GetTimeSigLow() < 4)
                    {
                        nextBeat = metronome.GetTimeSigLow() / 4f;
                    }
                    else
                    {
                        nextBeat = 1;
                    }

                    if (playExtraBeatLength)
                    {
                        if ((metronome.GetTimeSigUp() % ((float)metronome.GetTimeSigLow() / 4f)) != 0)
                        {
                            beatLength = metronome.getBeatTime() * calculateExtraBeatSize();
                        }
                    }
                    else
                    {
                        if ((metronome.GetTimeSigUp() % ((float)metronome.GetTimeSigLow() / 4f)) != 0)
                        {
                            beatLength = metronome.getBeatTime() + metronome.getBeatTime() * calculateExtraBeatSize();
                        }
                    }
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

    private float calculateNextBeat(float beat)
    {
        float returnVal = beat + (metronome.GetTimeSigLow() / 4f);
        if (returnVal > metronome.GetTimeSigUp())
        {
            if (metronome.GetTimeSigLow() < 4)
            {
                return metronome.GetTimeSigLow() / 4f;
            }
            else
            {
                return 1;
            }
        }
        return returnVal;
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
        if (metronome.GetTimeSigLow() < 4)
            return nextBeat == calculateNextBeat(4f / metronome.GetTimeSigLow());
        else
            return nextBeat == calculateNextBeat(1);
    }

    public void startMetronome()
    {
        play = true;
        MainManager.Instance.metronomePlay = play;
        previousBeatTime = Time.time; //the time that the last beat played was played
        nextBeatTime = Time.time; //when the next beat should be played
        nextBeat = 1;
    }

    public void stopMetronome()
    {
        play = false;
    }

    public void KeyPress(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UIToggle.GetComponent<Toggle>().isOn = !UIToggle.GetComponent<Toggle>().isOn;
        }
    }

    public void toggleMetronome()
    {
        if(play)
        {
            stopMetronome();
        }
        else
        {
            startMetronome();
        }
    }

    //This equation finds how long the final beat should be relative to the size of a quarter note
    //(upper%(l/4))/(l/4)
    private float calculateExtraPickupBeatSize()
    {
        return (float)(metronome.GetTimeSigUp() % (metronome.GetTimeSigLow() / 4)) / (metronome.GetTimeSigLow() / 4);
    }

    private float calculateExtraBeatSize()
    {
        return (float)(metronome.GetTimeSigUp() % (metronome.GetTimeSigLow() / 4)) / (metronome.GetTimeSigLow() / 4);
    }
}

