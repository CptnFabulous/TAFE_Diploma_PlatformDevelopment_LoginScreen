using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

#region EMAIL STUFF
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
#endregion

public class Login : MonoBehaviour
{
    [Header("Login")]
    public InputField loginEmail;
    public InputField loginPassword;
    public Text loginTooltip;

    [Header("Create account")]
    public InputField newUsername;
    public InputField newEmail;
    public InputField newPassword;
    public InputField confirmNewPassword;
    public Text newAccountToolTip;

    [Header("Change password")]
    public InputField changeemail;
    public InputField oldPassword;
    public InputField changePassword;
    public InputField confirmChangePassword;
    public Text changePasswordTooltip;

    [Header("Reset password")]
    public InputField resetEmail;
    public Text resetPasswordTooltip;


    [Header("Send Email")]
    public int portNumber = 8080; //80 25
    public int codeLength = 6;
    public bool caseSensitiveCode = true;

    
    [Header("Account Details Check")]
    public int usernameCharacterLimit;
    public Text errorLog;
    

    #region Create account
    public void CreateNewUser()
    {
        //StartCoroutine(CreateUser(newUsername.text, newEmail.text, newPassword.text));

        
        if (ValidatePassword(newPassword.text, confirmNewPassword.text) && newUsername.text.Length <= usernameCharacterLimit)
        {
            StartCoroutine(CreateUser(newUsername.text, newEmail.text, newPassword.text));
        }
        else
        {
            /*
            string errorMessage = "";

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
            */
        }
        
    }

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

    
    bool ValidatePassword(string password, string confirmPassword)
    {
        bool passwordMatches = false;
        bool passwordHasNumbers = false;
        bool passwordHasLowerCaseLetters = false;
        bool passwordHasUpperCaseLetters = false;

        if (password == confirmPassword)
        {
            passwordMatches = true;
        }

        string numbers = "0123456789";
        for(int i = 0; i < numbers.Length; i++)
        {
            if (password.Contains(numbers[i]))
            {
                passwordHasNumbers = true;
            }
        }

        string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        for (int i = 0; i < lowerCase.Length; i++)
        {
            if (password.Contains(lowerCase[i]))
            {
                passwordHasLowerCaseLetters = true;
            }
        }

        string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < upperCase.Length; i++)
        {
            if (password.Contains(upperCase[i]))
            {
                passwordHasUpperCaseLetters = true;
            }
        }
        
        if (passwordMatches == true && passwordHasNumbers == true && passwordHasLowerCaseLetters == true && passwordHasUpperCaseLetters == true)
        {
            return true;
        }
        return false;
    }
    
    #endregion

    #region Login
    public void SubmitLogin()
    {
        //StartCoroutine(UserLogin(username, newpassword));
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
            loginTooltip.text = webRequest.downloadHandler.text;
        }
    }
    #endregion

    #region Reset password
    public void InputChangePassword()
    {

    }

    public IEnumerator UpdatePassword(string username, string newPassword)
    {
        string updatePasswordURL = "http://localhost/nsirpg/updatepassword.php";
        WWWForm form = new WWWForm();
        form.AddField("username_Post", username);
        form.AddField("password_Post", newPassword);
        UnityWebRequest webRequest = UnityWebRequest.Post(updatePasswordURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
    }
    #endregion Forgot password


    #region Forgot password
    public void InputResetEmail()
    {
        StartCoroutine(CheckEmail(resetEmail.text));
    }

    public IEnumerator CheckEmail(string email)
    {
        string checkEmailURL = "http://localhost/nsirpg/checkemail.php";
        WWWForm form = new WWWForm();
        form.AddField("email_Post", email);
        UnityWebRequest webRequest = UnityWebRequest.Post(checkEmailURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);

        if (webRequest.downloadHandler.text != "User not found")
        {
            SendEmail(resetEmail.text, webRequest.downloadHandler.text);
        }
        else
        {
            //createAccountToolTip.text = webRequest.downloadHandler.text; Add tooltip to say user not found
            print(webRequest.downloadHandler.text);

        }
    }

    public void SendEmail(string _email, string _username)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("sqlunityclasssydney@gmail.com");
        mail.To.Add(_email);
        mail.Subject = "NSIRPG Password Reset";
        mail.Body = "Hello " + _username + ",\nReset your password using this code: " + GenerateCode(codeLength, caseSensitiveCode);
        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = portNumber;
        smtpServer.Credentials = new NetworkCredential("sqlunityclasssydney@gmail.com", "sqlpassword") as ICredentialsByHost; // MAKE PASSWORD AND EMAIL INTO PUBLIC VARIABLES
        smtpServer.EnableSsl = true;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors) { return true; };
        smtpServer.Send(mail);
        Debug.Log("Sending email");
    }

    string GenerateCode(int length, bool caseSensitive)
    {
        string[] digits = new string[length];
        string finalCode = "";
        for (int i = 0; i < length; i++)
        {
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if (caseSensitive == true)
            {
                characters += "abcdefghijklmnopqrstuvwxyz";
            }
            //digits[i] = characters[UnityEngine.Random.Range(0, characters.Length)].ToString();
            digits[i] = characters[Mathf.RoundToInt(UnityEngine.Random.Range(0, characters.Length))].ToString();
            finalCode += digits[i];
        }
        return finalCode;
    }
    #endregion


}