import os
import csv
import json

import numpy as np
import tensorflow as tf
from scipy.io import wavfile
from python_speech_features import mfcc

import eventlet
import socketio
import threading
import PySimpleGUI as sg

from google.cloud import texttospeech
import base64
from pyngrok import ngrok
import requests
import aiml

sio = socketio.Server()
app = socketio.WSGIApp(sio)

rasa = "http://localhost:5005/model/parse"
aimlfile = ""
ai = True
rule = False

text = ""
chat = ""
file = ""

@sio.event
def connect(sid, environ):
  print('connect', sid)

@sio.event
def message(sid, data):
  print('message: ' + data)
  sio.emit('message', data)

@sio.event
def disconnect(sid):
  print('disconnect', sid)

def wav2face(filepath):
  audio_fps = 8000
  audio_sample_size = int(audio_fps / 4) # 250ms

  def slideWindow(a, size, step):
    b = []
    i = 0
    pos = 0
    while pos + size < len(a):
      pos = int(i  * step)
      b.append(a[pos : pos + size])
      i+=1
    return b

  def getAudio(path, size = audio_sample_size, step = 1000):
    out = []
    sr, y = wavfile.read(path)
    samples = slideWindow(y, size, step)
    for sample in samples:
        out.append(mfcc(sample, audio_fps))
    print(path, sr, len(out))
    return out[:-1] # last one is not full

  model = tf.keras.models.load_model('AI\\speech2face_cnn')

  audio = getAudio(filepath, step=audio_sample_size)
  input = np.asarray(audio)
  input = (input - np.min(input)) / np.ptp(input)

  decoded = model.predict(np.expand_dims(input, axis=3))
  keyframes = np.concatenate(decoded)
        
  blendshapes = ["jawOpen", "mouthClose", "mouthFunnel", "mouthPucker", "mouthRight", "mouthLeft", "mouthSmileRight", "mouthSmileLeft", "mouthFrownRight", "mouthFrownLeft", "mouthDimpleRight", "mouthDimpleLeft", "mouthStretchRight", "mouthStretchLeft", "mouthRollLower", "mouthRollUpper", "mouthShrugLower", "mouthShrugUpper", "mouthPressRight", "mouthPressLeft", "mouthLowerDownRight", "mouthLowerDownLeft", "mouthUpperUpRight"]
          
  with open(filepath + "-frames.csv", 'w', newline='') as csvfile:
    writer = csv.writer(csvfile)
    writer.writerow(blendshapes)
    for row in keyframes:
      writer.writerow(row)

  return keyframes

def text2speech(text):
  os.environ["GOOGLE_APPLICATION_CREDENTIALS"] = "key.json"

  client = texttospeech.TextToSpeechClient()
  synthesis_input = texttospeech.SynthesisInput(text=text)
  
  voice = texttospeech.VoiceSelectionParams(
    language_code="de-DE", ssml_gender=texttospeech.SsmlVoiceGender.MALE
  )

  audio_config = texttospeech.AudioConfig(
    audio_encoding=texttospeech.AudioEncoding.LINEAR16,
    speaking_rate=0.8
  )
  audio_config_low = texttospeech.AudioConfig(
    audio_encoding=texttospeech.AudioEncoding.LINEAR16,
    sample_rate_hertz=8000,
    speaking_rate=0.8
  )

  response = client.synthesize_speech(
    input=synthesis_input, voice=voice, audio_config=audio_config
  )

  with open("output.wav", "wb") as out:
    out.write(response.audio_content)
    print('Audio content written to file "output.wav"')
  
  response_low = client.synthesize_speech(
    input=synthesis_input, voice=voice, audio_config=audio_config_low
  )

  with open("output_low.wav", "wb") as out:
    out.write(response_low.audio_content)
    print('Audio content written to file "output_low.wav"')

  return ("output_low.wav", "output.wav")

def readfile(path):
  with open(path, "rb") as data:
    return data.read()

def mat2string(mat):
  text = ""
  for line in mat:
    text += ','.join(['%.5f' % num for num in line]) + "\n"
  return text[:-1]

def worker():
  global ai
  global rule
  global rasa
  global aimlfile

  global text
  global chat
  global file

  while True:
    if (text):
      print(text)
      sio.emit("message", text)

      audio, audio_high = text2speech(text)
      face = wav2face(audio)
      face_enc = mat2string(face)
      audio_enc = readfile(audio_high)

      print("Sending face and audio data...")
      sio.emit('face', face_enc)
      sio.emit('audio', audio_enc)
      text = ""
    elif (chat):
      # Send to RASA
      payload = {'text': chat}
      headers = {'content-type': 'application/json'}
      r = requests.post(rasa, json=payload, headers=headers).json()
      print(r)

      # Interpret RASA output with AIML
      kernel = aiml.Kernel()
      kernel.learn(aimlfile)
      kernel.respond("load aiml b")
      entities = ""
      for entity in r["entities"]:
        entities += " " + entity["value"]
      text = kernel.respond(r["intent"]["name"] + entities)

      # Use AIML output -> set text to output
      chat = ""
    elif (file):
      face = wav2face(file)
      face_enc = mat2string(face)
      audio_enc = readfile(file)

      print("Sending face and audio data...")
      sio.emit('face', face_enc)
      sio.emit('audio', audio_enc)
      file = ""
    sio.sleep(1)

def server():
  sio.start_background_task(worker) #sending thread
  eventlet.wsgi.server(eventlet.listen(('localhost', 8080)), app)

threading.Thread(target=server, daemon=True).start()
tunnel = ngrok.connect(8080, bind_tls=True)
print(tunnel.public_url)

layout = [
  [sg.Text("Connect to: " + tunnel.public_url[8:-9])],
  [sg.Radio("Text", "Radio", True, key="-TEXT-RADIO-")],
  [sg.In(key="-TEXT-")],
  [sg.Radio("WAV file (16 Bit, 8000Hz)", "Radio", False, key="-FILE-RADIO-")],
  [
    sg.In(enable_events=True, key="-FILE-"),
    sg.FileBrowse(),
  ],
  [sg.Radio("Chat", "Radio", False, key="-CHAT-RADIO-")],
  [sg.In(key="-CHAT-")],
  [sg.Text("Rasa address")],
  [sg.In("http://localhost:5005/model/parse", key="-RASA-")],
  [sg.Text("AIML file")],
  [
    sg.In(enable_events=True, key="-AIML-FILE-"),
    sg.FileBrowse(),
  ],
  [
    sg.Radio("AI", "Radio2", True, key="-AI-"),
    sg.Radio("Rule based", "Radio2", False, key="-RULE-")
  ],
  [sg.Button("Talk")]
]

window = sg.Window("TalkingTimmy", layout)

while True:
  event, values = window.read()
  
  if event == "Exit" or event == sg.WIN_CLOSED:
    break
  if event == "Talk":
    ai = values["-AI-"]
    rule = values["-RULE-"]
    rasa = values["-RASA-"]
    aimlfile = values["-AIML-FILE-"]

    if (values["-TEXT-RADIO-"]):
      text = values["-TEXT-"]
    elif (values["-CHAT-RADIO-"]):
      chat = values["-CHAT-"]
    elif(values["-FILE-RADIO-"]):
      file = values["-FILE-"]

window.close()