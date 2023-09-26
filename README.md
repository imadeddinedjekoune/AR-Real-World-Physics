# AR-Real-World-Physics

This project combines OpenCV and Unity to create a real-time interactive environment where objects detected by the camera's webcam using OpenCV are transformed into colliders in Unity.

## Demo 
[![Watch the video](https://github-production-user-asset-6210df.s3.amazonaws.com/75379150/270513617-640b31d3-85f2-4ffc-8239-2bd92b48e6b9.gif)](https://www.youtube.com/watch?v=gEnqBC4i7NE)

## Table of Contents

- [Introduction](#introduction)
- [Getting Started](#getting-started)
- [Project Details and Technical Explanation](#project-details-and-technical-explanation)
- [Contributing](#contributing)
- [License](#license)

## Introduction

The CountourCollider Unity Project is designed to turn the objects detected by your phone's camera into interactive elements within a Unity environment. The project has the following key features:

- Object Detection: OpenCV is used to identify and create contours around objects within the camera's field of view.
- Collider Generation: The detected contours are transformed into colliders within Unity, allowing for physical interactions with the detected objects.
- Emitter: An "Emitter" creates balls within the Unity scene, which interact with the detected objects.
- Kill Zone: A "Kill Zone" is used to reset objects that fall out of view, ensuring they are pushed back to the emitter for continuous interaction.

## Project Details and Technical Explanation
### Object Detection with OpenCV
In this project, we employ OpenCV for object detection. OpenCV is an open-source computer vision library that provides various tools and algorithms for image processing. Here's how object detection works within the project:

- **Thresholding:** We start by applying a threshold to the camera's input frames. Thresholding converts a grayscale image into a binary image by setting pixels above a certain intensity value to white and the rest to black. This helps isolate objects against a white background.

- **Contour Detection:** With thresholded images, we utilize OpenCV's contour detection algorithms. Contours are simply the boundaries of objects in an image. OpenCV identifies these contours, which represent the shapes of objects within the camera's view.

### Collider Generation in Unity
Once we have identified contours, we use Unity to transform them into colliders. Colliders define the physical boundaries of objects within the Unity scene. Here's how collider generation is achieved:

- **Polygon Approximation:** Contours may not perfectly match the shape of an object. To create accurate colliders, we approximate the contours with polygons. This process involves simplifying the contour into a series of straight-line segments, which can then be used as the collider's shape.

- **Collider Creation:** For each detected object, a collider is created within Unity. These colliders allow for physical interactions between the detected objects and other game objects within the scene.

- **Emitter and Kill Zone:**
Two key components within the Unity scene are the "Emitter" and the "Kill Zone." These components add interactive elements to the project:

  - Emitter: The "Emitter" is responsible for generating balls within the Unity scene. These balls are subject to gravity and have rigid body physics, allowing them to fall and interact with the detected objects.

  - Kill Zone: The "Kill Zone" serves as a boundary within the scene. If a ball falls out of view or reaches this zone, it is redirected back to the "Emitter." This mechanism ensures a continuous flow of balls and interaction with the detected objects.

## Getting Started
To use this project, follow these steps:

Set up your development environment with OpenCV and Unity.

- Connect your phone's camera to your PC using the "IRIUN WEBCAM" software.
- Run the Unity project on your PC.
- Point the phone's camera at a white background.
- Place objects within the camera's field of view.
- OpenCV will detect the contours of the objects, and Unity will generate colliders for them.

## Contributing
We welcome contributions to this project. If you'd like to contribute, please follow these guidelines:

- Fork the repository.
- Create a new branch for your feature or bug fix.
- Make your changes and commit them with clear, concise messages.
- Test your changes thoroughly.
- Submit a pull request, describing the problem or feature and the changes you made.

## License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.


## Contact

If you have any questions or need further assistance, feel free to contact me at [imaddjekoune@gmail.com](mailto:imaddjekoune@gmail.com).
