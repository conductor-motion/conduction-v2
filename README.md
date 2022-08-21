
# Conduction
Conduction is an application that gives some objective measurements to conductors and also allows instructors to help teach conductors through a constructive feedback loop of reinforcement learning.
It was developed using Unity.

## Development
Conduction was originally developed for the Spring and Summer 2022 semesters for UCF's CS Senior Design course. The project sponsor was Dr. Scott Lubaroff, Director of Bands at UCF.

## Usage
1. Download a release or compile a build appropriate for your platform using the Unity Editor.
2. Plug in your Kinect using the Xbox One Kinect Power Adaptor.
    * If using a Mac or Linux computer, drivers will not automatically install. To use Conduction, Kinect drivers must be installed from [libfreenect2](https://github.com/OpenKinect/libfreenect2).
2. Launch the built application.
3. Navigate to the new recording page.
4. Ensure the Kinect is being used by looking for its white indicator light.
5. Begin a new recording by pressing space or clicking the record button.
    * Before doing so, a metronome is present and configurable to aid the conductor in keeping time.
6. Complete a recording by pressing space again or clicking the record button.
7. Save the animation using the input box at the bottom left of the screen.
8. Perform markup or begin a video evaluation of the conducting animation just created.

### Repository Structure
* `/` : The root of the Unity project. Contains configuration files and DLLs used for interacting with the System and Kinect V2. 
* `/NuiDatabase`, `/OpenNI2`, `/vgbtechs` : Configurations and libraries for interfacing with the Kinect and processing data from it.
* `/ProjectSettings` : Unity project settings, which cover everything from editor settings to build settings
* `/Assets` : All of the assets (objects, prefabs, scripts, etc.) used in the creation of Conduction
* `/Assets/Audio` : Sounds for the metronome
* `/Assets/Evereal` : The Evereal Video Capture library, used for video recording evaluations
* `/Assets/K2Examples` : The Kinect v2 Examples package, which contain many demos for showing how to control and use the Kinect V2.
* `/Assets/Prefabs` : Unity prefabs
* `/Assets/Resources` : Icons used for UI elements
* `/Assets/Scenes` : Unity scenes
* `/Assets/Scripts` : All scripts written by the Conduction team. Functionality of each script is gone over below.
* `/Assets/StreamingAssets` : Where all recordings are stored, and where FFmpeg binaries are stored so the user does not need to install FFmpeg.
* `/Assets/TextMesh Pro` : TMP configuration
* `/Assets/Textures` : Textures and sprites used for rendering UI elements
* `/Assets/Trails` : TrailRenderer configuration for hand trails

### Specific Scripts
#### `AnimationLoader`
This script handles loading an animation into memory from the file system by reading the file specified and deserializing it into its Unity object (AnimationClip). The current method of serialization is the BinaryFormatter, which is unideal for space and efficiency purposes. Protobuf would be a good alternative, but requires a great deal of set up in serializing an object.

#### `AxisController`
This script handles the visibility and movability of the axis lines present on the recording and viewing pages. It ensures that the configuration is consistent in the same session so the lines do not have to be repeatedly moved.

#### `BeatChecker`
Part of the metronome. It does the counts based on time to ensure that beats play at the correct time, even for irregular time signatures.

#### `BezierManager`
This script handles converting points given by the MarkupManager into a set of points representing a bezier curve. It gives these points back to the MarkupManager which draws them, as bezier curves are able to better represent smooth lines than ones own mouse movement may.

#### `CameraController`
This script controls the camera on the viewing page, allowing for rotation of the camera as well as zooming in and out so focus can be given to the hands.

#### `CaptureController`
This script handles the set of buttons on the viewing page that give the user control over video capture for evaluation. It calls functions that begin, stop, and cancel recordings in Evereal's video capture.

#### `ColorSelectorController`
This script handles rendering and giving functionality to the color selection wheel used for markup. When the circle is clicked on, the current pixel color is sampled and used to set the color used by the MarkupManager.

#### `DragModulePerspective`
Part of the axis lines. This script allows for generic moving of non-UI objects to the mouse cursor. It is configured only for moving the axis lines.

#### `ErrorAnalytics`
This script handles calculating and displaying hand distance analytics during playback.

#### `FPSCap`
This script ensures that at vital parts of the application when recording the FPS of the application matches an expected FPS, such that the recording comes out as expected and not faster or slower.

#### `FullScreenController`
This script handles the capability of enabling and disabling fullscreen mode at runtime while ensuring that no UI scaling occurs at the same time.

#### `ListController`
This script handles the list of recordings currently loaded into memory, and initailizes the list at program launch with recordings stored in the `StreamingAssets` folder.

#### `MarkupManager`
This script handles all markup functionality, enabling the ability of the user to draw on a canvas drawn over the avatar on the viewing page.

#### `MediaControls`
This script handles all media control actions on the viewing page, such as volume, playback speed, direction of playback, play/pause, and the usage of the timeline for scrubbing.

#### `Metronome`
This script handles the metronome on the recording page. It allows the user to play a sound at specified intervals to aid them in keeping time.

#### `MetronomeStorage`
Part of the metronome. This script contains a data structure for the metronome to more easily store configurations.

#### `MicrophoneRecorder`
A simple microphone recorder used for evaluation purposes during video recording.

#### `MocapPlayerOurs`
This script handles beginning playback of a selected AnimationClip once the viewing screen has been loaded. Additionally, it also handles the saving of the audio and animation data if the user desires.
Suffixed with "ours" as it is a complete rewrite of a script based on a demo in K2Examples for legacy animations.

#### `MocapRecorderOurs`
This script handles converting motion information into a legacy AnimationClip based on an avatar provided. This does not interact with the Kinect, it simply reads the motion of the avatar that is mapped to the Kinect by another script.

#### `Recording`
This script contains a data structure for storing recordings and displaying them with the `ListController`.

#### `SceneLoader`
An extension of the SceneManager built into Unity that allows for a prompt to be shown on the screen if configured to do so.

#### `SizeSelectorController`
Part of markup. This script controls the size of the markup lines based on a slider present on the viewing page.

#### `TempoSlider`
Part of the metronome. This controls the tempo used by the metronome and is highly configurable.

#### `TrailController`
This script controls the length of the hand trails based on slider information.

### Relevant Library Scripts

#### `AvatarController`
`/Assets/K2Examples/KinectScripts/AvatarController.cs`

Provides an avatar controller to the `KinectManager`, allowing for motion to be mapped to the avatar this script is attached to. The object it is attached to must support a humanoid rig type. This is where most configuration of what you want the motion capture to actual capture is found in.

#### `KinectManager`
`/Assets/K2Examples/KinectScripts/KinectManager.cs`

Using any provided avatar controllers, maps motion from the kinect to the avatar. This can be configured to support tracking multiple people and other actions. This is where most configuration regarding the Kinect is located.

## Functionality Based on Specs Given by Sponsor
- [x] Record a conductor
- [x] Playback recording of a conductor
- [x] Export recordings for sharing (mp4 format)
- [x] Markup on recordings
- [x] Hand trails to track motion over time
- [x] Offline application (practice rooms often do not have a solid internet connection)
- [x] Track bare hands conducting
- [x] Horizontal and vertical overlay lines for the platform
- [x] Quantitative analytics
    * These are extremely basic and are just used to measure deviation. For instance, in some types of conducting if the hands are not equidistant, this could be improper form.
- [x] Rotate playback in 360 degrees
- [x] Background audio recording so the user has context in recordings
- [ ] Cross platform compatibility
    * Mac and Linux compatibility are not built in, as they require manual installation of drivers not packaged with Conduction
- [ ] Track baton conducting
    * There exists a challenge in using the Kinect to recognize something that is not a part of a human being
- [ ] Applicability in live environments
    * The program at its core is designed as a tool for a waterfall approach to improving recording. A redesign would be necessary to enable this.

## Known Bugs
* Sometimes the video recorder will record a blank screen. The temporary fix has been to restart the application. This occurs with a low occurrence rate.
* Modern Windows laptops that use Microphone Arrays are incompatible with audio recording, as Unity still does not support Microphone Arrays.
    * A fix is to somehow get that device to have a non-"Microphone Array" mic available for the program to select
* Some builds do not have a metronome present on the Recording page. This appears to be a Unity bug and is fixed by rebuilding the application.

## V1 Members
Vijay Stroup, Connor Cabrera, Jordan McMillan, Michael Sedlack, Damian Portela

### V2 Ideas
https://github.com/conductor-motion/conduction/wiki/V2-Ideas
