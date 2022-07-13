using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace prsv02
{
    //Klasa koja sluzi za primjenu odgovarajuceg filtera na sliku
    class ImageProcess
    {
        //---------------------- verziju bez paralelizacije -----------------------//
        public Bitmap processNonParallel(Bitmap bmp, string filter)
        {
            //filter koji je korisnik izabrao
            int filterNumber = returnFilterNumber(filter);

            //prolazimo kroz matricu piksela slike
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color oldColor = bmp.GetPixel(i, j);  //stara boja piksela na lokaciji (x,y)=(i,j)
                    Color newColor;

                    if (filterNumber == 1) //Red filter
                        newColor = Color.FromArgb(oldColor.R, 0, 0);
                    else if (filterNumber == 2) //Green filter
                        newColor = Color.FromArgb(0, oldColor.G, 0);
                    else if (filterNumber == 3) //Blue filter
                        newColor = Color.FromArgb(0, 0, oldColor.B);
                    else  //Invert filter
                        newColor = Color.FromArgb(255 - oldColor.R, 255 - oldColor.G, 255 - oldColor.B);

                    bmp.SetPixel(i, j, newColor);    //podesavanje nove boje piksela
                }
            }
            return bmp;   //vracamo bitmapu sa primijenjenim filterom
        }

        //---------------- Paralelno izvrsavanje procesa ---------------------//
        public Bitmap processParallel(Bitmap bmp, string filter)
        {
            Bitmap bmpFinal;  //nova bitmapa na koju je primijenjen odgovarajuci filter
            //moramo razbit pocetnu bitmapu na vise bitmapa da ne bi dolazilo do kolizije podataka na procesorskim nitima
            Bitmap[,] bmparray = splitImage(bmp);    //niz bitmapa nastao razbijanjem proslijedjene bitmape
            int filterNumber = returnFilterNumber(filter);    //za primjenu odgovarajuceg filtera

            //For koji sluzi da svaka procesorska nit uzme svoj dio niza i da izvrsi procesovanje na tom dijelu niza
            //Sa ovim forom prolazimo kroz redove niza Canvas-a
            Parallel.For(0, 4, x =>
            {
                for (int y = 0; y < 2; y++)
                {
                    Size size = new Size(bmparray[x, y].Width, bmparray[x, y].Height);  //size -> duzina i visina canvasa jednog dijela niza
                    for (int i = 0; i < size.Width; i++)
                    {
                        for (int j = 0; j < size.Height; j++)
                        {
                            Color oldColor = bmparray[x, y].GetPixel(i, j);
                            Color newColor;

                            if (filterNumber == 1) //Red filter
                                newColor = Color.FromArgb(oldColor.R, 0, 0);
                            else if(filterNumber == 2) //Green filter
                                newColor = Color.FromArgb(0, oldColor.G, 0);
                            else if(filterNumber == 3) //Blue filter
                                newColor = Color.FromArgb(0, 0, oldColor.B);
                            else  //Invert filter
                                newColor = Color.FromArgb(255 - oldColor.R, 255 - oldColor.G, 255 - oldColor.B);

                            bmparray[x, y].SetPixel(i, j, newColor);  //podesavanje boje piksela
                        }
                    }
                }
            });

            bmpFinal = MergeImages(bmparray);  //spajanje posebnih canvasa u jednu bitmapu
            return bmpFinal;
        }

        //u zavisnosti od filtera koji je korisnik izabrao vraca se odredjeni broj
        private int returnFilterNumber(string filter)
        {
            int filterNumber;
            switch (filter)
            {
                case "red":
                    filterNumber = 1;
                    break;
                case "green":
                    filterNumber = 2;
                    break;
                case "blue":
                    filterNumber = 3;
                    break;
                case "invert":
                    filterNumber = 4;
                    break;
                default:
                    filterNumber = 99;
                    break;
            }

            return filterNumber;
        }

        //Funkcija koja razbija bitmapu na vise bitmapa
        public Bitmap[,] splitImage(Bitmap bmp)
        {
            //duzina i visina jednog Canvas-a
            Size size = new Size(bmp.Width / 4, bmp.Height / 2);
            Bitmap[,] bmparray = new Bitmap[4, 2];      //Definisanje novog niza bitmpa

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    //definisanje novog pravugaonika (class: Rectangle) koji predstavlja dio bitmape koji je potrebno
                    //da se doda u niz bitmapa
                    Rectangle rctg = new Rectangle(
                        i * size.Width, j * size.Height, size.Width, size.Height);
                    //Definisanje nove bitmape u koje je potrebno ucrtati canvas, odnosno dio proslijedjene bitmape 
                    //koji smo definisali preko rctg
                    bmparray[i, j] = new Bitmap(size.Width, size.Height);

                    using (Graphics canvas = Graphics.FromImage(bmparray[i, j]))
                    {
                        //Ucrtavamo dio proslijedjene bitmape u niz bitmapa
                        canvas.DrawImage(bmp, new Rectangle(0, 0, size.Width, size.Height), rctg, GraphicsUnit.Pixel);
                    }
                }
            }
            return bmparray;  //vracamo niz bitmapa
        }

        //funkcija koja sluzi da na osnovu niza bitmapa rekonstruise pocetnu bitmapu od koje je taj niz nastao 
        private Bitmap MergeImages(Bitmap[,] bmparray)
        {
            //duzina i visina bitmape koja nastaje od niza bitmapas
            Size bitmapSize = new Size(bmparray[0, 0].Width + bmparray[1, 0].Width + bmparray[2, 0].Width + bmparray[3, 0].Width, Math.Max(bmparray[0, 0].Height, bmparray[0, 1].Height) * 2);
            Bitmap bitmap = new Bitmap(bitmapSize.Width, bitmapSize.Height);

            //spajanje u jednu bitmapu
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(bmparray[0, 0], 0, 0);
                g.DrawImage(bmparray[1, 0], bmparray[0, 0].Width, 0);
                g.DrawImage(bmparray[2, 0], bmparray[0, 0].Width + bmparray[1, 0].Width, 0);
                g.DrawImage(bmparray[3, 0], bmparray[0, 0].Width + bmparray[1, 0].Width + bmparray[2, 0].Width, 0);
                g.DrawImage(bmparray[0, 1], 0, bmparray[0, 0].Height);
                g.DrawImage(bmparray[1, 1], bmparray[0, 1].Width, bmparray[1, 0].Height);
                g.DrawImage(bmparray[2, 1], bmparray[0, 1].Width + bmparray[1, 1].Width, bmparray[2, 1].Height);
                g.DrawImage(bmparray[3, 1], bmparray[0, 1].Width + bmparray[1, 1].Width + bmparray[2, 1].Width, bmparray[3, 1].Height);
            }
            return bitmap;
        }
    }
}
