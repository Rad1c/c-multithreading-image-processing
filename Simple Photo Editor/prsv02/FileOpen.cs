using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace prsv02
{
    class FileOpen
    {
        public string path = "";  //putanja do ucitane slike
        public float sizeInKB;  //velicina ucitane slike u KB
        public Size imgSize;    //duzina i visina slike
        public string imgTitle = null;

        public Bitmap openFile()
        {
            Bitmap bmp = null;

            //otvara se dialog korisniku u kome moze da izabere sliku koju zeli da ucita
            OpenFileDialog fileDialog = new OpenFileDialog();
            //filtriranje na nacin da se prikazuju samo slike ekstenzija png, jpeg, jpg, gif i bmp
            fileDialog.Filter = "Image files |*.png;*.jpeg; *.jpg; *.gif; *.bmp";  

            if (fileDialog.ShowDialog() == DialogResult.OK)  //ako je korisnik potvrdio izbor
            {
                path = fileDialog.FileName;                             //putanja do izabrane slike
                FileInfo imageInfo = new FileInfo(path);
                float pom = imageInfo.Length / 1000.0f;
                sizeInKB = (int)(pom * 10)/10.0f;                       //velicina slike u KB
                bmp = new Bitmap(Image.FromFile(fileDialog.FileName));  //smjestanje ucitane slike u bitmapu
                imgSize.Width = bmp.Width;                              //podesavanje sirine slike
                imgSize.Height = bmp.Height;                            //podesavanje visine slike
                imgTitle = imageInfo.Name;                              //cuvanje naziva slike
            }
            return bmp;                                                 //vraca ucitanu sliku kao bitmapu
        }
    }
}
