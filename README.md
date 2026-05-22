# VR 360 Video Show

An immersive **360° virtual reality concert experience** built in **Unity** for **Meta Quest** devices.

This project combines 360° video recording, ambisonic audio capture, spatial interaction, and VR development to create an interactive concert experience where the user can watch a live performance from multiple selectable viewpoints.

The application was developed as part of an interdisciplinary academic project connected to sound production and immersive media. It brings together audiovisual recording, music performance, ambisonic sound, and Unity-based VR development.

---

## Overview

**VR 360 Video Show** is a standalone VR application designed for Meta Quest headsets.

The experience places the user inside a 360° concert environment, where video content is projected onto the inside of an inverted sphere. The user can look around naturally using head tracking and switch between predefined viewpoints positioned around the performance space.

Unlike a simple linear 360° video player, this application gives the viewer agency. The user can choose different perspectives during the show while the performance continues playing, preserving the temporal continuity of the concert.

The project focuses on:

- Immersive 360° video playback
- Ambisonic spatial audio
- Multiple selectable viewpoints
- VR comfort and motion sickness reduction
- Hand tracking and controller-based interaction
- World-space interface design
- Standalone Meta Quest deployment
- External media loading for large 360° video files

---

## Academic Context

Project developed by student **Sergio Botelho**  
Course: **Master’s in Sound Production and Technologies**  
Discipline: **Interdisciplinary Project**  
Professors: **Adriana Sá** and **David Novak**

The project combines sound production, 360° audiovisual capture, ambisonic recording, video/audio editing, and VR application development.

---

## Collaboration

This was a collaborative project involving audiovisual production, music performance, technical recording, editing, and VR development.

**Sergio Botelho** was responsible for the 360° video and audio recording process, as well as the editing of the recorded material.

**Felipe Buchabqui Saenger Marzanasco** was responsible for the Unity VR development of the application, including the interactive system, VR playback architecture, user interface, viewpoint switching, and Meta Quest deployment.

---

## Technical Credits

### .DAISY. — Performance

- **Lead Vocals, Acoustic Guitar and Keyboards:** .Daisy.
- **Bass:** Nikolas Gomes
- **Electric Guitar:** Vasco Santana
- **Drums:** André Cunha

### Audio and Video

- **Audio and Video Operator:** Sérgio Botelho

### VR Development

- **VR Unity Dev:** Felipe Buchabqui Saenger Marzanasco

---

## Recording Equipment

### Audio Recorders

- SoundDevices MixPre-10 II
- Zoom F6
- Zoom H3-VR

### Ambisonic Microphones

- Sennheiser AMBEO
- Saramonic SR-VRMIC
- Zoom H3-VR

### 360° Cameras

- Insta360 X3
- Insta360 X5
- GoPro Max
- GoPro Fusion

---

## Main Features

- 360° equirectangular video playback in VR
- Video projection onto an inverted sphere
- Multiple selectable viewpoints during the show
- Continuous playback when switching perspectives
- Fade-to-black transitions for comfort and loading masking
- Ambisonic spatial audio support
- Meta Quest standalone deployment
- Hand tracking support
- VR controller support
- Poke interaction for close-range UI selection
- Ray interaction for distant world-space UI elements
- Head tracking for natural 360° viewing
- World-space interface integrated into the VR scene
- Animated viewpoint icons positioned in 3D space
- Lobby environment before the show experience
- Lightmapped environment optimization for standalone VR
- External media loading through sideloaded video files

---

## Technologies Used

- Unity
- C#
- Universal Render Pipeline, also known as URP
- Meta Quest
- Meta SDK
- Meta XR Interaction SDK
- Meta Audio SDK
- Unity VideoPlayer
- Ambisonic audio
- Figma for UI planning
- Visual Studio for C# development
- Android build pipeline for standalone VR deployment

---

## How the Experience Works

The show is displayed using a 2D equirectangular 360° video mapped onto the inside of a sphere with inverted normals.

The user is positioned at the center of this sphere. As the user rotates their head, the visible part of the video changes naturally, creating the sensation of being inside the recorded performance.

---

## Viewpoint Switching System

A central feature of the project is the ability to switch between different 360° camera perspectives during the show.

Each viewpoint button stores references to:

- A target 360° video file
- A corresponding ambisonic audio file
- Position and scale data for the world-space UI
- Viewpoint-specific configuration parameters

When the user selects a new viewpoint, the application performs a transition sequence:

1. The screen fades to black.
2. The destination video is assigned to a secondary VideoPlayer.
3. The new video is prepared in the background.
4. The current playback timestamp is copied from the active player.
5. The new video and audio are synchronized to the same timestamp.
6. The secondary player becomes the active player.
7. The screen fades back into the show.

This allows the user to change perspective without restarting the performance from the beginning.

---

## Dual VideoPlayer Architecture

The project uses two Unity VideoPlayer components instead of a single player.

This architecture helps reduce visible loading delays when switching between large external 360° video files. While one VideoPlayer is actively rendering the current perspective, the second one can prepare the next selected video in the background.

During the fade-to-black transition, the system swaps the active media source only after the new video is prepared and synchronized.

This makes transitions feel smoother and avoids abrupt interruptions in the experience.

---

## Ambisonic Audio

The project uses ambisonic audio to increase the sense of presence inside the concert environment.

Each viewpoint can be associated with its own ambisonic audio asset, allowing the perceived sound field to match the selected visual perspective.

The audio is processed using Meta Audio SDK, allowing the spatial sound field to respond to the user’s head rotation. This helps preserve the sensation that sound is coming from consistent positions inside the virtual performance space.

This is especially important in a concert experience, where the perceived position of instruments, vocals, audience sound, and stage elements contributes strongly to immersion.

---

## Interaction Model

The application supports multiple VR interaction methods.

### Hand Tracking

Users can interact with the interface using their hands, creating a more natural and direct interaction model.

### Controllers

The experience also supports traditional Meta Quest controllers, making it accessible in different usage scenarios.

### Poke Interaction

Poke interaction is used for close-range interface elements, allowing users to press buttons directly with their hands or controllers.

### Ray Interaction

Ray interaction is used for selecting world-space UI elements positioned farther away in the scene.

This combination allows the interface to remain flexible, readable, and usable in immersive VR.

---

## World-Space UI

The interface is implemented in world space instead of being fixed to the screen.

Each selectable viewpoint is represented by an icon placed in a specific 3D position inside the scene. These icons can be positioned above artists, instruments, or relevant locations in the concert environment.

To improve visibility and affordance, the icons use a subtle vertical animation, helping users identify them as interactive elements without overwhelming the scene.

The interface also rotates toward the user on the Y axis, improving readability while preserving spatial consistency. This avoids making the UI feel completely detached from the environment.

---

## Lobby Environment

Before entering the show, the user starts in a themed VR lobby.

The lobby is not only a menu. It also works as a sensory preparation space before the concert experience.

The environment helps the user adapt to:

- The VR scale
- The darker visual mood of the show
- Spatial sound perception
- Basic interaction mechanics
- The overall atmosphere of the experience

The lobby includes sound-emitting elements such as speakers, lights, and environmental objects to introduce the user to the spatial audio behavior before the main performance begins.

The 3D environment was optimized for standalone VR using baked lighting and lightmaps, reducing runtime rendering cost while preserving visual quality.

---

## Comfort Design

Comfort was an important design consideration throughout the project.

Instantly cutting between 360° viewpoints can feel disorienting in VR because the user’s spatial reference changes abruptly. To reduce this discomfort, the application uses fade-to-black transitions between perspectives.

The fade serves two purposes:

1. It masks the loading and preparation of the next video.
2. It creates a perceptual buffer that reduces the risk of discomfort or motion sickness.

This makes the experience feel more cinematic, controlled, and comfortable.

---

## External Media Loading

Large 360° video files can make a standalone VR build extremely heavy and difficult to deploy.

To avoid this, the project uses an external media workflow. Instead of embedding all video files directly into the application build, the videos can be stored in a public or app-accessible directory on the Meta Quest device and loaded at runtime.

This approach has several advantages:

- Smaller application build size
- Easier replacement of video files
- Faster testing iteration
- Better separation between application logic and heavy media assets
- More flexible deployment for different show recordings

Example Quest storage paths may include:

```txt
/storage/emulated/0/Download/
```

but for this project a custom folder was created:
```txt
/storage/emulated/0/Show Videos/
```

The exact path depends on the deployment configuration on build time.

---

## Target Platform

The project is designed primarily for:

```txt
Meta Quest 2
Meta Quest 3
```

The application targets standalone Android-based VR deployment through Unity.

---

## Project Structure

A simplified project structure:

```txt
Video Show/
│
├── Assets/
│   ├── Materials/
│   ├── Models/
│   ├── Scenes/
│   ├── Scripts/
│   ├── Sounds/
│   ├── User Interface/
│   └── Videos/

```

Large video files are intentionally not included in this repository.

---

## Running the Project

To run the project locally:

1. Open the project in Unity.
2. Make sure the required Meta XR packages are installed.
3. Configure the project for Android build target.
4. Connect a Meta Quest device with developer mode enabled.
5. Place the required 360° video and audio files in the expected device directory.
6. Build and run the application on the headset.

Because the project depends on large external media files, the repository may not include the original show videos or audio assets.

---

## Repository Notes

This repository is intended to showcase the technical structure and development approach behind the VR experience.

Some assets may be excluded due to:

- File size limitations
- Media licensing
- Artist rights
- Recording rights
- Academic project constraints
- GitHub repository size best practices

## Status

This project is a functional VR prototype focused on immersive 360° concert visualization.

Further improvements may include:

- More robust media loading system
- Improved video/audio synchronization
- Better error handling for missing media files
- More polished lobby interactions
- Additional viewpoint transition effects
- Quest 3-specific optimization
- In-app media selection menu
- Improved onboarding tutorial
- More detailed debug tools for media playback

---

## Credits

### Project and Academic Context

- **Project developed by:** Sergio Botelho
- **Course:** Master’s in Sound Production and Technologies
- **Discipline:** Interdisciplinary Project
- **Professors:** Adriana Sá and David Novak

### Performance — .DAISY.

- **Lead Vocals, Acoustic Guitar and Keyboards:** .Daisy.
- **Bass:** Nikolas Gomes
- **Electric Guitar:** Vasco Santana
- **Drums:** André Cunha

### Technical Production

- **Audio and Video Operator:** Sérgio Botelho
- **360° Video and Audio Recording:** Sérgio Botelho
- **Video and Audio Editing:** Sérgio Botelho
- **VR Unity Development:** Felipe B Marza

---

## License and Media Rights

This repository is intended for portfolio, academic, and development purposes.

Original show video, audio recordings, ambisonic files, edited media, and performance materials may be excluded from the repository due to file size, licensing, artist rights, or academic project restrictions.

Do not redistribute the original audiovisual material without permission from the involved artists, performers, and technical collaborators.
