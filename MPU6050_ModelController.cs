using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using TMPro;
using System.IO.Ports;
using UnityEngine;


// quick tool put together for visualising MPU6050 data via an fbx model in Unity.
// used for a swimming wearable project
// outputs lap time, number of turns, average lap time and degrees of rotation as 
// text in a unity scene



public class MPU6050_ModelController : MonoBehaviour
{

    public GameObject SwimmerModel;
    public GameObject StartScreen;
    public GameObject StButton;

    Timer timerScript;

    public float y;
    public float z;
    public float x;

    public GameObject RoomData;
    public GameObject LapTimer;

    //Start button
    public Button StartButton;
    bool _Start = false;

    //TEXT
    public TextMeshPro degRotation;
    public string degRotationText;

    public TextMeshPro turnCounter;
    public string turnCounterText;
    bool turned = true;
    bool countIt;

    public int turnCount;
    List<int> dumbnum;


    SerialPort serial = new SerialPort("/dev/tty.usbserial-A5050VSZ", 38400);

    // Use this for initialization
    void Start()
    {
        //set roomData and model to inactive.
        RoomData.SetActive(false);
        SwimmerModel.SetActive(false);


        timerScript = LapTimer.GetComponent<Timer>();
        StartButton = StartButton.GetComponent<Button>();

        StartButton.onClick.AddListener(TaskOnClick);

        try
        {
            serial.Open();
            serial.ReadTimeout = 25;
            serial.WriteTimeout = 25;

        }
        catch (Exception e)
        {
            Debug.Log("Couldn't open serial port" + e);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!serial.IsOpen)
        {

            serial.Open();

        }

        //check if start button clicked
        if (_Start)
        {
            try
            {
                serial.Write("a");
                //Debug.Log(serial.ReadLine());
                int AcX = int.Parse(serial.ReadLine());

                transform.localEulerAngles = new Vector3(x -= AcX / 3000, 0, -90);
            }
            catch (Exception e)
            {
                Debug.Log("ERROR sending a: {0}" + e);
            }


            //Turn counter
            if ((AcX > 100) && (turned == true)){
                countIt = true;
                turnCount++;
                timerScript.CalculateAverage();
                timerScript.ResetTime(); //reset lap time

                turned = false;

                }

            if(AcX < -100){
                turned = true;
                countIt = false;
            }

            //Output Degrees
            degRotationText = transform.localEulerAngles.x.ToString("0.0");
            degRotation.text = degRotationText;

            //Output Turn Count
            turnCounterText = turnCount.ToString();
            turnCounter.text = turnCounterText;
        }
    }

    void TaskOnClick()
    {
        _Start = true;
        StartScreen.SetActive(false);
        SwimmerModel.SetActive(true);
        RoomData.SetActive(true);
        StButton.SetActive(false);
    }

    void GoToStartScreen()
    {
        StartScreen.SetActive(true);
        SwimmerModel.SetActive(false);
        RoomData.SetActive(false);
        StButton.SetActive(true);
    }


}
