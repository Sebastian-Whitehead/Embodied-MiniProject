import cv2
from deepface import DeepFace
import UdpComms as U
import time

face_cascade = cv2.CascadeClassifier(cv2.data.haarcascades + 'haarcascade_frontalface_default.xml')

cap = cv2.VideoCapture(0)

if not cap.isOpened():
    cap = cv2.VideoCapture(1)
if not cap.isOpened():
    raise IOError("Cannot open webcam")

sock = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001, enableRX=True, suppressWarnings=True)

def data_Server_Send(emotion):
    sock.SendData(emotion)  # Send this string to other application

def process_video():
    while True:
        ret,frame = cap.read()
        results = DeepFace.analyze(frame, actions = ['emotion'], enforce_detection=False)

        # Save the dominant emotion of the first result as a string
        dominant_emotion = results[0]['dominant_emotion'] if isinstance(results, list) else results['dominant_emotion']
        #print(dominant_emotion)
        data_Server_Send(dominant_emotion)

        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = face_cascade.detectMultiScale(gray,1.1,4)

        for (x, y, w, h) in faces:
            cv2.rectangle(frame, (x, y), (x+w, y+h), (0,255,0), 2)

            font = cv2.FONT_HERSHEY_SIMPLEX

            cv2.putText(frame,
                        dominant_emotion,
                        (x, y-10),
                        font, 1,
                        (0, 0 , 255),
                        2,
                        cv2.LINE_4)
        cv2.imshow('Original video', frame)

        if cv2.waitKey(2) & 0xFF == ord('q'):
            break

if __name__ == "__main__":
    process_video()
