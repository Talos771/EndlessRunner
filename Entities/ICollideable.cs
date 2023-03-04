using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace EndlessRunner.Entities
{
    public interface ICollideable
    {
        public interface ICollidable
        {
            Rectangle CollisionBox { get; }
        }
    }
}