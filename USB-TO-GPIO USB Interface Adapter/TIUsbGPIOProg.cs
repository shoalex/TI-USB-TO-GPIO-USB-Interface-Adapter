///-----------------------------------------------------------------
///   Namespace:      USB_TO_GPIO_USB_Interface_Adapter
///   Class:          TIUsbGPIOProg
///   Description:    The Class connect to TI USB GPIO
///   Author:         Alex Shoyhit                    Date: 30/9/17
///   Notes:          
///   Revision History:
///   Name:Alex Shoyhit           Date:30/9/17        Description:Init
///-----------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TIDP.SAA;
using UCD9081_Transport_Project;

namespace USB_TO_GPIO_USB_Interface_Adapter
{
    class TIUsbGPIOProg
    {
        private byte address; //address of the connected device
        private int iDevice = -1;//number of founded connected device
        private float fFirmware = 0;//firmeare number of connected device
        private string sParFilePath;// *.par file path
        private bool bIsConnected = false;
        private Sequencer_Interface_Project.Sequencer_Interface_Project Sequencer_Interface;
        
        /// <summary>
        /// Constractor of the class
        /// </summary>
        /// <param name="sParFilePath"></param>
        public TIUsbGPIOProg(string sParFilePath)
        {
            this.sParFilePath = sParFilePath;
            Sequencer_Interface = new Sequencer_Interface_Project.Sequencer_Interface_Project();
        }

        /// <summary>
        /// Make connection to device
        /// </summary>
        /// <returns></returns>
        public bool Connect()
        {
            try
            {
                if (SMBusAdapter.Discover() == 0)
                {
                    this.bIsConnected = false;
                    return false;
                }
                if (!File.Exists(this.sParFilePath))
                {
                    this.bIsConnected = false;
                    return false;
                }
                bool[] present = new bool[16];
                Sequencer_Interface.discoverDevices(ref present);
                bool skipParameters = false;
                for (int index = 0; index < 16; ++index)
                {
                    if (present[index])
                    {
                        this.address = (byte)(96 + index);
                        if (Sequencer_Interface.getDeviceInfo(address, ref iDevice, ref fFirmware))
                        {

                            skipParameters = Sequencer_Interface.readParamsFromDevice();
                            break;
                        }
                        break;
                    }
                }
                if(!this.Sequencer_Interface.loadParamsFromFile(this.sParFilePath))
                {
                    this.Sequencer_Interface.resetParams(skipParameters);
                    this.bIsConnected = false;
                    return false;
                }
                this.bIsConnected = true;
                return true;
            }
            catch (Exception)
            {
                this.bIsConnected = false;
                return false;
            }
        }

        /// <summary>
        /// Reset the divice
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            try
            {
                
                this.Sequencer_Interface.resetParams(false);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Burn the choosen file to board
        /// </summary>
        /// <returns></returns>
        public bool Burn()
        {
            this.Sequencer_Interface.updateConfiguration(false);
            if(!this.Sequencer_Interface.resequenceDevice())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Get Current Voltage on the device
        /// </summary>
        /// <returns></returns>
        public float[] GetVolts()
        {
            float[] fVolts = new float[8];
            Sequencer_Interface.readVoltages(out fVolts);
            return fVolts;
        }

        /// <summary>
        /// return status of the device
        /// </summary>
        /// <returns></returns>
        public string ToString()
        {
            return "UCD9081 Present, I2C Address: 0x" + address.ToString("X");
        }
        
    }
}
