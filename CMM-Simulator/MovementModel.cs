using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CMM_Simulator;
public class MovementModel
{
    public string MovementType { get; set; }
    public decimal Distance { get; set; }
    public int XVector { get; set; }
    public int YVector { get; set; }
    public int ZVector { get; set; }
}
