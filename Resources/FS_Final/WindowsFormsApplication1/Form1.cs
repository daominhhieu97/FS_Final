using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1.Classes;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Classes.File global_file = new Classes.File(); // tao global file

        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lay du lieu can thiet: mua giai, club
            string club_name;
            int indexMuaGiai;
            layDuLieuClubVaMuaGiai(out club_name, out indexMuaGiai);

            //hien thi danh sach CAU THU len lIST BOX 2
            if (indexMuaGiai == 0)//neu la mua giai dau tien
            {
                hienThiMuaGiaiDau(club_name, indexMuaGiai);
                
            }
            else
            {
                hienThiMuaGiaiConLai(club_name, indexMuaGiai);
            }


        }

        private void layDuLieuClubVaMuaGiai(out string club_name, out int indexMuaGiai)
        {
            //lay ten club
            club_name = listBox1.GetItemText(listBox1.SelectedItem);
            //lay index mua giai
            string[] muagiai_line = this.comboBox1.GetItemText(this.comboBox1.SelectedItem).Split(' ');
            indexMuaGiai = int.Parse(muagiai_line[1]) - 1;
        }

        private void hienThiMuaGiaiConLai(string club_name, int indexMuaGiai)
        {
            if (global_file.Lst_Seasons[indexMuaGiai - 1].Lst_clubs.FirstOrDefault(club => club.ClubName == club_name) != null) //neu co tham gia mua giai truoc 
            {
                listBox2.DataSource = global_file.Lst_Seasons[indexMuaGiai - 1].Lst_clubs.FirstOrDefault(club => club.ClubName == club_name).Lst_Players;
                listBox2.DisplayMember = "Name";
                listBox3.DataSource = global_file.Lst_Seasons[indexMuaGiai].Lst_clubs.FirstOrDefault(club => club.ClubName == club_name).Lst_Players;
                listBox3.DisplayMember = "Name";
            }
            else // neu ko tham gia mua giai truoc 
            {
                listBox2.DataSource = null;
                listBox2.Items.Clear();
                listBox3.DataSource = global_file.Lst_Seasons[indexMuaGiai].Lst_clubs.FirstOrDefault(club => club.ClubName == club_name).Lst_Players;
                listBox3.DisplayMember = "Name";

            }
        }

        private void hienThiMuaGiaiDau(string club_name, int indexMuaGiai)
        {
            Club club = global_file.Lst_Seasons[indexMuaGiai].Lst_clubs.FirstOrDefault(tmp => tmp.ClubName == club_name);
            //LAST SEASON
            listBox3.DataSource = club.Lst_Players;
            listBox3.DisplayMember = "Name";

            //the previous season
            listBox2.DataSource = null;
            listBox2.Items.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //CHON FILE
            OpenFileDialog choofdlog = chonFile();

            if (choofdlog.ShowDialog() == DialogResult.OK) // neu bam ok
            {
                string sFileName = choofdlog.FileName; // lay file name

                //tao moi  FileProcessing 
                Classes.File file = new Classes.File();
                file.Path = sFileName;
                textBox1.Text = file.Path;

                using (StreamReader sr = new StreamReader(sFileName)) // goi streamreader de doc file 
                {
                    //xu ly thong tin dong dau 
                    //khoi tao
                    int numSeasons, checkAlgorithm;
                    List<Season> lst_Seasons = new List<Season>();

                    khoiTaoFile(file, sr, out numSeasons, out checkAlgorithm, lst_Seasons);

                    int biendemmuagiai = 0; //bien dem xac dinh mua giai ban dau

                    biendemmuagiai = xuliSeason(file, sr, biendemmuagiai);

                    //luu thong tin vao bien toan cuc FILE
                    global_file = file;

                    //hien thi du lieu len GUI
                    hienThiMuaGiaiLenGUI(file);
                }
            }
        }

        private void hienThiMuaGiaiLenGUI(Classes.File file)
        {
            //hien thi combobox
            comboBox1.DataSource = file.Lst_Seasons;
            comboBox1.DisplayMember = "Name";
        }

        private  int xuliSeason(Classes.File file, StreamReader sr, int biendemmuagiai)
        {
            foreach (Season season in file.Lst_Seasons)// duyet qua tat ca cac mua giai trong FILE
            {
                if (biendemmuagiai == 0)
                {
                    khoiTaoChoMuaGiaiDau(sr, biendemmuagiai, season);
                }

                else // neu ko phai mua giai dau tien
                {
                    khoiTaoChoCacMuaGiaiTiepTheo(file, sr, biendemmuagiai, season);
                }

                //luu thong tin season vua roi vao file va gan cho global file
                global_file = file;

                //tang bien dem mua giai
                biendemmuagiai++;
            }

            // deFragement cho toan bo season
            doDefragmentForAllClubInSeason();

            return biendemmuagiai;
        }

        private void doDefragmentForAllClubInSeason()
        {
            string dataasstring = "-> DEFRAGMENTATION: " + global_file.Lst_Seasons.Last().Seasonnprocessing.Defragment() + '\n'; //your data
            byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
            global_file.Lst_Seasons.Last().FileRecords.Write(info, 0, info.Length);
        }

        private void khoiTaoChoCacMuaGiaiTiepTheo(Classes.File file, StreamReader sr, int biendemmuagiai, Season season)
        {
            khoiTaoChoSeason(sr, season, biendemmuagiai); // khoi tao cho season

            //khoi tao Clubs cho cho season
            ganDSMuaGiaiCu(file, biendemmuagiai, season);//gan CLUBS cu~ cho mua giai hien tai
            khoiTaoClubChoSeason(sr, season, biendemmuagiai); // gan CLUBS cho SEASON hien tai

            //khoi tao Cau Thu cho cac Clubs
            khoiTaoCauThuChoTungClub(sr, season, biendemmuagiai); //khoi tao cau thu cho tung CLUBS trong SEASON
        }

        private void khoiTaoChoMuaGiaiDau(StreamReader sr, int biendemmuagiai, Season season)
        {
            khoiTaoChoSeason(sr, season, biendemmuagiai);

            khoiTaoClubChoSeason(sr, season, biendemmuagiai);

            khoiTaoCauThuChoTungClub(sr, season, biendemmuagiai);
        }

        private static OpenFileDialog chonFile()
        {
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;
            return choofdlog;
        }

        private static void ganDSMuaGiaiCu(Classes.File file, int biendemmuagiai, Season season)
        {
           foreach(Club club in file.Lst_Seasons[biendemmuagiai - 1].Lst_clubs)
            {
                season.Lst_clubs.Add(club.Clone());
            }

        }

        private  void khoiTaoFile(Classes.File file, StreamReader sr, out int numSeasons, out int checkAlgorithm, List<Season> lst_Seasons)
        {
            //lay thong tin cho file 
            string[] first_line = sr.ReadLine().Split(' ');
            numSeasons = int.Parse(first_line[0]);
            checkAlgorithm = int.Parse(first_line[1]);

            //gan thong tin cho object FILE
            file.NumberSeasons = numSeasons;
            file.CheckAlgorithm = checkAlgorithm;

            //tao mang season
            khoiTaoMangSeason(file, lst_Seasons);

            //gan mang season cho FILE
            file.Lst_Seasons = lst_Seasons;
            global_file = file;
        }

        private static void khoiTaoMangSeason(Classes.File file, List<Season> lst_Seasons)
        {
            for (int i = 0; i < file.NumberSeasons; i++)
            {
                Season season = new Season(); // khoi tao season
                season.Name = "Season " + (i + 1).ToString(); // gan ten cho season
                lst_Seasons.Add(season);
            }
        }

        private  void khoiTaoCauThuChoTungClub(StreamReader sr, Season season,int biendemmuagiai)
        {
            //thao tac voi cac CLUBS trong SEASON: them or xoa cau thu
            for (int soluongcauthu = 1; soluongcauthu <= season.NumberParticipantsChange; soluongcauthu++)
            {
                //doc thong tin line cua cau thu 
                string[] cauthu_line = sr.ReadLine().Split(' ');
                int isAdded = int.Parse(cauthu_line[0]); // kiem tra ADD hay DELETE
                Club club = season.Lst_clubs.FirstOrDefault(x => x.ClubName == cauthu_line[1]);
                //Them hoac xoa cau thu khoi doi bong
                if (isAdded == 1)// them cau thu vao CLUB thich hop trong SEASON
                {
                    themCauThuVaoClub(cauthu_line, club);

                }
                else // xoa cau thu ra khoi CLUB thich hop trong SEASON
                {
                    xoaCauThuKhoiClub(cauthu_line, club);

                }
            }

            doDefragmentForAllClubAllPlayerInSeason(season);
        }

        private void xoaCauThuKhoiClub(string[] cauthu_line, Club club)
        {
            //khoi tao cau thu
            Player player = new Player();
            player.Name = cauthu_line[2];

            //xoa Player ra CLUB 
            club.Lst_Players.Remove(player);

            //xu ly file 
            if (global_file.CheckAlgorithm == 1) // xoa theo kieu First Fit
            {
                string dataasstring = "-" + player.Name + " : " + club.Systemprocessing.doDeleteFirstFit(player) + '\n';
                byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                club.FileRecords.Write(info, 0, info.Length);
            }
            else // xoa theo kieu Best fit
            {
                string dataasstring = "-" + player.Name + " : " + club.Systemprocessing.doDeleteBestFit(player) + '\n';
                byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                club.FileRecords.Write(info, 0, info.Length);
            }
        }

        private void themCauThuVaoClub(string[] cauthu_line, Club club)
        {
            //khoi tao cau thu
            Player player = new Player();
            player.Name = cauthu_line[2]; // GAN TEN CHO CAU THU
                                          //them Player vao CLUB thich hop 
            club.Lst_Players.Add(player);

            //xu ly file 
            if (global_file.CheckAlgorithm == 1) // 1 = > First Fit
            {
                string dataasstring = "+" + player.Name + " : " + club.Systemprocessing.doFirstFitRecording(player) + '\n'; //your data
                byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                club.FileRecords.Write(info, 0, info.Length);
            }
            else // Best Fit
            {
                string dataasstring = "+" + player.Name + " : " + club.Systemprocessing.doAddBestFit(player) + '\n'; //your data
                byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                club.FileRecords.Write(info, 0, info.Length);
            }
        }

        private static void doDefragmentForAllClubAllPlayerInSeason(Season season)
        {
            foreach (Club club in season.Lst_clubs) // duyet tat ca cac CLUBs trong Season
            {
                string dataasstring = "--------> DEFRAGMENTATION: " + club.Systemprocessing.doDefragment() + '\n';
                byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
                club.FileRecords.Write(info, 0, info.Length);
            }
        }

        private static void khoiTaoClubChoSeason(StreamReader sr, Season season,int biendemmuagiai)
        {
            //khoi tao LIST CLUB cho Season
            for (int biendem = 0; biendem < season.NumberClubChange; biendem++)
            {
                //doc thong tin cua club trong season 
                string[] club_line = sr.ReadLine().Split(' '); // doc thong tin
                int isAdded = int.Parse(club_line[0]); // lay ki tu dau tien
                string nameClub = club_line[1]; // lay ki tu thu 2

                //kiem tra them hoac xoa club
                if (isAdded == 1) // them club vao mua giai  
                {
                    themClubVaoSeason(season, nameClub);
                }
                else // xoa CLUB ra khoi mua giai
                {
                    if (isAdded == 0)
                    {
                        xoaClubKhoiSeason(season, nameClub);
                    }
                }
            }

            //tao file RECORDING cho tung CLUBs trong season 
            khoiTaoFileRecordingChoTungSeason(season, biendemmuagiai);
        }

        private static void khoiTaoFileRecordingChoTungSeason(Season season, int biendemmuagiai)
        {
            foreach (Club club in season.Lst_clubs)
            {
                club.FileRecords = System.IO.File.Create(club.ClubName + (biendemmuagiai + 1).ToString() + ".txt");
            }
        }

        private static void xoaClubKhoiSeason(Season season, string nameClub)
        {
            //khoi tao club
            Club club = season.Lst_clubs.FirstOrDefault(x => x.ClubName == nameClub);
            //xoa club ra khoi LIST CLUB cua SEASON
            season.Lst_clubs.Remove(club);

            //xoa club ra khoi Season va luu vao trong file CLUBS
            string dataasstring = "-" + club.ClubName + " : " + season.Seasonnprocessing.DeleteRecord(club.ClubName) + '\n'; //your data
            byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
            season.FileRecords.Write(info, 0, info.Length);
        }

        private static void themClubVaoSeason(Season season, string nameClub)
        {
            //khoi tao club
            Club club = new Club(); // khoi tao club
            club.ClubName = nameClub; // gan ten cho club
            club.Systemprocessing = new ClubProcessing(); // khoi tao SystemProcessing cho CLUB
                                                          //them club vao LIST CLUB cua SEASON;
            season.Lst_clubs.Add(club);

            //them club vao season trong file CLUBS theo dang fix length 
            string dataasstring = "+" + club.ClubName + " : " + season.Seasonnprocessing.AddRecord(club.ClubName) + '\n'; //your data
            byte[] info = new UTF8Encoding(true).GetBytes(dataasstring);
            season.FileRecords.Write(info, 0, info.Length);
        }

        private  void khoiTaoChoSeason(StreamReader sr, Season season, int biendemmuagiai)
        {
            //lay thong tin toan bo mua giai 
            string[] thongtinmuagiai = sr.ReadLine().Split(' ');
            //khoi tao thong tin 
            int numClubChange, numParticipantsChange;
            numClubChange = int.Parse(thongtinmuagiai[0]);
            numParticipantsChange = int.Parse(thongtinmuagiai[1]);

            //gan thong tin mua giai 
            season.NumberClubChange = numClubChange;
            season.NumberParticipantsChange = numParticipantsChange;
            if (biendemmuagiai == 0)
            {
                season.FileRecords = System.IO.File.Create("CLUBS" + ".txt");
                season.Seasonnprocessing = new SeasonProcessing();
            }
            else
            {
                season.Seasonnprocessing = global_file.Lst_Seasons[biendemmuagiai - 1].Seasonnprocessing.Clone();
                season.FileRecords = global_file.Lst_Seasons[biendemmuagiai - 1].FileRecords;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            //lay index mua giai
            int indexMuaGiai = layIndexMuaGiai();

            ////hien thi danh sach CLUB cua MUA GIAI len LIST BOX
            hienThiDanhSachClubLenGUI(indexMuaGiai);

        }

        private int layIndexMuaGiai()
        {
            try
            {
                string[] muagiai_line = this.comboBox1.GetItemText(this.comboBox1.SelectedItem).Split(' ');
                int indexMuaGiai = int.Parse(muagiai_line[1]) - 1;
                return indexMuaGiai;
            }
            catch(IndexOutOfRangeException e)
            {
                throw e;
            }
            
        }

        private void hienThiDanhSachClubLenGUI(int indexMuaGiai)
        {
            foreach (Club club in global_file.Lst_Seasons[indexMuaGiai].Lst_clubs)
            {

                listBox1.Items.Add(club);
            }
            listBox1.DisplayMember = "ClubName";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
