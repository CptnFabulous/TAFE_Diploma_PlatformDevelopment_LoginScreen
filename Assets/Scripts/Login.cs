using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;


public class Login : MonoBehaviour
{
    [Header("Login")]

    [Header("Create account")]
    public InputField usernameInput;
    public InputField emailInput;
    public InputField passwordInput;
    
    public Text createAccountToolTip;



    /*
    [Header("Account Details Check")]
    public int usernameCharacterLimit;

    bool limitNotExceeded;
    bool passwordsMatch;
    bool passwordHasNumbers;

    string errorMessage;
    public Text errorLog;
    */

    [Header("Forgot Password")]


    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */

    IEnumerator CreateUser(string username, string email, string password)
    {
        string createUserURL = "http://localhost/nsirpg/insertuser.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("email", email);
        form.AddField("password", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(createUserURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.error);
    }

    /*
    public void CreateNewUser(string username, string email, string password, string confirmPassword)
    {
        
        if (username.Length <= usernameCharacterLimit)
        {
            limitNotExceeded = true;
        }

        if (password == confirmPassword)
        {
            passwordsMatch = true;
        }

        for (int i = 0; i < 10; i++)
        {
            if (username.Contains(i.ToString()))
            {
                passwordHasNumbers = true;
            }
            i++;
        }

        if (limitNotExceeded == true && passwordsMatch == true && passwordHasNumbers == true)
        {
            StartCoroutine(CreateUser(username, email, password));
        }
        else
        {
            errorMessage = null;

            if (limitNotExceeded == false)
            {
                errorMessage += "Username character limit exceeded!";
                errorMessage += "\n";
            }
            if (passwordsMatch == false)
            {
                errorMessage += "Passwords do not match!";
                errorMessage += "\n";
            }
            if (passwordHasNumbers == false)
            {
                errorMessage += "Password does not contain numbers!";
                errorMessage += "\n";
            }

            errorLog.text = errorMessage;
        }
    }
    */

    public void CreateNewUser()
    {
        StartCoroutine(CreateUser(usernameInput.text, emailInput.text, passwordInput.text));
    }





    IEnumerator UserLogin(string username, string password)
    {
        string createUserURL = "http://localhost/nsirpg/login.php";
        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("password", password);
        UnityWebRequest webRequest = UnityWebRequest.Post(createUserURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
        if (webRequest.downloadHandler.text == "Login Successful")
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            createAccountToolTip.text = webRequest.downloadHandler.text;
        }
    }

    public void SubmitLogin()
    {
       // StartCoroutine(UserLogin(username, password));
    }
}
