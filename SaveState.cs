using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessRunner
{
    [Serializable]
    public class SaveState
    {
        public List<int> HighScore { get; set; }
        public List<string> Names { get; set; }
    }
}