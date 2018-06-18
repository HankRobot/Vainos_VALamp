using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Enumeration;
using System.Diagnostics;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI;


namespace VALamp
{
    /// <summary>
    /// A simple program which sends trigger command via Serial interface to an Arduino when certain keywords are detected using the Microsoft speech recognition library. 
    /// Remember to go to Package.app.manifest and add the microphone capability and serial port capabilities. Just open as XML and copy the <capabilities></capabilities> section from this code.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //Objects declaration
        SpeechRecognizer userInput = new SpeechRecognizer();
        SpeechSynthesizer feedback = new SpeechSynthesizer();
        DispatcherTimer checkSerialTimer = new DispatcherTimer();
        DataReader lampReader;

        //For showing the green box if program is listening.
        bool isListening = false;

        public MainPage()
        {
            this.InitializeComponent();
            checkSerialTimer.Tick += CheckSerialTimer_Tick;
            checkSerialTimer.Interval = System.TimeSpan.FromMilliseconds(600);
        }

        //Connect to serial port button (This project was "hacked" in such a short amount of time, I didn't bother naming the buttons)
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            ConnectToSerialPort();
        }

        //Write to serial
        private async void changeColor(string color)
        {
            DataWriter dataWriter = new DataWriter(serialDevice.OutputStream);
            dataWriter.WriteString(color);
            await dataWriter.StoreAsync();
            dataWriter.DetachStream();
            dataWriter = null;
        }
        byte buffer = 0;

        //When timer is triggered, check if recognition is still runnning. If Recognition is done, trigger it again.
        private void CheckSerialTimer_Tick(object sender, object e)
        {
            if(!isListening)
            {
                StartRecognition();
            }
        }

        //Read status, unused.
        private async void ReadmySerial()
        {
            DataReader lreader = new DataReader(serialDevice.InputStream);
            await lreader.LoadAsync(1);
            buffer = lreader.ReadByte();
            lreader.DetachStream();
            lreader = null;
        }

        //Pause button. Stops the timer so that the speech recognition stops.
        private void button_Click(object sender, RoutedEventArgs e)
        {
            checkSerialTimer.Stop();
        }

        //Trigger this function to start speech recognition
        private async void StartRecognition()
        {
            Status.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 200, 70));
            isListening = true;
            await userInput.CompileConstraintsAsync();
            SpeechRecognitionResult theResult = await userInput.RecognizeAsync();
            textBlock.Text = theResult.Text;
            SpeechSynthesisStream feedbackStream = await feedback.SynthesizeTextToStreamAsync("Ok!");
            //Uncomment this if you want to play feedback on the computer. 
            //media.SetSource(feedbackStream, feedbackStream.ContentType);
            if (theResult.Text.Contains("red") || theResult.Text.Contains("angry") || theResult.Text.Contains("manchester") || theResult.Text.Contains("rose"))
            {
                changeColor("r");
            }
            else if (theResult.Text.Contains("blue") || theResult.Text.Contains("sad") || theResult.Text.Contains("chelsea") || theResult.Text.Contains("sea") || theResult.Text.Contains("ocean"))
            {
                changeColor("b");
            }
            else if (theResult.Text.Contains("green") || theResult.Text.Contains("forest") || theResult.Text.Contains("leaf") || theResult.Text.Contains("jealousy") || theResult.Text.Contains("tree") || theResult.Text.Contains("hulk") || theResult.Text.Contains("gamora"))
            {
                changeColor("g");
            }
            else if (theResult.Text.Contains("yellow") || theResult.Text.Contains("pikachu") || theResult.Text.Contains("cheese") || theResult.Text.Contains("gold") || theResult.Text.Contains("sponge") || theResult.Text.Contains("banana"))
            {
                changeColor("y");
            }
            else if (theResult.Text.Contains("white") || theResult.Text.Contains("bright") || theResult.Text.Contains("cloud") || theResult.Text.Contains("paper"))
            {
                changeColor("w");
            }
            else if (theResult.Text.Contains("purple") || theResult.Text.Contains("thanos") || theResult.Text.Contains("junk"))
            {
                changeColor("p");
            }
            else if (theResult.Text.Contains("rainbow") || theResult.Text.Contains("fade") || theResult.Text.Contains("multi"))
            {
                changeColor("x");
            }
            else if (theResult.Text.Contains("disco") || theResult.Text.Contains("random") || theResult.Text.Contains("colourful") || theResult.Text.Contains("beat"))
            {
                changeColor("d");
            }
            else if (theResult.Text.Contains("iron") || theResult.Text.Contains("marvel") || theResult.Text.Contains("flash"))
            {
                changeColor("k");
            }
            else if (theResult.Text.Contains("razor") || theResult.Text.Contains("razer") || theResult.Text.Contains("samsung"))
            {
                changeColor("!");
            }
            else if (theResult.Text.Contains("turn off") || theResult.Text.Contains("dark") || theResult.Text.Contains("black") || theResult.Text.Contains("empty"))
            {
                changeColor("e");
            }
            isListening = false;
            Status.Fill = new SolidColorBrush(Color.FromArgb(255,200, 0,0));
        }
        SerialDevice serialDevice;

        //Handles the serial connection to the Arduino
        private async void ConnectToSerialPort()
        {
            try
            {
                //You might want to include a selector box. I hardcoded this because I'm using my laptop for demo. (AND, I wanted to have the program done quick and dirty)
                string selector = SerialDevice.GetDeviceSelector("COM5");
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(selector);
                if (devices.Count > 0)
                {
                    DeviceInformation deviceInfo = devices[0];
                    serialDevice = await SerialDevice.FromIdAsync(deviceInfo.Id);
                    Debug.WriteLine(serialDevice);
                    serialDevice.BaudRate = 9600;
                    serialDevice.DataBits = 8;
                    serialDevice.StopBits = SerialStopBitCount.Two;
                    serialDevice.Parity = SerialParity.None;
                    lampReader = new DataReader(serialDevice.InputStream);
                }
                else
                {
                    MessageDialog popup = new MessageDialog("Sorry, no device found.");
                    await popup.ShowAsync();
                }
            }
            catch (Exception e)
            {
                MessageDialog popup = new MessageDialog(e.Message);
            }
            
        }

        //Start button, basically just starts the timer. Speech recognition is triggered by the timer event.
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            checkSerialTimer.Start();
        }
    }
}
