using System;
using System.Collections.Generic;
using System.IO;

namespace WindowsFormsApplication1.Classes
{
    internal class Club: ICloneable<Club>
    {
        List<Player> _lst_Players = new List<Player>();
        FileStream _fileRecords;
        String _clubName; 

        internal List<Player> Lst_Players
        {
            get
            {
                return _lst_Players;
            }

            set
            {
                _lst_Players = value;
            }
        }

        public FileStream FileRecords
        {
            get
            {
                return _fileRecords;
            }

            set
            {
                _fileRecords = value;
            }
        }

        public string ClubName
        {
            get
            {
                return _clubName;
            }

            set
            {
                _clubName = value;
            }
        }

        public void addPlayer(Player player) { }
        public void removePlayer(Player player) { }

        public Club Clone()
        {
            Club deepCopy = new Club();
            deepCopy.ClubName = this.ClubName;
            
            foreach(Player tmp in this.Lst_Players)
            {
                deepCopy.Lst_Players.Add(tmp.Clone());
            }

            deepCopy.FileRecords = this.FileRecords;
            return deepCopy;
        }
    }
}