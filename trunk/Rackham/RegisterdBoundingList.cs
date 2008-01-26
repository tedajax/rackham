using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Tanks
{
    public class RegisteredBoundingSphere
    {
        public BoundingSphere boundingsphere;
        public int type;
        public TimeSpan lifespan;
        public TimeSpan elapsedlife;

        public RegisteredBoundingSphere(BoundingSphere sphere, int typer, TimeSpan lifespan)
        {
            boundingsphere = sphere;
            type = typer;
            this.lifespan = lifespan;
            elapsedlife = new TimeSpan();
        }
    }
}
