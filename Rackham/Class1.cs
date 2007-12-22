using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace Tanks
{
    public class ProfileSaveData
    {
        public Keys LeftKey;
        public Keys RightKey;
        public Keys UpKey;
        public Keys DownKey;
        public Keys EnterKey;

        public ProfileSaveData()
        {

        }

        public ProfileSaveData(GamePlayer player)
        {
            LeftKey = player.LeftKey;
            RightKey = player.RightKey;
            UpKey = player.UpKey;
            DownKey = player.DownKey;
            EnterKey = player.EnterKey;
        }

        public static GamePlayer SaveDatatoPlayer(GamePlayer player, ProfileSaveData data)
        {
            player.EnterKey = data.EnterKey;
            player.LeftKey = data.LeftKey;
            player.RightKey = data.RightKey;
            player.UpKey = data.UpKey;
            player.DownKey = data.DownKey;
            return player;
        }
            

    }
}
