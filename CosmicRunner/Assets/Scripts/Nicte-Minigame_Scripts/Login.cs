using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public TMP_InputField User;
    public TMP_InputField Password;

    string username, password;
    public void Awake()
    {
        PlayerPrefs.DeleteKey("UserId");
    }
    public void GetUsername()
    {
        username = User.text;
        Debug.Log("User entered: " + username);
    }

    public void GetPassword()
    {
        password = Password.text;
        Debug.Log("Password entered: " + password);
    }

    public void LoginButton()
    {
        //PlayerPrefs.SetInt("UserId", 5);
        PlayerPrefs.SetInt("UserId", 1);
        SceneManager.LoadScene("GameSelectScene");
    }

}
