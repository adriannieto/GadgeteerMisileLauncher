using System;
using Microsoft.SPOT;
using GHI.Premium.USBHost;
using GHI.Premium.System;
using System.Threading;

namespace GadgeteerMisileLauncher
{


    class MisileLauncher
    {
        
        public enum MisileCommand : byte
        {
            STOP = 0,
            LEFT = 1,
            RIGHT = 2,
            UP  = 4,
            DOWN = 8,
            FIRE = 16,
        }

        private USBH_RawDevice usbDevice;
        private USBH_RawDevice.Pipe statusPipe;


        public MisileLauncher(USBH_Device device)
        {

            if (device.PRODUCT_ID != 0x0202 || device.VENDOR_ID != 0x1130)
                throw new Exception("Invalid device!");

            // Setup of the hardware device
            usbDevice = new USBH_RawDevice(device);
            usbDevice.SendSetupTransfer(0x00, 0x09, 0x0001, 0x0000);

            statusPipe = usbDevice.OpenPipe(usbDevice.GetConfigurationDescriptors(0).interfaces[0].endpoints[0]);
            statusPipe.TransferTimeout = 0;

        }

        private void UsbMisileSendCommand(byte a, byte b, byte c, byte d, byte e, byte f, byte g, byte h)
        {
            byte[] buf = new byte[8];
            Array.Clear(buf, 0, buf.Length);

            buf[0] = a; buf[1] = b; buf[2] = c; buf[3] = d;
            buf[4] = e; buf[5] = f; buf[6] = g; buf[7] = h;

            Debug.Print("Sending data to USB Misile Launcher");
            usbDevice.SendSetupTransfer(0x21, 0x09, 0x0200, 0x001, buf, 0, buf.Length);
        }

        private void UsbMisileSendCommand64(byte a, byte b, byte c, byte d,  byte e, byte f, byte g, byte h)
        {
            byte[] buf = new byte[64];
            Array.Clear(buf, 0, buf.Length);
            
            buf[0] = a; buf[1] = b; buf[2] = c; buf[3] = d;
            buf[4] = e; buf[5] = f; buf[6] = g; buf[7] = h;

            Debug.Print("Sending data to USB Misile Launcher");
            usbDevice.SendSetupTransfer(0x21, 0x09, 0x0200, 0x000, buf, 0, buf.Length);
        }

        public void MisileDO(MisileCommand action)
        {
            byte command = (byte) action;
            
            // Initializator packages
            UsbMisileSendCommand(85, 83, 66, 67, 0, 0, 4, 0);
            UsbMisileSendCommand(85, 83, 66, 67, 0, 64, 2, 0);

            // Handle command
             byte a, b, c, d, e;
                
             a = (byte) (action & MisileCommand.LEFT) > 0x0 ?  (byte) 0x1 : (byte) 0x0;
             b = (byte) (action & MisileCommand.RIGHT) > 0x0 ? (byte)0x1 : (byte)0x0; ;
             c = (byte) (action & MisileCommand.UP) > 0x0 ? (byte)0x1 : (byte)0x0; ;
             d = (byte) (action & MisileCommand.DOWN) > 0x0 ? (byte)0x1 : (byte)0x0; ;
             e = (byte) (action & MisileCommand.FIRE) > 0x0 ? (byte)0x1 : (byte)0x0; ;

             UsbMisileSendCommand64(0,a,b,c,d,e, 8, 8);
        }

    }


}
