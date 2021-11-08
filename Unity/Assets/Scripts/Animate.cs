using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Socket.Quobject.SocketIoClientDotNet.Client;
using System;
using System.IO;
using System.Text;
using static OpenWavParser;
using UnityEngine.SceneManagement;
using System.Globalization;
using UnityEngine.InputSystem;

public class Animate : MonoBehaviour
{
  private Controller controls;
  private Vector2 rotate;
  private Vector2 rotate2;

  private Animation animation;
  private AudioSource audioSource;
  private QSocket socket;

  static volatile bool audio_q = false;
  static volatile bool face_q = false;

  static byte[] audio;
  static string face;

  private GameObject avatar1;
  private GameObject avatar2;
  private GameObject avatar;
  private GameObject avatarFace;

  private void Start() {
    avatar1 = GameObject.Find("Avatar1");
    avatar2 = GameObject.Find("Avatar2");

    if(StaticClass.Avatar == 1) {
      avatar = avatar1;
      avatarFace = GameObject.Find("Wolf3D.Avatar_Renderer_Head");
      avatar2.SetActive(false);
    } else {
      avatar = avatar2;
      avatarFace = avatar2;
      avatar1.SetActive(false);
    }

    animation = avatarFace.GetComponent<Animation>();
    audioSource = avatarFace.GetComponent<AudioSource>();

    string filePath = Application.persistentDataPath + "/output.wav";
    
    string address = StaticClass.Address;
    Debug.Log("Connecting to " + address);

    try {
      socket = IO.Socket("https://" + address + ".ngrok.io");
    } catch(Exception e) {
      SceneManager.LoadScene("MenuScene");
      return;
    }

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

      if(OpenWavParser.IsWAVFile(decodedBytes)) {
        Debug.Log("This is a valid PCM WAV file!!");
        audio = decodedBytes;
        audio_q = true;
      } else {
        Debug.Log("This is not a PCM WAV file.");
      }
    });
  }

  private void OnDestroy() {
    if(socket != null) socket.Disconnect();
  }

  private void Awake() {
    controls = new Controller();
    controls.ControlMap.Rotate.performed += ctx => rotate = ctx.ReadValue<Vector2>();
    controls.ControlMap.Rotate.canceled += ctx => rotate = Vector2.zero;
    controls.ControlMap.Rotate2.performed += ctx => rotate2 = ctx.ReadValue<Vector2>();
    controls.ControlMap.Rotate2.canceled += ctx => rotate2 = Vector2.zero;
  }

  private void OnEnable() {
    controls.ControlMap.Enable();
  }

  private void Update() {
    Vector2 r = new Vector2(rotate.y, -rotate.x) * 5f * Time.deltaTime;
    avatar.transform.Rotate(r, Space.World);

    if(rotate.x == 0 && rotate.y == 0) {
      Vector2 r2 = new Vector2(-rotate2.x, -rotate2.y) * 20f * Time.deltaTime;
      avatar.transform.Rotate(r2, Space.World);
    }

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
    Keyframe[,] ks = new Keyframe[blendshapes.Length, lines.Length];

    for(int t = 0; t < lines.Length; t++) {
      string[] col = lines[t].Split(',');
      for(int i = 0; i < col.Length; i++) {
        float val = float.Parse(col[i], CultureInfo.InvariantCulture);
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