using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using GHI.Premium.USBHost;
using GHI.Premium.System;

namespace GadgeteerMisileLauncher
{

    public partial class Program
    {

       private MisileLauncher launcherPod;
       private Joystick.Position joystickPosition;
        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/


            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            multicolorLed.TurnRed();
            


            // Subscribe to USBH events.
            USBHostController.DeviceConnectedEvent += DeviceConnectedEvent;
            USBHostController.DeviceDisconnectedEvent += DeviceDisconnectedEvent;

            // Get Joystick Position
            joystickPosition = joystick.GetPosition();
            GT.Timer joystickTimer = new GT.Timer(60);
            joystickTimer.Tick += joystickTimer_Tick;
            joystickTimer.Start();


            // Subscribe to Joystick events.
            joystick.JoystickPressed += new Joystick.JoystickEventHandler(joystick_JoystickPressed);
            
            Debug.Print("Program Started");
        }

        private void joystickTimer_Tick(GT.Timer timer)
        {
            
            const double X_DEADZONE = 0.25;
            const double Y_DEADZONE = 0.25;


            // If Misilelauncher is initializated, handle joystick position
            if (launcherPod != null){
            
                joystickPosition = joystick.GetPosition();
                if (joystickPosition.X > X_DEADZONE)
                {
                    launcherPod.MisileDO(MisileLauncher.MisileCommand.RIGHT);
                }
                else if (joystickPosition.X < -X_DEADZONE)
                {
                    launcherPod.MisileDO(MisileLauncher.MisileCommand.LEFT);
                }


                if (joystickPosition.Y > Y_DEADZONE)
                {
                    launcherPod.MisileDO(MisileLauncher.MisileCommand.UP);
                }
                else if (joystickPosition.Y < -Y_DEADZONE)
                {
                    launcherPod.MisileDO(MisileLauncher.MisileCommand.DOWN);
                }
            }


        }

        private void joystick_JoystickPressed(Joystick sender, Joystick.JoystickState state)
        {
            multicolorLed.BlinkRepeatedly(GT.Color.Yellow, new TimeSpan(10), GT.Color.Orange, new TimeSpan(10));

            if (launcherPod != null)
            {
                launcherPod.MisileDO(MisileLauncher.MisileCommand.FIRE);
            }
                        multicolorLed.TurnGreen();

        }

        private void DeviceConnectedEvent(USBH_Device device)
        {

            if (device.PRODUCT_ID == 0x0202 && device.VENDOR_ID == 0x1130 && device.INTERFACE_INDEX == 0)
            {
                Debug.Print("Device connected...");
                launcherPod = new MisileLauncher(device);
               
                
                // Notify the user that misile launcher is available
                multicolorLed.TurnGreen();
            }


        }

        private void DeviceDisconnectedEvent(USBH_Device device)
        {

            if (device.PRODUCT_ID == 0x0202 && device.VENDOR_ID == 0x1130)
            {
                Debug.Print("Device disconnected...");
                multicolorLed.TurnRed();

            }



        }

    }
}
