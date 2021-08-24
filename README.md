# TalkingTimmy

## HiWi Project – Frank Fundel

## How to use

1. Create new Unity project and add avatar of choice that has ARKit BlendShapes
2. Make sure it has SkinnedMeshRenderer, Animation and Audio Source
3. Add the `animate.cs` script to it (needs SocketIO and OpenWaveParser)
4. Run the server using `python text2face.py`
5. Create tunnel using `ngrok http 8080`
6. Put tunnel url into `animate.cs`
7. Build and run unity android app
8. Type into command line and press enter

## Content

[1. The Idea](#_Toc79967239)

[2. The Model](#_Toc79967240)

[3. Training Data](#_Toc79967241)

[4. The Training](#_Toc79967242)

[5. Application](#_Toc79967247)

## 1. The Idea

Use facial tracking data from an iPhone X in combination with speech recordings to generate face movement, especially mouth movement of a 3D avatar from pure audio.

## 2. The Model

I created some 3D models in blender that can be used as avatar. The first one I tried was using images of myself and recreating me in 3D (which looked horrifying).

![](RackMultipart20210824-4-10gbpbr_html_e2fa675d2a842861.gif)

After that I watched several tutorials on how to create a friendly looking avatar and built the following one which was certainly much better.

![](RackMultipart20210824-4-10gbpbr_html_98335dcfac3667ed.png)

The next step was to create the ShapeKeys for the BlendShapes. The BlendShapes are the attributes you get from recording facial expressions using Apples Face Tracking API (ARKit). So I had to animate all 61 BlendShapes by hand into the ShapeKeys which are the shapes the model moves according to certain variables i.g. the BlendShape attributes.

![](RackMultipart20210824-4-10gbpbr_html_23821e4737c4012c.png)

The attributes are in between the range of 0 and 1 and look like this after recording (stored in a CSV):

![](RackMultipart20210824-4-10gbpbr_html_5fd7c82fec09a1ff.png)

I decided that we actually don&#39;t need all 61 attributes but just 23:

blendshapes = [&quot;jawOpen&quot;, &quot;mouthClose&quot;, &quot;mouthFunnel&quot;, &quot;mouthPucker&quot;, &quot;mouthRight&quot;, &quot;mouthLeft&quot;, &quot;mouthSmileRight&quot;, &quot;mouthSmileLeft&quot;, &quot;mouthFrownRight&quot;, &quot;mouthFrownLeft&quot;, &quot;mouthDimpleRight&quot;, &quot;mouthDimpleLeft&quot;, &quot;mouthStretchRight&quot;, &quot;mouthStretchLeft&quot;, &quot;mouthRollLower&quot;, &quot;mouthRollUpper&quot;, &quot;mouthShrugLower&quot;, &quot;mouthShrugUpper&quot;, &quot;mouthPressRight&quot;, &quot;mouthPressLeft&quot;, &quot;mouthLowerDownRight&quot;, &quot;mouthLowerDownLeft&quot;, &quot;mouthUpperUpRight&quot;]

First we need to prepare the raw data. Because the frame rate is not consistent when recording facial data, we split up the data into 1 second blocks of varying sizes using the timecodes. Then we lower the FPS to 24 FPS for all blocks e.g. take 24 evenly spaced items out of each block. Then we do some more magic so the data can be read the right way and done.

Now we want to apply the facial data to our model. There was no existing script oder plugin for Blender that could do that so I created one.

Perfect. So now we can gather some training data.

## 3. Training Data

I was reading hours stories and wikipedia articles out loud into the front camera of my phone using the Live Link Face App from Unreal Engine which records sound and facial data. After preparing (24 FPS and magic) I loaded each CSV into Blender on my model and adjusted the sound to the animation because most of the time the alignment was off. After that the data was almost ready to use, I now chopped the face and audio data into pieces. Because speech sounds do not occur in isolation and do not have a one-to-one mapping to characters, we can capture the effects of coarticulation (the articulation of one sound influencing the articulation of another). By training the network on overlapping windows of audio data that captures sound from before and after the current time index. So let&#39;s concatenate all data and slide a window over it to generate 6 frames (0.25 sec) long samples.

&quot;Depending on the data sampling rate, we recommend 26 cepstral features for 16,000 Hz and 13 cepstral features for 8,000 hz.&quot; - [https://zhuanlan.zhihu.com/p/28274740](https://zhuanlan.zhihu.com/p/28274740)

So for each audio peace we extract the 13 cepstral features (I manually lowered the sampling rate off the audio to 8000Hz). Then we select the 23 facial features and voilà. Here is a visual representation off the data (facial features on the top, audio features on the bottom):

![](RackMultipart20210824-4-10gbpbr_html_12278fef918e3059.png)

For each network we try the audio features will be the input and the facial features will be the desired output.

## 4. The Training

I had three different network models in mind which we will try:

1. First we try a complete encoder-decoder convolutional neural network, we could also try Frequency to Time convolution
2. Then we will try a complete sequence to sequence recurrent neural network, with LSTM or GRU
3. Transformer-Network

Hyperparamters to try:

- Different sizes
- Different pooling
- Dropout
- BatchNorm

###

### 1. CNN

##### Model summary

![](RackMultipart20210824-4-10gbpbr_html_87f2cefbc00029ac.png)

##### Loss

![](RackMultipart20210824-4-10gbpbr_html_79c2bc272497fb7e.png)

Performance (input audio features on top, ground truth facial features in the middle and predicted facial features on the bottom)

![](RackMultipart20210824-4-10gbpbr_html_28cab97aac021ccf.png)

The results are „Okay&quot;, we can see some understanding.

### 2. RNN

##### Model summary

![](RackMultipart20210824-4-10gbpbr_html_6b622f5c2f793ec8.png)

##### Loss

![](RackMultipart20210824-4-10gbpbr_html_c9dc8b83a40e6a3e.png)

##### Performance

![](RackMultipart20210824-4-10gbpbr_html_e8cd88d4aa375f6c.png)

Either some error lies in here or this model is just not made for this kind of problems.

### 3. Transformer Network

#####

##### Model summary

![](RackMultipart20210824-4-10gbpbr_html_c5a9a8cacbb98a34.png)

##### Loss

![](RackMultipart20210824-4-10gbpbr_html_76a4ae2818b1c9a1.gif)

##### Performance

![](RackMultipart20210824-4-10gbpbr_html_14d2b0de094bb094.gif)

Results are comparable to CNN.

### 4. Audio Transcription

One more method came to my mind: Use audio transcription and forced alignment to get the timestamps of each word, syllable or phoneme and then match them to the corresponding animation. We could either use an existing transcription of an audio or use speech recognition to align the audio.

## 5. Application

Next I copied the CSV to Animation Script and modified it to read .WAV files and run it through the saved neural network models to apply the predicted animations to the Blender model as before. I chose the CNN one because the results applied to the Blender model looked better than the Transformer ones.

Next was the last problem: display the animation on the Holofil. Luckily there is an App for the Holofil that can display 3D-Files. But when I export them from Blender they don&#39;t work, I spent days trying to resolve this problem efficiently but could not find a solution. So next best thing would be: Live stream it from the PC to the Holofil using TeamViewer or using a secondary display via HDMI. Or you could render the animation and play it but that would take too long.

So I figured out we have to make a custom Unity App for Android, because the Holofil App cannot play sound and rendering takes too long and is not movable in 3D space. I could easily set up a Unity App on android and import the Blender model with all its BlendShapes.

Now all that is needed is a Script that converts the neural network output into an animation. Luckily I found exactly that on the internet. Now you can enter text, it gets sent to a server which runs it through text-to-speech, prepares it and then runs it through the neural network to send back the keyframe data and the audio.

We could add now eye tracking and remote control support to make it more like the Holofil App.
