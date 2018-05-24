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
    public interface ICloneable<T>
    {
        T Clone();
    }

    

    public partial class Form1 : Form
    {
        Classes.File global_file = new Classes.File();
        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //lay du lieu can thiet: mua giai, club
                //lay ten club
            string club_name = listBox1.GetItemText(listBox1.SelectedItem);
                //lay index mua giai
            string[] muagiai_line = this.comboBox1.GetItemText(this.comboBox1.SelectedItem).Split(' ');
            int indexMuaGiai = int.Parse(muagiai_line[1]) - 1;

            //hien thi danh sach CAU THU len lIST BOX 2
            if (indexMuaGiai == 0)//neu la mua giai dau tien
            {
               
                listBox3.DataSource = global_file.Lst_Seasons[indexMuaGiai].Lst_clubs.FirstOrDefault(club => club.ClubName == club_name).Lst_Players;
                listBox3.DisplayMember = "Name";
            }
            else
            {
               
                listBox2.DataSource = global_file.Lst_Seasons[indexMuaGiai-1].Lst_clubs.FirstOrDefault(club => club.ClubName == club_name).Lst_Players;
                listBox2.DisplayMember = "Name";
                listBox3.DataSource = global_file.Lst_Seasons[indexMuaGiai].Lst_clubs.FirstOrDefault(club => club.ClubName == club_name).Lst_Players;
                listBox3.DisplayMember = "Name";
            }
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //CHON FILE
            OpenFileDialog choofdlog = new OpenFileDialog();
            choofdlog.Filter = "All Files (*.*)|*.*";
            choofdlog.FilterIndex = 1;
            choofdlog.Multiselect = false;

            if (choofdlog.ShowDialog() == DialogResult.OK)
            {
                string sFileName = choofdlog.FileName; // lay file name
                //khoi tao FileProcessing 
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
                  
                    foreach (Season season in file.Lst_Seasons)// duyet qua tat ca cac mua giai trong FILE
                    {
              
                        if (biendemmuagiai == 0)
                        {
                            khoiTaoChoSeason(sr, season,biendemmuagiai);

                            khoiTaoClubChoSeason(sr, season);

                            khoiTaoCauThuChoTungClub(sr, season);
                           
                        }
              
                        else // neu ko phai mua giai dau tien
                        {
                            khoiTaoChoSeason(sr, season, biendemmuagiai); // khoi tao cho season

                            //khoi tao Clubs cho cho season
                            ganDSMuaGiaiCu(file, biendemmuagiai, season);//gan CLUBS cu~ cho mua giai hien tai
                            khoiTaoClubChoSeason(sr, season); // gan CLUBS cho SEASON hien tai

                            //khoi tao Cau Thu cho cac Clubs
                            khoiTaoCauThuChoTungClub(sr, season); //khoi tao cau thu cho tung CLUBS trong SEASON
                        }
                        biendemmuagiai++;
                    }
              

                    //luu thong tin vao bien toan cuc FILE
                    global_file = file;

                    //hien thi du lieu len GUI
                        //hien thi combobox
                    comboBox1.DataSource = file.Lst_Seasons;
                    comboBox1.DisplayMember = "Name";
                }
            }            
        }

       
        private static void ganDSMuaGiaiCu(Classes.File file, int biendemmuagiai, Season season)
        {
           foreach(Club club in file.Lst_Seasons[biendemmuagiai - 1].Lst_clubs)
            {
                season.Lst_clubs.Add(club.Clone());
            }
        }

        private static void khoiTaoFile(Classes.File file, StreamReader sr, out int numSeasons, out int checkAlgorithm, List<Season> lst_Seasons)
        {
            //lay thong tin 
            string[] first_line = sr.ReadLine().Split(' ');
            numSeasons = int.Parse(first_line[0]);
            checkAlgorithm = int.Parse(first_line[1]);
            //gan thong tin cho object FILE
            file.NumberSeasons = numSeasons;
            file.CheckAlgorithm = checkAlgorithm;
            //tao mang season
            for (int i = 0; i < file.NumberSeasons; i++)
            {
                Season season = new Season();
                season.Name = "Season " + (i + 1).ToString();
                lst_Seasons.Add(season);
            }
            //gan mang season cho FILE
            file.Lst_Seasons = lst_Seasons;
        }

        private static void khoiTaoCauThuChoTungClub(StreamReader sr, Season season)
        {
            //thao tac voi cac CLUBS trong SEASON: them or xoa cau thu
            for (int soluongcauthu = 1; soluongcauthu <= season.NumberParticipantsChange; soluongcauthu++)
            {
                //doc thong tin line cua cau thu 
                string[] cauthu_line = sr.ReadLine().Split(' ');
                int isAdded = int.Parse(cauthu_line[0]);
                //Them hoac xoa cau thu khoi doi bong
                if (isAdded == 1)// them cau thu vao CLUB thich hop trong SEASON
                {
                    //khoi tao cau thu
                    Player player = new Player();
                    player.Name = cauthu_line[2];
                    //them Player vao CLUB thich hop 
                    season.Lst_clubs.FirstOrDefault(x => x.ClubName == cauthu_line[1]).Lst_Players.Add(player);
                }
                else // xoa cau thu ra khoi CLUB thich hop trong SEASON
                {
                    //khoi tao cau thu
                    Player player = new Player();
                    player.Name = cauthu_line[2];

                    //xoa Player ra CLUB 
                    season.Lst_clubs.FirstOrDefault(x => x.ClubName == cauthu_line[1]).Lst_Players.Remove(player);
                }
            }
        }

        private static void khoiTaoClubChoSeason(StreamReader sr, Season season)
        {
            //khoi tao LIST CLUB cho Season
            for (int biendem = 0; biendem < season.NumberClubChange; biendem++)
            {
                //doc thong tin cua club trong season 
                string[] club_line = sr.ReadLine().Split(' '); // doc thong tin
                int isAdded = int.Parse(club_line[0]);
                string nameClub = club_line[1];

                //kiem tra them hoac xoa club
                if (isAdded == 1) // them club vao mua giai  
                {
                    //khoi tao club
                    Club club = new Club();
                    club.ClubName = nameClub;
                    //them club vao LIST CLUB cua SEASON
                    season.Lst_clubs.Add(club);
                }
                else // xoa CLUB ra khoi mua giai
                {
                    if(isAdded == 0)
                    {
                        //khoi tao club
                        Club club = season.Lst_clubs.FirstOrDefault(x => x.ClubName == nameClub);
                        //xoa club ra khoi LIST CLUB cua SEASON
                        season.Lst_clubs.Remove(club);
                    }
                }
            }
        }

        private static void khoiTaoChoSeason(StreamReader sr, Season season, int biendemmuagiai)
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
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //hien thi danh sach club trong mua giai do 

            
            listBox1.Items.Clear();

            //lay index mua giai
            string[] muagiai_line = this.comboBox1.GetItemText(this.comboBox1.SelectedItem).Split(' ');
            int indexMuaGiai = int.Parse(muagiai_line[1]) - 1;

            ////hien thi danh sach CLUB cua MUA GIAI len LIST BOX 1
            foreach (Club club in global_file.Lst_Seasons[indexMuaGiai].Lst_clubs)
            {

                listBox1.Items.Add(club);
            }
            listBox1.DisplayMember = "ClubName";

            //hien thi danh sach CAU THU cua CLUB len LIST BOX 2

        }
    }
}
