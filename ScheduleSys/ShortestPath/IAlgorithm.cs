using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleSys.ShortestPath
{
    public interface IAlgorithm
    {
        Network network { get; set; }
        Node source { get; set; }

        void runAlgorithm();
    }
}
