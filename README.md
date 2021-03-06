# TalkingTimmy

## HiWi Project – Frank Fundel

![MainScreen](https://github.com/FrankFundel/TalkingTimmy/blob/main/Doc/MainScreen.jpg)
![MenuScreen](https://github.com/FrankFundel/TalkingTimmy/blob/main/Doc/MenuScreen.jpg)
![Python](https://github.com/FrankFundel/TalkingTimmy/blob/main/Doc/Python.png)

[APK download 2.0.0](https://drive.google.com/file/d/16-DRuZtffvYEOVU06io-kpq6Rw21lNx1/view?usp=sharing)
+ Supports face tracking
+ Supports moving model using touchscreen
+ Supports moving model using remote joystick
+ SSL transmission

[APK download 2.0.0 for Tablet](https://drive.google.com/file/d/1BIILN9RyShudK72pWfC0JDujjKfVxrF9/view?usp=sharing)
+ Without face tracking
+ Supports moving model using touchscreen
+ Supports moving model using remote joystick
+ Without SSL transmission

Google Play Store: [1.0.2](https://play.google.com/store/apps/details?id=com.Hadros.TalkingTimmy)

# How to use
1. Start `python text2face_gui.py`
2. Start App
3. Type in ID and choose avatar
4. Press start

# How to use Rasa & AIML
1. Run custom rasa server and type in NLU address e.g. http://localhost:5005/model/parse
2. Create AIML file and type in path
3. Type in command/question/... in chat field, press "Talk" to submit

## How to use without gui on custom unity project

1. Create new Unity project and add avatar of choice that has ARKit BlendShapes
2. Make sure it has SkinnedMeshRenderer, Animation and Audio Source
3. Add the `animate.cs` script to it (needs SocketIO and OpenWaveParser)
4. Run the server using `python text2face.py`
5. Create tunnel using `ngrok http 8080`
6. Put tunnel url into `animate.cs`
7. Build and run unity android app
8. Type into command line and press enter to talk

Documentation: [Doc](https://drive.google.com/file/d/1Idtei_umpMKyik-906pI7h4622yhiQZe/view?usp=sharing)
