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


    public PlayerScriptable playerScriptable;

    private void Awake()
    {
        configuration = new GoogleSignInConfiguration { WebClientId = webClientId, RequestEmail = true, RequestIdToken = true};
        CheckFirebaseDependencies(); 
        
    }

    void Start()
    {
    }

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
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

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnAuthenticationFinished);
    }

    private void OnSignOut()
    {
        GoogleSignIn.DefaultInstance.SignOut();
        playerScriptable.Reset();
        PlayerPrefs.DeleteAll();
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
                
                PlayerPrefs.SetString("name", playerScriptable.name);
                PlayerPrefs.SetString("email", playerScriptable.email);
                PlayerPrefs.SetString("profileImageUrl", playerScriptable.profileImageUrl);
                
                PlayerPrefs.Save();
          
            SignInWithGoogleOnFirebase(task.Result.IdToken);
        }
    }

    private void SignInWithGoogleOnFirebase(string idToken)
    {
        Credential credential = GoogleAuthProvider.GetCredential(idToken, null);

        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            playerScriptable.token = task.Result.UserId;
            PlayerPrefs.SetString("token", task.Result.UserId);
            PlayerPrefs.Save();
            AggregateException ex = task.Exception;
            if (ex != null)
            {
                if (ex.InnerExceptions[0] is FirebaseException inner && (inner.ErrorCode != 0))
                    Debug.Log("\nError code = " + inner.ErrorCode + " Message = " + inner.Message);
            }
            else
            {   
                FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(task.Result.UserId).GetValueAsync().ContinueWithOnMainThread(t => {
                    if (t.IsFaulted)
                    {
                        Debug.Log("Error");
                    }
                    else if (t.IsCompleted)
                    {
                        DataSnapshot snapshot = t.Result;
                        if (snapshot.Exists)
                        {
                            int balance = Convert.ToInt32(snapshot.Child("balance").Value.ToString());
                            playerScriptable.balance = balance;
                            PlayerPrefs.SetInt("balance", balance);
                            PlayerPrefs.Save();
                            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(task.Result.UserId).Child("profileImageUrl").SetValueAsync(playerScriptable.profileImageUrl);
                            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(task.Result.UserId).Child("name").SetValueAsync(playerScriptable.name);
                            Debug.Log("User already exists");
                        }
                        else
                        {
                            Debug.Log("User does not exist");
                            User user = new User(playerScriptable.name, playerScriptable.email, playerScriptable.profileImageUrl);
                            string json = JsonUtility.ToJson(user);
                            FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(task.Result.UserId).SetRawJsonValueAsync(json).ContinueWithOnMainThread(task => {
                                if (task.IsFaulted)
                                {
                                    Debug.Log("Error");
                                }
                                else if (task.IsCompleted)
                                {
                                    Debug.Log("Success");
                                }
                            });
                        }
                    }
                });
                
                SceneManager.LoadScene("MenuScreen");
                Debug.Log("Sign In Successful.");
            }
        });
    }
}