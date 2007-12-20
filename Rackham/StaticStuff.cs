using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace Tanks
{
    class StaticStuff
    {
        public static NetworkSession newsession = NetworkSession.Create(NetworkSessionType.Local, 3, 3);
    }
}
