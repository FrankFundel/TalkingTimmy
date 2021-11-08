using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuHandler : MonoBehaviour {
  GameObject glowImage1;
  GameObject glowImage2;
  GameObject addressInput;

  public void Start() {
    StaticClass.Avatar = 1;
    glowImage1 = GameObject.Find("GlowImage");
    glowImage2 = GameObject.Find("GlowImage2");
    addressInput = GameObject.Find("AddressInput");
    setMenu1();
  }

  public void setMenu1() {
    StaticClass.Avatar = 1;
    glowImage1.SetActive(true);
    glowImage2.SetActive(false);
  }

  public void setMenu2() {
    StaticClass.Avatar = 2;
    glowImage2.SetActive(true);
    glowImage1.SetActive(false);
  }

  public void startButton() {
    StaticClass.Address = addressInput.GetComponent<TMP_InputField>().text;
    SceneManager.LoadScene("MainScene");
  }
}
