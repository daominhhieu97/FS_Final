using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1.Classes
{
    class File
    {
        string _path;
        int checkAlgorithm;
        int numberSeasons;
        List<Season> lst_Seasons = new List<Season>();

        public string Path
        {
            get
            {
                return _path;
            }

            set
            {
                _path = value;
            }
        }

        public int CheckAlgorithm
        {
            get
            {
                return checkAlgorithm;
            }

            set
            {
                checkAlgorithm = value;
            }
        }

        public int NumberSeasons
        {
            get
            {
                return numberSeasons;
            }

            set
            {
                numberSeasons = value;
            }
        }

        internal List<Season> Lst_Seasons
        {
            get
            {
                return lst_Seasons;
            }

            set
            {
                lst_Seasons = value;
            }
        }

        public void readFile() { }
        public void writeFile(String str) { }
    }
}
