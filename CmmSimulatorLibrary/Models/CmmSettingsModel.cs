using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmSimulatorLibrary.Models;
public class CmmSettingsModel
{
    public double Approach { get; set; }
    public double Retract { get; set; }
    public double Depth { get; set; }
    public double Clearance { get; set; }
}
