using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;

[CreateAssetMenu(fileName = "UserInfo", menuName = "Persistance")]
public class UserInfo : ScriptableObject
{
    // UserInfo
    public bool signedIn;
    public string email;
    public string password;

    // PurchasingInfo
    public int bookings = 0;
    public string film = "";
    public string cinema = "";
    public string time = "";
    public string date = "";
    public string tickets = "";
    public string amount = "0";
    public string cvn = "";
    public string accountNumber1 = "";
    public string accountNumber2 = "";
    public string accountNumber3 = "";
    public string accountNumber4 = "";

    public int seatsAvailable;
    public int seatsChoosen;

    // BookingsInfo
    public List<DocumentSnapshot> Bookings;

    public void initialize()
    {
            film = "";
            cinema = "";
            time = "";
            date = "";
            tickets = "";
            amount = "0";
            cvn = "";
            accountNumber1 = "";
            accountNumber2 = "";
            accountNumber3 = "";
            accountNumber4 = "";
            seatsAvailable = 29;
            seatsChoosen = 0;

        Bookings = new List<DocumentSnapshot>();
    }
}
