using System.Collections.Generic;
using System.IO;

namespace WindowsFormsApplication1.Classes
{
    internal class Season
    {
        string _name;
        List<Club> _lst_clubs = new List<Club>();
        int numClubChange;
        int numParticipantsChange;
        FileStream _fileRecords;

        internal List<Club> Lst_clubs
        {
            get
            {
                return _lst_clubs;
            }

            set
            {
                _lst_clubs = value;
            }
        }

        public int NumberClubChange
        {
            get
            {
                return numClubChange;
            }

            set
            {
                numClubChange = value;
            }
        }

        public int NumberParticipantsChange
        {
            get
            {
                return numParticipantsChange;
            }

            set
            {
                numParticipantsChange = value;
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

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public void insertClub(Club club) { }
        public void removeClub(Club club) { }
    }
}