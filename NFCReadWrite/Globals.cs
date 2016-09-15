using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Proximity;

/**
 * Global variables declared here are accessed by the classes in the Pages folder.
 **/ 
namespace NFCReadWrite
{
    class Globals
    {
        public static ProximityDevice proximityDevice { get; set; }
        public static long subscriptionId { get; set; }
    }
}
