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
using Firebase.Extensions;
using Firebase.Database;

public class GoogleSignInFirebase : MonoBehaviour
{
    private string webClientId = "858479882859-d8vpciu5tehf84bvprcr3ac4vu6jinif.apps.googleusercontent.com";

    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;

    public static string userToken;
    public static string userEmail;
    public static Uri userPhotoUrl;
    public static string userName;

    public PlayerScriptable playerScriptable;

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
                else
                    Debug.Log("Could not resolve all Firebase dependencies: " + task.Result.ToString());
            }
            else
            {
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
        Debug.Log("Calling SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        playerScriptable.Reset();
        SceneManager.LoadScene("LoginScreen");
    }

    public void OnDisconnect()
    {
        Debug.Log("Calling Disconnect");
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
                    Debug.Log("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.Log("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
           Debug.Log("Canceled");
        }
        else
        {
            playerScriptable.name = task.Result.DisplayName;
            playerScriptable.email = task.Result.Email;
            playerScriptable.profileImageUrl = task.Result.ImageUrl.ToString();
            playerScriptable.token = task.Result.IdToken;

            User user = new User(playerScriptable.name, playerScriptable.email, playerScriptable.profileImageUrl, playerScriptable.token);
            string json = JsonUtility.ToJson(user);
            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(playerScriptable.token.GetHashCode().ToString()).SetRawJsonValueAsync(json).ContinueWith(task => {
                if (task.IsFaulted)
                {
                    Debug.Log("Error");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("Success");
                }
        });
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
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {
                SceneManager.LoadScene("MenuScreen");
                Debug.Log("Sign In Successful.");
            }
        });
    }

    public void OnSignInSilently()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        Debug.Log("Calling SignIn Silently");

        GoogleSignIn.DefaultInstance.SignInSilently().ContinueWith(OnAuthenticationFinished);
    }

    public void OnGamesSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = true;
        GoogleSignIn.Configuration.RequestIdToken = false;

        Debug.Log("Calling Games SignIn");

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished);
    }
}