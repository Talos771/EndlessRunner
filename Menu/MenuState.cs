using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EndlessRunner.Menu
{
    public enum MenuState
    {
        Main,
        Playing,
        Transition,
        None,
        Options,
        ScoreBoard,
        GameOver,
        TextEntry
    }
}
