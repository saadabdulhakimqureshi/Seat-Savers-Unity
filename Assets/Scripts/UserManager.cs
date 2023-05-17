using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    // Start is called before the first frame update
    string userId;
    Firebase.FirebaseApp app;
    FirebaseFirestore db;
    CollectionReference usersReference;
    CollectionReference transactionsReference;
    [SerializeField] public UserInfo userInfo;
    void Start()
    {
        
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;
                db = FirebaseFirestore.DefaultInstance;
                usersReference = db.Collection("users");
                transactionsReference = db.Collection("transactions");
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                Debug.Log("Firebase.Zindabad");
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

    // Update is called once per frame
    public string CreateNewUser(string email, string password, int bookings = 0)
    {
        // User doesn't exist, so create a new user
        usersReference.AddAsync(new { email, password, bookings }).ContinueWithOnMainThread(
        task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("AddUser was canceled.");
                return "Failed";
            }
            if (task.IsFaulted)
            {
                Debug.LogError("AddUser encountered an error: " + task.Exception);
                return "Error";
            }
            DocumentReference documentRef = task.Result;
            Debug.Log("Document added with ID: " + documentRef.Id);

            return "Successful";
        });
        return "Successful";
    }
    public string AddTransaction(string email, string film, string cinema, string date, string time, int number, string tickets, string amount)
    {
        // User doesn't exist, so create a new user
        transactionsReference.AddAsync(new { amount, cinema, date, email, film, number, tickets, time}).ContinueWithOnMainThread(
        task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("AddTransaction was canceled.");
                return "Failed";
            }
            if (task.IsFaulted)
            {
                Debug.LogError("AddTransaction encountered an error: " + task.Exception);
                return "Error";
            }
            DocumentReference documentRef = task.Result;
            Debug.Log("Document added with ID: " + documentRef.Id);

            return "Successful";
        });
        return "Successful";
    }

    public void GetTransactions(string email, Action<List<DocumentSnapshot>> callback)
    {
        List<DocumentSnapshot> matchingTransactions = new List<DocumentSnapshot>();

        // Matching email with emails registered to check if user exists.
        transactionsReference.GetSnapshotAsync().ContinueWithOnMainThread(
            task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    callback(null);
                    return;
                }

                QuerySnapshot snapshot = task.Result;
                foreach (DocumentSnapshot document in snapshot.Documents)
                {
                    if (document.Exists)
                    {
                        object value = document.GetValue<object>("email");
                        if (value.ToString() == email)
                        {
                            // Add matching transaction to the list
                            matchingTransactions.Add(document);
                        }
                    }
                }

                // Return the list of matching transactions through the callback
                callback(matchingTransactions);
            });
    }

    public void CheckIfUserExists(string email, Action<bool> callback)
    {
        // Matching email with emails registered to check if user exists.
       usersReference.GetSnapshotAsync().ContinueWithOnMainThread(
        task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                callback(false);
                return;
            }

            QuerySnapshot snapshot = task.Result;
            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                if (document.Exists)
                {
                    object value = document.GetValue<object>("email");
                    if (value.ToString() == email)
                    {
                        callback(true);
                        return;
                    }
                }

            }

            callback(false);
            return;
        });
    }

    public void SignIn(string email, string password, Action<string> callback)
    {
        // Matching email and passwords with emails registered and their passwords to validate credentials.
        usersReference.GetSnapshotAsync().ContinueWithOnMainThread(
         task =>
         {
             if (task.IsCanceled || task.IsFaulted)
             {
                 callback("Failed");
                 return;
             }

             QuerySnapshot snapshot = task.Result;
             foreach (DocumentSnapshot document in snapshot.Documents)
             {
                 if (document.Exists)
                 {
                     object value = document.GetValue<object>("email");
                     if (value.ToString() == email)
                     {
                         value = document.GetValue<object>("password");
                         if (value.ToString() == password)
                         {
                             callback("Successful");
                             return;
                         }
                     }
                 }

             }

             callback("Incorrect");
         });
    }
}