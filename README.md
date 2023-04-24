
# Conduction
Conduction is an application that gives some objective measurements to conductors and also allows instructors to help teach conductors through a constructive feedback loop of reinforcement learning.
It was developed using Unity.

## Development
Conduction was originally developed for the Spring and Summer 2022 semesters for UCF's CS Senior Design course, and continued with another group for the Fall 2022 and Spring 2023 semesters. The project sponsor was Dr. Scott Lubaroff, Director of Bands at UCF.

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
* `/Data` : Recorded videos and associated data separated into folders by date and time
* `/Captures` : Screen-recorded videos of feedback given in the Playback Screen
* `/ProjectSettings` : Unity project settings, which cover everything from editor settings to build settings
* `/Assets` : All of the assets (objects, prefabs, scripts, etc.) used in the creation of Conduction
* `/Assets/Audio` : Sounds for the metronome
* `/Assets/Evereal` : The Evereal Video Capture library, used for video recording evaluations
* `/Assets/Prefabs` : Unity prefabs
* `/Assets/Resources` : Icons used for UI elements
* `/Assets/Scenes` : Unity scenes
* `/Assets/Scripts` : All scripts written by the Conduction team. Functionality of each script is gone over below.
* `/Assets/StreamingAssets` : Where all recordings are stored, and where FFmpeg binaries are stored so the user does not need to install FFmpeg.
* `/Assets/TextMesh Pro` : TMP configuration
* `/Assets/Textures` : Textures and sprites used for rendering UI elements
* `/Assets/Trails` : TrailRenderer configuration for hand trails

### Specific Scripts
#### `Analytics/scriptnamehere`
description

#### `Avatars/CharacterSelection`
This script holds the avatar models and keeps track of which avatar the user is currently hovered over in the CharacterSelection Scene.

#### `LoadAvatar1`
This script instantiates a prefab with the user selected avatar on scene startup.

#### `PoseVisualizer3D`
This script uses camera input to estimate landmarks of a user. Landmark indices can be seen here: https://github.com/google/mediapipe/blob/master/docs/solutions/pose.md.  These landmarks are then used to angle the limbs on the attached character to mirror the user's input.

#### `CameraController`
This script controls the camera on the viewing page, allowing for rotation of the camera as well as zooming in and out so focus can be given to the hands.

#### `ErrorAnalytics`
This script handles calculating and displaying hand distance analytics during playback.

#### `MainMenu/FileManager`
This script controls the File Browser on the landing page.

#### `MainMenu/FullScreenController`
This script handles the capability of enabling and disabling fullscreen mode at runtime while ensuring that no UI scaling occurs at the same time.

#### `MainMenu/ListController`
This script handles the list of recordings currently loaded into memory, and initailizes the list at program launch with recordings stored in the `StreamingAssets` folder.

#### `MainMenu/ListEntryController`
This script conrtols webcam entries listed in webcam select menu

#### `MainMenu/Recording`
This script contains a data structure for storing recordings and displaying them with the `ListController`.

#### `MainMenu/WebCamData`
This script is a class to hold data needed for webcam select menu

#### `MainMenu/WebcamSelectController`
This script controls the webcam select menu as a whole

#### `MultiScene/FPSCap`
This script ensures that at vital parts of the application when recording the FPS of the application matches an expected FPS, such that the recording comes out as expected and not faster or slower.

#### `MultiScene/MainManager`
This script holds all data that needs to be referenced across the app

#### `MultiScene/SceneLoader`
An extension of the SceneManager built into Unity that allows for a prompt to be shown on the screen if configured to do so.

#### `Multiscene/TrailController`
This script controls the length of the hand trails based on slider information.

#### `Playback/BezierManager`
This script handles converting points given by the MarkupManager into a set of points representing a bezier curve. It gives these points back to the MarkupManager which draws them, as bezier curves are able to better represent smooth lines than ones own mouse movement may.

#### `Playback/CaptureController`
This script handles the set of buttons on the viewing page that give the user control over video capture for evaluation. It calls functions that begin, stop, and cancel recordings in Evereal's video capture.

#### `Playback/ColorSelectorController`
This script handles rendering and giving functionality to the color selection wheel used for markup. When the circle is clicked on, the current pixel color is sampled and used to set the color used by the MarkupManager.

#### `Playback/MarkupManager`
This script handles all markup functionality, enabling the ability of the user to draw on a canvas drawn over the avatar on the viewing page.

#### `Playback/MediaControls`
This script handles all media control actions on the viewing page, such as volume, playback speed, direction of playback, play/pause, and the usage of the timeline for scrubbing.

#### `Playback/MicrophoneRecorder`
A simple microphone recorder used for evaluation purposes during video recording.

#### `Playback/ReplayController`
This script controls the Playback scene, including loading in the video from the directory listed in the MainManager

#### `Playback/SizeSelectorController`
Part of markup. This script controls the size of the markup lines based on a slider present on the viewing page.

#### `Recording/AxisController`
This script handles the visibility and movability of the axis lines present on the recording and viewing pages. It ensures that the configuration is consistent in the same session so the lines do not have to be repeatedly moved.

#### `Recording/BeatChecker`
Part of the metronome. It does the counts based on time to ensure that beats play at the correct time, even for irregular time signatures.

#### `Recording/DragModulePerspective`
Part of the axis lines. This script allows for generic moving of non-UI objects to the mouse cursor. It is configured only for moving the axis lines.

#### `Recording/Metronome`
This script handles the metronome on the recording page. It allows the user to play a sound at specified intervals to aid them in keeping time.

#### `Recording/MetronomeStorage`
Part of the metronome. This script contains a data structure for the metronome to more easily store configurations.

#### `Recording/RecordingController`
This script controls the Recording scene, including using Evereal to record the webcam video and starting and stopping recording. 

#### `Recording/TempoSlider`
Part of the metronome. This controls the tempo used by the metronome and is highly configurable.

#### `VisualizerData/FrameData`
This script is a class to hold data fo each frame.

#### `VisualizerData/Frames`
This script is a class to hold data for all frames recorded.

#### `VisualizerData/HandMovementData`
This script is a class to hold hand data needed for analytics in json format.

#### `VisualizerData/PoseVisualizer3D`
This script was based off of the BlazePose package we utilized. It transforms the video input to data points and saves it to the associated data.json file

#### `VisualizerData/WebCamInput`
This script was based off of the BlazePose package we utilized. It puts the either the webcam feed or an uploaded video into a thumbnail on the screen.


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
- [x] Cross platform compatibility
- [ ] Track baton conducting
- [x] Applicability in live environments
- [x] Upload pre-recorded videos
- [x] Use any webcam, built-in or external

## Known Bugs
*

## V1 Members
Vijay Stroup, Connor Cabrera, Jordan McMillan, Michael Sedlack, Damian Portela

## V2 Members
Alexandra Brown, Allison Emokpae, Gabriel De David, Jonathan Haldas, Noah Frank, Rafael Sanchez

### V3 Ideas
Integrated grading feature
- Different users for different students
- Instructor can track student progress over time
- Student view with performance history
- Website so data and feedback can be viewed from anywhere

