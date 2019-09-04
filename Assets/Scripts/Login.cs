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
    [Header("Screens")]
    public GameObject loginScreen;
    public GameObject createAccountScreen;
    //public GameObject changePasswordScreen;
    public GameObject forgotPasswordScreen;

    [Header("Login")]
    public InputField loginUsername;
    public InputField loginPassword;
    public Text loginErrorLog;

    [Header("Create account")]
    public int usernameCharacterLimit;
    public InputField createUsername;
    public InputField createEmail;
    public InputField createPassword;
    public InputField confirmCreatePassword;
    public Text createAccountErrorLog;
    /*
    [Header("Change password")]
    public InputField changeUsername;
    public InputField oldPassword;
    public InputField changePassword;
    public InputField confirmChangePassword;
    public Text changePasswordErrorLog;
    */
    [Header("Forgot password")]
    public InputField resetEmail;
    public InputField codeInput;
    public InputField resetPassword;
    public InputField confirmResetPassword;
    public Text resetPasswordErrorLog;
    public int portNumber = 8080; //80 25
    public int codeLength = 6;
    public bool caseSensitiveCode = true;
    string usernameForResetting;
    string validationCode;

    string passwordError = "";
   
    public void DisplayScreen(GameObject screen)
    {
        loginScreen.SetActive(false);
        createAccountScreen.SetActive(false);
        //changePasswordScreen.SetActive(false);
        forgotPasswordScreen.SetActive(false);

        screen.SetActive(true);
    }

    bool ValidatePassword(string password, string confirmPassword)
    {
        passwordError = "";

        bool passwordMatches = false;
        bool passwordHasNumbers = false;
        bool passwordHasLowerCaseLetters = false;
        bool passwordHasUpperCaseLetters = false;

        if (password == confirmPassword)
        {
            passwordMatches = true;
            print("Passwords match.");
        }

        string numbers = "0123456789";
        for (int i = 0; i < numbers.Length; i++)
        {
            if (password.Contains(numbers[i]))
            {
                passwordHasNumbers = true;
            }
        }

        if (passwordHasNumbers == true)
        {
            print("Password contains numbers.");
        }

        string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        for (int i = 0; i < lowerCase.Length; i++)
        {
            if (password.Contains(lowerCase[i]))
            {
                passwordHasLowerCaseLetters = true;
            }
        }

        if (passwordHasLowerCaseLetters == true)
        {
            print("Password contains lower case letters.");
        }

        string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < upperCase.Length; i++)
        {
            if (password.Contains(upperCase[i]))
            {
                passwordHasUpperCaseLetters = true;
            }
        }

        if (passwordHasUpperCaseLetters == true)
        {
            print("Password contains upper case letters.");
        }

        if (passwordMatches == true && passwordHasNumbers == true && passwordHasLowerCaseLetters == true && passwordHasUpperCaseLetters == true)
        {
            return true;
        }

        // Code past this point only runs if the bool does not return true.
        if (passwordMatches == false)
        {
            passwordError += "Passwords do not match!";
            passwordError += "\n";
        }
        if (passwordHasNumbers == false)
        {
            passwordError += "Password does not contain numbers!";
            passwordError += "\n";
        }
        if (passwordHasLowerCaseLetters == false)
        {
            passwordError += "Password does not contain lower case letters!";
            passwordError += "\n";
        }
        if (passwordHasUpperCaseLetters == false)
        {
            passwordError += "Password does not contain upper case letters!";
            passwordError += "\n";
        }
        return false;
    }

    #region Create account
    public void CreateNewUser()
    {
        if (ValidatePassword(createPassword.text, confirmCreatePassword.text) && createUsername.text.Length <= usernameCharacterLimit)
        {
            StartCoroutine(CreateUser(createUsername.text, createEmail.text, createPassword.text));
        }
        else
        {
            string errorMessage = "";
            if (ValidatePassword(createPassword.text, confirmCreatePassword.text) == false)
            {
                errorMessage += passwordError;
            }
            if (createUsername.text.Length > usernameCharacterLimit)
            {
                errorMessage += "Username character limit exceeded!";
                errorMessage += "\n";
            }
            createAccountErrorLog.text = errorMessage;
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
        Debug.Log(webRequest.downloadHandler.text);
        createAccountErrorLog.text = webRequest.downloadHandler.text;
        
        
    }
    #endregion

    #region Login
    public void SubmitLogin()
    {
        StartCoroutine(UserLogin(loginUsername.text, loginPassword.text));
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
        if (webRequest.downloadHandler.text == "Login successful")
        {
            print("Loading scene");
            SceneManager.LoadScene(1);
        }
        else
        {
            loginErrorLog.text = webRequest.downloadHandler.text;
        }
    }
    #endregion

    #region Reset password
    public void InputChangePassword()
    {
        /*
        bool passwordIsValid = ValidatePassword(changePassword.text, confirmChangePassword.text);
        if (passwordIsValid == true)
        {
            StartCoroutine(UpdatePassword(changeUsername.text, resetPassword.text));
        }
        else
        {
            string passwordResetError = passwordError;
            if (codeInput.text != validationCode)
            {
                passwordResetError += "Verification code does not match the one sent to the email.";
            }
            resetPasswordErrorLog.text = passwordResetError;
        }
        */
    }

    public IEnumerator UpdatePassword(string username, string createPassword)
    {
        string updatePasswordURL = "http://localhost/nsirpg/updatepassword.php";
        WWWForm form = new WWWForm();
        form.AddField("username_Post", username);
        form.AddField("password_Post", createPassword);
        UnityWebRequest webRequest = UnityWebRequest.Post(updatePasswordURL, form);
        yield return webRequest.SendWebRequest();
        Debug.Log(webRequest.downloadHandler.text);
    }
    #endregion

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
            usernameForResetting = webRequest.downloadHandler.text;
            print(usernameForResetting);
            SendResetEmail(resetEmail.text, usernameForResetting);
        }
        else
        {
            //createAccountToolTip.text = webRequest.downloadHandler.text; Add tooltip to say user not found
            print(webRequest.downloadHandler.text);
        }
    }

    public void SendResetEmail(string _email, string _username)
    {
        validationCode = GenerateCode(codeLength, caseSensitiveCode);
        print(validationCode);
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress("sqlunityclasssydney@gmail.com");
        mail.To.Add(_email);
        mail.Subject = "NSIRPG Password Reset";
        mail.Body = "Hello, " + _username + ",\nReset your password using this code: " + validationCode;
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

    public void CheckCode()
    {
        bool passwordIsValid = ValidatePassword(resetPassword.text, confirmResetPassword.text);
        if (passwordIsValid == true && codeInput.text == validationCode)
        {
            StartCoroutine(UpdatePassword(usernameForResetting, resetPassword.text));
        }
        else
        {
            string passwordResetError = passwordError;
            if (codeInput.text != validationCode)
            {
                passwordResetError += "Verification code does not match the one sent to the email.";
            }
            resetPasswordErrorLog.text = passwordResetError;
        }
    }
    #endregion
}