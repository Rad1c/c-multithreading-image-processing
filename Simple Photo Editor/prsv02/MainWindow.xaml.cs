using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace prsv02
{
    public partial class MainWindow : Window
    {
        Bitmap srcImage = null;     //ucitana slike
        Bitmap newImage = null;     //pomocna promjenjiva na kojoj se vrsi primjena filtera
        string path;                //putanja do ucitane slike
        bool imgChange = false;     //logicka promjenjiva koja sluzi da registruje ako je na slici doslo do izmjena
        FileOpen file = new FileOpen();     //FileOpen -> klasa koja sluzi da se ucita slika
        ImageProcess imgProcessing = new ImageProcess();        //ImageProcess -> klasa koja vrsi primjenu filtera na sliku

        public MainWindow()
        {
            //Podesavanje izgleda pri inicijalnom pokretanju aplikacije
            InitializeComponent();
            lblDimension.Visibility = Visibility.Hidden;
            lblSize.Visibility = Visibility.Hidden;
            imgDimension.Visibility = Visibility.Hidden;
            imgSize.Visibility = Visibility.Hidden;
            lblExecutedTIme.Visibility = Visibility.Hidden;
        }

        private void miOpen_Click(object sender, RoutedEventArgs e)
        {
            openImage();  //ucitavanje slike
        }

        //funkcija koja vrsi ucitavanje slike na kojoj se primjenjuju filteri
        public void openImage()
        {
            srcImage = file.openFile();                     //ucitavanje slike
            lblExecutedTIme.Visibility = Visibility.Hidden;

            if (srcImage != null)
            {
                this.Title = file.imgTitle + " - Simple Photo Editor";      //podesavanje naslova aplikacije
                if (imgChange)
                    SaveImage.save(newImage, path);
                imgChange = false;
                path = file.path;
                btnOpenImage.Visibility = Visibility.Hidden;
                btnOpenImage.Visibility = Visibility.Collapsed;
                imgOpen.Visibility = Visibility.Hidden;
                imgOpen.Visibility = Visibility.Collapsed;
                displayImage(srcImage);                         //prikazivanje slike u aplikaciji
                miSave.IsEnabled = true;                        //Omogucavanje da se ucitana slika sacuva
                miSaveAs.IsEnabled = true;                      //Omogucivanje da se ucitana slika sacuva na odredjenoj lokaciji
                showLabelStatusBar();                           
            }
        }

        //funkcija koja prikazuje ucitanu sliku
        public void displayImage(Bitmap bmp)
        {
            imgSource.Source = BitmapToImageSource(bmp);
        }

        //Pomocna funkcija koja pretvara bitmapu u ImageSource da bi se mogla prikazati u aplikaciji
        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        //funkcija koja se poziva nakon klika korisnika na dugme Start
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            //ako slika nije ucitana prikazuje se poruka korisniku
            if (srcImage == null)
            {
                System.Windows.Forms.MessageBox.Show("Ucitajte fotografiju", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Bitmap tmp = (Bitmap)srcImage.Clone();              //pomocna promjenjiva gdje se smjesta ucitana slika
            string filter = checkFilterOptions();               //ispitivanje da li je ispravno odabran filter

            if (String.Equals(filter, "none") == false)         //ako je filter ispravno odabran
            {
                string mode;
                this.Cursor = System.Windows.Input.Cursors.Wait;        //promjena kursora u Wait dok traje obrada slike (primjena filtera)
                Stopwatch sw = new Stopwatch();                         //pravi se novi objekat klase StopWatch
                sw.Start();                                             //startovanje stoperice

                if (cmbMode.SelectedIndex == 0)                         //ispitivanje da li je odabran paralelni nacin izvrsavanja ili ne
                {
                    mode = "Parallel";
                    newImage = imgProcessing.processParallel(tmp, filter);      //izvrsava se primjena filtera paralelizacijom
                }
                else
                {
                    mode = "Non-Parallel";
                    newImage = imgProcessing.processNonParallel(tmp, filter);   //izvrsava se primjena filtera bez paralelizacije
                }

                sw.Stop();                          //Stopiranje stoperice
                TimeSpan ts = sw.Elapsed;           //mjerenje vremena koje je proslo od pocetka startovanja stoperice
                writeToConsole(ts, mode, filter);   //ispisivanje vremena u konzolu
                writeToStatusBar(ts);               //ispisivanje vremena na statusnu liniju

                imgChange = true;                   //logicka promjenjiva koja definise da li se nad slikom desila modifikacija se postavlja na true
                displayImage(newImage);
                this.Cursor = System.Windows.Input.Cursors.Arrow;           //kursor se postavlja na Arrow nakon sto je obrada slike zavrsena

            }
            else
                System.Windows.Forms.MessageBox.Show("Izaberite jedan od filtera!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        //Funkcija koja ispisuje vrijeme obrade fotografije u konzolu
        private void writeToConsole(TimeSpan ts, string mode, string filter)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime + ", Mode: " + mode + ", Filter: " + filter);
        }

        //funkcija koja ispisuje vrijeme koje je potrebno da se izvrsi primjena filtera. Ispisuje se na statusnu liniju.
        private void writeToStatusBar(TimeSpan ts)
        {
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",       //podesavanje formata vremena (sati, minute, sekunde, milisekunde)
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            lblExecutedTIme.Visibility = Visibility.Visible;                        //prikazivanje labele gdje ce se vrijeme prikazat
            lblExecutedTIme.Content = "Executed time: " + elapsedTime;              //prikaz vremena na labeli
        }

        //funkcija koja prikazuje informacije o slici na statusnoj liniji
        private void showLabelStatusBar()
        {
            lblDimension.Visibility = Visibility.Visible;
            lblSize.Visibility = Visibility.Visible;
            imgDimension.Visibility = Visibility.Visible;
            imgSize.Visibility = Visibility.Visible;

            lblSize.Content = "Size: " + file.sizeInKB + "KB";
            lblDimension.Content = file.imgSize.Width + " x " + file.imgSize.Height + "px";
        }

        //Funkcija koja provjerava stanje radio button-a (ako je izabran filter vraca njegovo ime, a ako nije vraca none)
        private string checkFilterOptions()
        {
            string filter;

            if ((bool)rbRedFilter.IsChecked)
                filter = "red";
            else if ((bool)rbGreenFilter.IsChecked)
                filter = "green";
            else if ((bool)rbBlueFilter.IsChecked)
                filter = "blue";
            else if ((bool)rbInvertColorsFilter.IsChecked)
                filter = "invert";
            else
                filter = "none";

            return filter;
        }

        //funkcija koja ponistava filtere koje smo primijenili na sliku
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            newImage = (Bitmap)srcImage.Clone();
            displayImage(srcImage);
        }

        //klikom na dugme Exit zatvara se aplikacija
        private void miExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult result = System.Windows.Forms.MessageBox.Show("Da li zelite da izadjete iz programa?", "Message", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            if(result == System.Windows.Forms.DialogResult.Yes)
                this.Close();
        }

        //funkcija kojom se slika ucitava
        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            openImage();
        }

        //funkcija koja cuva promjene koje smo izvrsili nad ucitanom slikom
        private void miSave_Click(object sender, RoutedEventArgs e)
        {
            if (imgChange)
            {
                this.Cursor = System.Windows.Input.Cursors.Wait;            //promjena kursora u Wait dok se slika ne sacuva
                newImage.Save(path);                                        //cuvanje slike
                this.Cursor = System.Windows.Input.Cursors.Arrow;           //promjena kursora na Arrow (inicijalno stanje)
            }
        }

        //funkcija koja se poziva klikom na stavku menija Save as
        private void miSaveAs_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = System.Windows.Input.Cursors.Wait;
            if (imgChange)
                SaveImage.saveAs(newImage);
            else
                SaveImage.saveAs(srcImage);
            if (SaveImage.path != null)
                path = SaveImage.path;
            this.Cursor = System.Windows.Input.Cursors.Arrow;
        }

        //otvaranje sekcija About klikom misa na stavku menija About
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //Otvaram novi prozor za prikaz
            About about = new About();              //prozor koji se otvara klikom na About
            about.ShowDialog();                     //prikazivanje prozora About kao ShowDialog
        }
    }
}