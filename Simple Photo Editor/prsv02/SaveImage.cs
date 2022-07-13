using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace prsv02
{
    //Staticka klasa koja sluzi za cuvanje slike na odgovarajucoj lokaciji
    static class SaveImage
    {
        //putanja do lokacije gdje ce se slika sacuvati
        public static string path = null;

        //staticka metoda koja cuva sliku na lokaciji koju korisnik izabere
        public static void saveAs(Bitmap bmpForSave)
        {
            //otvaranje dialoga koji omogucava korisniku da izabere lokaciju za cuvanje slike
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "images| *.jpg; *.png; *.bmp; *jpeg";  //filtriramo da se prikazuju samo slike

            if (dialog.ShowDialog() == DialogResult.OK)   //ako korisnik potvrdi lokaciju slika se sacuva
            {
                path = dialog.FileName;
                bmpForSave.Save(path);
            }
        }

        //metoda koja sluzi za cuvanje slike na vec odredjenoj lokaciji
        public static void save(Bitmap bmp, string path)
        {
            System.Windows.Forms.MessageBox.Show("Da li zelite sa sacuvate promjene\n" + path + "?", "Message", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
    }
}
