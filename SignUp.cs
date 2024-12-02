using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class SignUp : MonoBehaviour
{
    public TMP_InputField usernameField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;
    public TMP_InputField loginUsernameField;
    public TMP_InputField loginPasswordField;

    public TextMeshProUGUI statusText;

    public Button submitButton;
    public Button loginButton;

    bool statusActive = false, statusInfinite = false;
    float totalTime, timeElapsed = 0;

    public Animator SelectLine, ContainerPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (statusActive && !statusInfinite)
        {
            timeElapsed += Time.deltaTime;
            if (timeElapsed > totalTime)
            {
                timeElapsed = 0;
                DeactivateStatus();
            }
        }
    }

    public void CallRegister()
    {
        ActivateStatus(Color.white, "Please wait...", 0f);
        StartCoroutine(Register());
    }

    public void CallLogin()
    {
        ActivateStatus(Color.white, "Please wait...", 0f);
        StartCoroutine(Login());
    }

    public void ActivateLogin()
    {
        SelectLine.SetTrigger("Login");
        ContainerPanel.SetTrigger("Login");
    }

    public void ActivateSignUp()
    {
        SelectLine.SetTrigger("SignUp");
        ContainerPanel.SetTrigger("SignUp");
    }

    IEnumerator Register()
    {
        totalTime = 0f;

        WWWForm form = new();
        form.AddField("name", usernameField.text);
        form.AddField("password", passwordField.text);

        WWW www = new WWW("https://sqlhandler.000webhostapp.com/register.php", form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("User created.");
            ActivateStatus(Color.white, "Signed up successfully!", 3f);
            ActivateLogin();
        }
        else
        {
            Debug.Log("User creation failed. " + www.text);
            ActivateStatus(Color.red, www.text, 3f);
        }
    }

    IEnumerator Login()
    {
        totalTime = 0f;

        WWWForm form = new();
        form.AddField("name", loginUsernameField.text);
        form.AddField("password", loginPasswordField.text);

        WWW www = new WWW("https://sqlhandler.000webhostapp.com/login.php", form);
        yield return www;

        if (www.text == "0")
        {
            Debug.Log("Welcome, " + loginUsernameField.text);
            ActivateStatus(Color.white, "Welcome, " + loginUsernameField.text + ".", 3f);
            UserPreferences.Instance._username = loginUsernameField.text;
            SceneManager.LoadScene(1);
        }
        else
        {
            Debug.Log("Login failed. " + www.text);
            ActivateStatus(Color.red, www.text, 3f);
        }
    }

    public void VerifyInput()
    {
        submitButton.interactable = (usernameField.text.Length >= 4 && passwordField.text.Length >= 6 && passwordField.text == confirmPasswordField.text);
        loginButton.interactable = (loginUsernameField.text != "" && loginPasswordField.text != "" && loginUsernameField.text.Trim() != null);
    }

    void ActivateStatus(Color color, string text, float time)
    {
        statusText.text = text;
        statusText.color = color;
        statusActive = true;
        totalTime = time;
        if (time == 0f) statusInfinite = true;
        else statusInfinite = false;
    }

    void DeactivateStatus()
    {
        statusText.text = "";
        statusActive = false;
    }
}
