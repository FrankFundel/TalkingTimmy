using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[ExecuteInEditMode]
public class DepthIllusion_ARFace_Binder : MonoBehaviour
{
    public ARSessionOrigin origin;
    public GameObject projectionCam;

    void Update() {
      // Face space to Content space mapping
      //from
      // x: +-0.2
      // y: +-0.2
      // z: 0.2-0.5
      //to
      // x: +-4.5
      // y: +-4.5
      // z: -5 - -10

      ARFaceManager faceManager = origin.GetComponent<ARFaceManager>();
      if(faceManager != null) {
        foreach (ARFace face in faceManager.trackables)
        {
          Vector3 remappedPos = new Vector3(
            -face.transform.position.x.Remap(0.2f, -0.2f, 5.0f, -5.0f),
            face.transform.position.y.Remap(0.13f, -0.13f, 5.0f, -5.0f),
            face.transform.position.z.Remap(0.2f, 0.5f, -10.0f, -30.0f)
          );

          projectionCam.transform.position = remappedPos;
        }
      }
    }
}
