using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmmSimulatorLibrary.Models;
public class PointModel : FeatureModel
{
    public PointModel(double x, double y, double z, double i, double j, double k) : base(x, y, z, i, j, k)
    {
    }
    public PointModel() { }
}
