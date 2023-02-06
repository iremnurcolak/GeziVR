using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using Google;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class GoogleSignInFirebase : MonoBehaviour
{
    private string webClientId = "858479882859-d8vpciu5tehf84bvprcr3ac4vu6jinif.apps.googleusercontent.com";

    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    //public static string userToken;
    //public static string userEmail;
    //public static Uri userPhotoUrl;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true};
        CheckFirebaseDependencies(); 
    }

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                if (task.Result == DependencyStatus.Available)
                    auth = FirebaseAuth.DefaultInstance;
                else{
                    //AddToInformation("Could not resolve all Firebase dependencies: " + task.Result.ToString());
                    Debug.Log("Could not resolve all Firebase dependencies: " + task.Result.ToString());
                }
                   
            }
            else
            {
                //AddToInformation("Dependency check was not completed. Error : " + task.Exception.Message);
                Debug.Log("Dependency check was not completed. Error : " + task.Exception.Message);
            }
        });
    }

    public void SignInWithGoogle() { OnSignIn(); }
    public void SignOutFromGoogle() { OnSignOut(); }

    private void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        //AddToInformation("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private  void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        SceneManager.LoadScene("LoginScreen");
    }

    public void OnDisconnect()
    {
        //AddToInformation("Calling Disconnect");
        GoogleSignIn.DefaultInstance.Disconnect();
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    //AddToInformation("Got Error: " + error.Status + " " + error.Message);
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    //AddToInformation("Got Unexpected Exception?!?" + task.Exception);
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            //AddToInformation("Canceled");
            Debug.Log("Canceled");
        }
        else
        {
            //AddToInformation("Welcome: " + task.Result.DisplayName + "!");
            //AddToInformation("Email = " + task.Result.Email);
            //userToken = task.Result.IdToken;
            //userEmail = task.Result.Email;
            //userPhotoUrl = task.Result.ImageUrl;
            PlayerPrefs.SetString("name", task.Result.DisplayName);
            PlayerPrefs.SetString("email", task.Result.Email);
            PlayerPrefs.SetString("token", task.Result.IdToken);
            PlayerPrefs.SetString("photoUrl", task.Result.ImageUrl.ToString());
            SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                {
                    //AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
                }
                    //AddToInformation("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                SceneManager.LoadScene("MenuScreen");
                //AddToInformation("Sign In Successful.");
                Debug.Log("Sign In Successful.");
            }
        });
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        //AddToInformation("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        //AddToInformation("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    //public void AddToInformation(string str) { infoText.text += "\n" + str; }
}