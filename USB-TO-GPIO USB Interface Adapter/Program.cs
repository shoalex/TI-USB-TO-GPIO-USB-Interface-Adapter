using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace USB_TO_GPIO_USB_Interface_Adapter
{
    class Program
    {
        static void Main(string[] args)
        {
            string sParFilePath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\\\", "\\") + "Last.par";
            if(!File.Exists(sParFilePath))
            {
                Console.WriteLine("File Not exist");
                Console.ReadKey();
                return;
            }
            TIUsbGPIOProg ti = new TIUsbGPIOProg(sParFilePath);
            if(!ti.Connect())
            {
                Console.WriteLine("Can't connect to device");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Connected to Device: " + ti.ToString());
            if(!ti.Burn())
            {
                Console.WriteLine("Can't burn file");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("File Burned");
            float[] fVoltage = ti.GetVolts();
            for (int i = 0; i < fVoltage.Length; i++)
            {
                Console.WriteLine("Rail" + (i + 1) + " " + fVoltage[i].ToString("#0.00")+" V");
            }
            Console.ReadKey();

        }
    }
}
