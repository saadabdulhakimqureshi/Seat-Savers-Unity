using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    // Canvas objects
    public GameObject WelcomeCanvas;
    public GameObject LoginCanvas;
    public GameObject RegisterCanvas;
    public GameObject DashboardCanvas;

    // Screen objects
    public GameObject FeedScreen;
    public GameObject BookingsScreen;
    public GameObject NowShowing;
    public GameObject Cinemas;
    public GameObject DateAndTime;
    public GameObject SeatPlanning;
    public GameObject Payment;

    // Message Box
    public GameObject RegisterationMessageBox;
    public GameObject LoginMessageBox;
    public GameObject DashboardMessageBox;

    // Game objects
    public GameObject DatabaseManager;
    public GameObject[] Rows = new GameObject[10];

    // Text objects
    public GameObject RegisterEmail;
    public GameObject RegisterPassword;
    public GameObject LoginEmail;
    public GameObject LoginPassword;
    public TextMeshProUGUI RegisterationMessageBoxText;
    public TextMeshProUGUI LoginMessageBoxText;
    public TextMeshProUGUI DashboardMessageBoxText;


    public TextMeshProUGUI NumberOfTickets;
    public TextMeshProUGUI Amount;
    public TextMeshProUGUI Email;
    public TextMeshProUGUI Date;
    public TextMeshProUGUI Time;
    public TextMeshProUGUI Movie;
    public TextMeshProUGUI Cinema;
    public TextMeshProUGUI Seats;
    public TextMeshProUGUI SeatsAvailable;
    public TextMeshProUGUI SeatsChoosen;
    public TextMeshProUGUI AccountNumber1;
    public TextMeshProUGUI AccountNumber2;
    public TextMeshProUGUI AccountNumber3;
    public TextMeshProUGUI AccountNumber4;
    public TextMeshProUGUI CVN;
    // User objects
    [SerializeField] public UserInfo userInfo;

    // PurchasingObjects
    public GameObject CinemaList;
    public GameObject DateList;
    public UnityEngine.UI.Button[] SeatButtons = new UnityEngine.UI.Button[29];
    public bool[] Booked = new bool[29];
    public bool[] Choosen = new bool[29];
    void Start()
    {
        if (userInfo.signedIn)
        {
            WelcomeCanvas.SetActive(false);
            LoginCanvas.SetActive(false);
            RegisterCanvas.SetActive(false);
            DashboardCanvas.SetActive(true);
        }
        else
        {
            WelcomeCanvas.SetActive(true);
            LoginCanvas.SetActive(false);
            RegisterCanvas.SetActive(false);
            DashboardCanvas.SetActive(false);
        }
        FeedScreen.SetActive(true);
        BookingsScreen.SetActive(false);
        NowShowing.SetActive(true);
        Cinemas.SetActive(false);
        DateAndTime.SetActive(false);
        SeatPlanning.SetActive(false);
        Payment.SetActive(false);
        userInfo.initialize();

    }

    // Update is called once per frame
    void Update()
    {
        userInfo.amount = (userInfo.seatsChoosen * 1000).ToString();
        
    }

    // Welcome Screen Navigation
    public void ShowLoginScreen()
    {
        WelcomeCanvas.SetActive(false);
        LoginCanvas.SetActive(true);
    }

    public void ShowRegisterScreen()
    {
        WelcomeCanvas.SetActive(false);
        RegisterCanvas.SetActive(true);
    }

    public void BackToWelcomeScreen()
    {
        WelcomeCanvas.SetActive(true);
        RegisterCanvas.SetActive(false);
        LoginCanvas.SetActive(false);
    }

    // Dashboard Navigation
    public void ShowFeedScreen()
    {
        FeedScreen.SetActive(true);
        BookingsScreen.SetActive(false);
    }

    public void ShowBookingsScreen()
    {
        BookingsScreen.SetActive(true);
        FeedScreen.SetActive(false);

        UserManager UM = DatabaseManager.GetComponent<UserManager>();
        UM.GetTransactions(userInfo.email, result=>
        {
            userInfo.Bookings = result;
            Debug.Log(result);
            int i = 0;
            foreach (DocumentSnapshot document in userInfo.Bookings)
            {
                if (document.Exists)
                {
                    GameObject Row = Rows[i];
                    Transform transform = Row.GetComponent<Transform>();

                    foreach (Transform childTransform in transform)
                    {
                        TextMeshProUGUI Content = childTransform.GetComponent<TextMeshProUGUI>();
                        if (Content.tag == "Film")
                        {
                            object value = document.GetValue<object>("film");

                            Content.text = value.ToString();
                        }
                        if (Content.tag == "Cinema")
                        {
                            object value = document.GetValue<object>("cinema");

                            Content.text = value.ToString();
                        }
                        if (Content.tag == "Date")
                        {
                            object value = document.GetValue<object>("date");

                            Content.text = value.ToString();
                        }
                        if (Content.tag == "Time")
                        {
                            object value = document.GetValue<object>("time");

                            Content.text = value.ToString();
                        }
                        if (Content.tag == "Amount")
                        {
                            object value = document.GetValue<object>("amount");

                            Content.text = value.ToString();
                        }
                        if (Content.tag == "Tickets")
                        {
                            object value = document.GetValue<object>("number");

                            Content.text = value.ToString();
                        }
                        if (Content.tag == "Seats")
                        {
                            object value = document.GetValue<object>("tickets");

                            Content.text = value.ToString();
                        }
                    }
                }
                i++;
            }
        }
        );
    }

    // Registeration Navigation
    public void CloseMessageBox()
    {
        RegisterationMessageBox.SetActive(false);
        LoginMessageBox.SetActive(false);
        DashboardMessageBox.SetActive(false);
    }

    // Database Navigation

    public void SignIn()
    {
        UserManager UM = DatabaseManager.GetComponent<UserManager>();

        string password = LoginPassword.GetComponent<TMP_InputField>().text;
        string email = LoginEmail.GetComponent<TMP_InputField>().text;

        UM.CheckIfUserExists(email, exists =>
        {
            // Checking if user exists.
            if (exists)
            {
                // Logging in with new email and password.
                UM.SignIn(email, password, result =>
                {
                    if (result == "Failed")
                    {
                        LoginMessageBox.SetActive(true);
                        LoginMessageBoxText.text = "The server is overloaded at this time. Please try again later!";
                    }
                    if (result == "error")
                    {
                        LoginMessageBox.SetActive(true);
                        LoginMessageBoxText.text = "Please check your network connection and try again.";
                    }
                    if (result == "Incorrect")
                    {
                        LoginMessageBox.SetActive(true);
                        LoginMessageBoxText.text = "Please check your email and password";
                    }
                    if (result == "Successful")
                    {
                        DashboardCanvas.SetActive(true);
                        WelcomeCanvas.SetActive(false);
                        LoginCanvas.SetActive(false);
                        RegisterCanvas.SetActive(false);

                        // Updating user status.
                        userInfo.signedIn = true;
                        userInfo.email = email;
                    }
                });
            }
            else
            {
                LoginMessageBox.SetActive(true);
                LoginMessageBoxText.text = "This email is not registered please sign up.";
            }
        });
    }

    public void SignOut()
    {
        // Setting user to null.
        userInfo.signedIn = false;
        userInfo.email = "";

        // Updating screens.
        DashboardCanvas.SetActive(false);
        WelcomeCanvas.SetActive(true);
        LoginCanvas.SetActive(false);
        RegisterCanvas.SetActive(false);
        Start();
    }

    public void Register()
    {
        UserManager UM = DatabaseManager.GetComponent<UserManager>();

        string password = RegisterPassword.GetComponent<TMP_InputField>().text;
        string email = RegisterEmail.GetComponent<TMP_InputField>().text;

        // Checking if user exists.
        UM.CheckIfUserExists(email, exists =>
        {
            if (exists)
            {
                RegisterationMessageBox.SetActive(true);
                RegisterationMessageBoxText.text = "The email you provided is already registered. Register with another email.";
            }
            else
            {
                // Registering new email and password.
                string result = UM.CreateNewUser(email, password);
                Debug.Log(result);
                if (result == "Failed")
                {
                    RegisterationMessageBox.SetActive(true);
                    RegisterationMessageBoxText.text = "The server is overloaded at this time. Please try again later!";
                }
                if (result == "error")
                {
                    RegisterationMessageBox.SetActive(true);
                    RegisterationMessageBoxText.text = "Please check your network connection and try again.";
                }
                if (result == "Successful" || result == "")
                {
                    DashboardCanvas.SetActive(true);
                    WelcomeCanvas.SetActive(false);
                    LoginCanvas.SetActive(false);
                    RegisterCanvas.SetActive(false);

                    // Updating user status.
                    userInfo.signedIn = true;
                    userInfo.email = email;
                }
            }
        });

    }

    public void RegisterBooking()
    {
        UserManager UM = DatabaseManager.GetComponent<UserManager>();

        string email = userInfo.email;

        Debug.Log("Booking");
        UM.CheckIfUserExists(email, exists =>
        {
            // Checking if user exists.
            if (exists)
            {
                // Booking.
                
                string result = UM.AddTransaction(userInfo.email, userInfo.film, userInfo.cinema, userInfo.date, userInfo.time, userInfo.seatsChoosen, userInfo.tickets, userInfo.amount);
                Debug.Log(result);
                if (result == "Failed")
                {
                    DashboardMessageBox.SetActive(true);
                    DashboardMessageBoxText.text = ("Booking failed. Please try again later!");
                }
                else if (result == "error")
                {
                    DashboardMessageBox.SetActive(true);
                    DashboardMessageBoxText.text = ("Network Error Encountered!");
                }
                else if (result == "Successful")
                {
                    DashboardMessageBox.SetActive(true);
                    DashboardMessageBoxText.text = ("Booking confirmed. You can find the invoice in your bookings!");
                    userInfo.initialize();
                    Payment.SetActive(false);
                }
            }
            else
            {
                LoginMessageBox.SetActive(true);
                LoginMessageBoxText.text = "This email is not registered.";
            }
        });
    }
    // Film Selection Navigation
    public void MoveToCinemas()
    {
        if (userInfo.film == ""){
            DashboardMessageBox.SetActive(true);
            DashboardMessageBoxText.text = "Please Select A Film.";

        }
        else
        {
            Cinemas.SetActive(true);
            Cinema.text = userInfo.cinema;
            Amount.text = userInfo.amount;
            Date.text = userInfo.date;
            Time.text = userInfo.time;
            Movie.text = userInfo.film;
            NumberOfTickets.text = userInfo.seatsChoosen.ToString();
            Seats.text = userInfo.tickets;
            Email.text = userInfo.email;
            userInfo.tickets = "";
            for (int i = 0; i < 29; i++)
            {
                if (Choosen[i] == true)
                {
                    userInfo.tickets += (i + 1) + " ";
                }
            }
            Seats.text = userInfo.tickets;
        }
    }
    public void MoveToDateAndTime()
    {
        if (userInfo.cinema == "" || userInfo.cinema == "Select A Cinema")
        {
            DashboardMessageBox.SetActive(true);
            DashboardMessageBoxText.text = "Please Select A Cinema.";

        }
        else
        {
            Cinemas.SetActive(false);
            DateAndTime.SetActive(true);
            Cinema.text = userInfo.cinema;
            Amount.text = userInfo.amount;
            Date.text = userInfo.date;
            Time.text = userInfo.time;
            Movie.text = userInfo.film;
            NumberOfTickets.text = userInfo.seatsChoosen.ToString();
            Email.text = userInfo.email;
            Seats.text = userInfo.tickets;

            userInfo.tickets = "";
            for (int i = 0; i < 29; i++)
            {
                if (Choosen[i] == true)
                {
                    userInfo.tickets += (i + 1) + " ";
                }
            }
            Seats.text = userInfo.tickets;
        }
    }
    public void MoveToSeatingPlan()
    {
        if (userInfo.date == "" || userInfo.date == "Date" || userInfo.time == "")
        {
            DashboardMessageBox.SetActive(true);
            DashboardMessageBoxText.text = "Please Select a Date and Time.";

        }
        else
        {
            SeatPlanning.SetActive(true);
            DateAndTime.SetActive(false);
            Cinema.text = userInfo.cinema;
            Amount.text = userInfo.amount;
            Date.text = userInfo.date;
            Time.text = userInfo.time;
            Movie.text = userInfo.film;
            Seats.text = userInfo.tickets;
            NumberOfTickets.text = userInfo.seatsChoosen.ToString();
            Email.text = userInfo.email;
            userInfo.tickets = "";
            for (int i = 0; i < 29; i++)
            {
                if (Choosen[i] == true)
                {
                    userInfo.tickets += (i + 1) + " ";
                }
            }
            Seats.text = userInfo.tickets;
        }
    }

    public void MoveToPayment()
    {
        if (userInfo.seatsChoosen == 0)
        {
            DashboardMessageBox.SetActive(true);
            DashboardMessageBoxText.text = "Please Select atleast one Seat.";

        }
        else
        {
            userInfo.amount = (userInfo.seatsChoosen * 200).ToString();

            SeatPlanning.SetActive(false);
            Payment.SetActive(true);
            Cinema.text = userInfo.cinema;
            Amount.text = userInfo.amount;
            Date.text = userInfo.date;
            Time.text = userInfo.time;
            Movie.text = userInfo.film;
            Seats.text = userInfo.tickets;
            NumberOfTickets.text = userInfo.seatsChoosen.ToString();
            Email.text = userInfo.email;

            userInfo.tickets = "";
            for (int i = 0; i < 29; i++)
            {
                if (Choosen[i] == true)
                {
                    userInfo.tickets += (i + 1) + " ";
                }
            }
            Seats.text = userInfo.tickets;
            Debug.Log(userInfo.tickets);
        }
    }

    public void Book()
    {

        if (CVN.text.Length < 3  || AccountNumber1.text.Length < 4 || AccountNumber2.text.Length < 4 || AccountNumber3.text.Length < 4 || AccountNumber4.text.Length < 4)
        {
            DashboardMessageBox.SetActive(true);
            DashboardMessageBoxText.text = "Please Provide Your Account Details!";

        }
        else
        {
            userInfo.amount = (userInfo.seatsChoosen * 200).ToString();
            userInfo.tickets = "";
            for (int i = 0; i < 29; i++)
            {
                if (Choosen[i] == true)
                {
                    userInfo.tickets += (i+1) + " ";
                }
            }
            userInfo.accountNumber1 = AccountNumber1.text;
            userInfo.accountNumber2 = AccountNumber2.text;
            userInfo.accountNumber3 = AccountNumber3.text;
            userInfo.accountNumber4 = AccountNumber4.text;
            RegisterBooking();

        }
    }

    public void SelectFilm(string film)
    {
        userInfo.film = film;
    }

    public void SelectCinema()
    {
        int index = CinemaList.GetComponent<TMP_Dropdown>().value;
        userInfo.cinema = CinemaList.GetComponent<TMP_Dropdown>().options[index].text;
    }

    public void SelectDate()
    {
        int index = DateList.GetComponent<TMP_Dropdown>().value;
        userInfo.date = DateList.GetComponent<TMP_Dropdown>().options[index].text;
    }

    public void SelectTime(string time)
    {
        userInfo.time = time;
    }

    public void SelectSeat(string seat)
    {
        if (Booked[int.Parse(seat)])
        {
            DashboardMessageBox.SetActive(true);
            DashboardMessageBoxText.text = "This seat is booked select another!";
        }
        else
        {
            if (Choosen[int.Parse(seat)])
            {
                Choosen[int.Parse(seat)] = false;
                ColorBlock colorBlock = SeatButtons[int.Parse(seat)].colors;
                colorBlock.normalColor = Color.white;
                colorBlock.selectedColor = Color.white;
                colorBlock.highlightedColor = Color.white;
                SeatButtons[int.Parse(seat)].colors = colorBlock;
                userInfo.seatsAvailable++;
                userInfo.seatsChoosen--;
                SeatsAvailable.text = userInfo.seatsAvailable.ToString();
                SeatsChoosen.text = userInfo.seatsChoosen.ToString();
                
                
            }
            else
            {
                Choosen[int.Parse(seat)] = true;
                ColorBlock colorBlock = SeatButtons[int.Parse(seat)].colors;
                colorBlock.normalColor = Color.red;
                colorBlock.selectedColor = Color.red;
                colorBlock.highlightedColor = Color.red;
                SeatButtons[int.Parse(seat)].colors = colorBlock;

                userInfo.seatsAvailable--;
                userInfo.seatsChoosen++;
                SeatsAvailable.text = userInfo.seatsAvailable.ToString();
                SeatsChoosen.text = userInfo.seatsChoosen.ToString();
            }
        }


    }

}
