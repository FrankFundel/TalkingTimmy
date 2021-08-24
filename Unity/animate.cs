using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Socket.Quobject.SocketIoClientDotNet.Client;
using System;
using System.IO;
using System.Text;
using static OpenWavParser;

public class animate : MonoBehaviour
{
  private Animation animation;
  private AudioSource audioSource;
  private QSocket socket;

  static volatile bool audio_q = false;
  static volatile bool face_q = false;

  static byte[] audio;
  static string face;

  private void Start() {
    animation = GetComponent<Animation>();
    audioSource = GetComponent<AudioSource>();

    string filePath = Application.persistentDataPath + "/output.wav";
    
    Debug.Log("Start");
    socket = IO.Socket("https://789b-134-60-112-69.ngrok.io"); //IO.Socket("http://localhost:8080");

    socket.On(QSocket.EVENT_CONNECT, () => {
      Debug.Log("Connected");
      socket.Emit("chat", "test");
    });

    socket.On("chat", data => {
      Debug.Log("chat: " + data);
    });
    
    socket.On("face", data => {
      Debug.Log("Face received!");
      face = (string)data;
      face_q = true;
    });
    
    socket.On("audio", data => {
      byte[] decodedBytes = (byte[])(data);
      Debug.Log("Audio received: " + decodedBytes.Length.ToString());

      if(OpenWavParser.IsCompatible(decodedBytes)) {
        Debug.Log("This is a valid PCM WAV file!!");
        audio = decodedBytes;
        audio_q = true;
      } else {
        Debug.Log("This is not a PCM WAV file.");
      }
    });
  }

  private void OnDestroy() {
    socket.Disconnect();
  }

  private void Update() {
    if(audio_q && face_q) {
      audio_q = false;
      face_q = false;

      try {
        audioSource.clip = OpenWavParser.ByteArrayToAudioClip(audio);
        AnimationClip clip = renderAnimation(face);
        animation.AddClip(clip, clip.name);

        Debug.Log("Playing...");
        animation.Play(clip.name);
        audioSource.Play();
      } catch (Exception e) {
        Debug.Log(e);
      }
    }
  }

  private AnimationClip renderAnimation(string data) {
    string[] blendshapes = {"jawOpen", "mouthClose", "mouthFunnel", "mouthPucker", "mouthRight", "mouthLeft", "mouthSmileRight", "mouthSmileLeft", "mouthFrownRight", "mouthFrownLeft", "mouthDimpleRight", "mouthDimpleLeft", "mouthStretchRight", "mouthStretchLeft", "mouthRollLower", "mouthRollUpper", "mouthShrugLower", "mouthShrugUpper", "mouthPressRight", "mouthPressLeft", "mouthLowerDownRight", "mouthLowerDownLeft", "mouthUpperUpRight"};
    string[] lines = data.Split("\n"[0]);
    float fps = 24.0f;
    AnimationCurve anim;
    Keyframe[,] ks = new Keyframe[blendshapes.Length, lines.Length];

    for(int t = 0; t < lines.Length; t++) {
      string[] col = lines[t].Split(',');
      for(int i = 0; i < col.Length; i++) {
        float val = float.Parse(col[i].Replace('.', ','));
        ks[i, t] = new Keyframe((float)t / fps, val * 100);
      }
    }

    AnimationClip clip = new AnimationClip();
    clip.legacy = true;

    for(int b = 0; b < blendshapes.Length; b++) {
      Keyframe[] keyframes = new Keyframe[lines.Length + 1];
      for(int n = 0; n < lines.Length; n++) keyframes[n] = ks[b, n];
      keyframes[lines.Length] = new Keyframe((float)lines.Length / fps, 0.0f);
      AnimationCurve curve = new AnimationCurve(keyframes);
      clip.SetCurve("", typeof(SkinnedMeshRenderer), "blendShape." + blendshapes[b], curve);
    }

    return clip;
  }
}