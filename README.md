# Unity and Python Emotion Detection Chatbot

This project enables real-time communication between a Python program that detects user emotions using a webcam and a Unity application functioning as an AI chatbot. The detected emotion is appended to the chatbot's messages to provide more context and personalize the conversation.

## Purpose

The main goal of integrating Unity and Python using UDP communication is to create an interactive chatbot experience that responds dynamically to the user's emotional state. By combining emotion detection from Python with chatbot functionality in Unity, we aim to build a more engaging and personalized user experience.

## Functionality

The Unity application and the Python script communicate through a UDP client-server setup described in the provided scripts. The Python script analyzes webcam footage to detect the user's emotions, while the Unity application acts as an AI chatbot, responding to user messages and incorporating the detected emotion into its responses.

## Setup

To run the project:

1. **Python Environment Setup**:
   - Ensure you have Python installed on your system.
   - Install the required Python libraries: OpenCV and DeepFace.
   - Run the main Python script (`main.py`) in a Python environment.

2. **Unity Environment Setup**:
   - Install any Language Learning Model (LLM) into the LLM object in the Unity scene. (e.g., Mistral 7b is recommended)
   - Open the Unity project and run the scene named "scene."

3. **Connection**:
   - Upon running, the Python script and the Unity environment should connect automatically and begin sending information.

## Testing

To verify the communication between Unity and Python:

1. Check the Unity console for errors. Lack of connection will be indicated by error messages.
2. Send a message to the chatbot in Unity. The Unity console should display a message in the format `<emotion>`, indicating the most up-to-date emotion detected.

## Troubleshooting

If problems arise:

- **Python Script Issues**: Check the Python script's console. Ensure it only draws a face around the user's face. If other faces are detected, adjust the environment or lighting. If no face is detected, ensure adequate lighting or adjust the webcam selection in the script.
- **Connection Refusal**: Ensure that the ports referenced in both the Unity environment and the Python script are set to the same values.

## Credits

This project builds upon the work of:

- Youssef Elashry's implementation of two-way communication between Python 3 and Unity (C#).
- Undreamai's LLM Unity library.
