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

        public Vector2 Position;
        public double Radius;
        public double RadiusConst;
        public double MaxRadius;

        public RegisteredBoundingSphere(Vector2 Position, double Radius, double RadiusConst, double MaxRadius, int typer)
        {
            this.Position = Position;
            this.Radius = Radius;
            this.RadiusConst = RadiusConst;
            this.MaxRadius = MaxRadius; 
            type = typer;
            elapsedlife = new TimeSpan();
            boundingsphere = new BoundingSphere(new Vector3(Position.X, 0, Position.Y),(float)Radius);
        }

        public bool Update()
        {
            Radius += RadiusConst;
            boundingsphere = new BoundingSphere(new Vector3(Position.X, 0, Position.Y),(float) Radius);
            if (Radius > MaxRadius) return false;
            return true;
        }

    }
}
