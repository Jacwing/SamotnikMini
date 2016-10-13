using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SamotnikMini
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Współrzędne[] pozycjaStartowaPionków = new Współrzędne[]
            {
                new Współrzędne(0,0),
                new Współrzędne(1,0),
                new Współrzędne(2,0),
                new Współrzędne(3,0),
                new Współrzędne(3,1),
                new Współrzędne(3,2),
                new Współrzędne(3,3),
                new Współrzędne(2,3),
                new Współrzędne(1,3),
                new Współrzędne(0,3)
            };

            Plansza plansza = new Plansza(pozycjaStartowaPionków, 4);
            int numerPozycji = 1;
            foreach (var następnaPlansza in plansza.PobierzKolejneUstawienieStartowe())
            {
                Console.WriteLine("Pozycja startowa nr " + numerPozycji++);
                następnaPlansza.Wypisz();
                Console.WriteLine();
                RekurencjaPlansza(następnaPlansza, 0, null, "");
            }
            Console.ReadLine();
        }


        static void RekurencjaPlansza(Plansza plansza, int ruchy, Współrzędne wsporzędnaOstaniegoPunktu, string sciezkaBicia)
        {
            foreach (Współrzędne pozycja in plansza.PobierzKolejnaPozycjęPionka())
            {
                foreach (Kierunek kierunek in plansza.PobierzNastępnyKierunkBicia(pozycja))
                {
                    string kolejneBicia = sciezkaBicia + String.Format(" ({0},{1},{2}){3}", pozycja.X, pozycja.Y, kierunek, Environment.NewLine);
                    Plansza planszaTymczasowa = plansza.Kopia();
                    Współrzędne tymczasowaWspółrzędna = planszaTymczasowa.WykonajBicie(pozycja, kierunek);
                    if ((wsporzędnaOstaniegoPunktu == null) || !wsporzędnaOstaniegoPunktu.Equals(pozycja))
                        ruchy++;
                    if ((planszaTymczasowa.LiczbaPionkówNaPlanszy() == 1) && (ruchy < 8))
                    {
                        Console.WriteLine("Liczba ruchów = " + ruchy + Environment.NewLine + kolejneBicia);
                    }
                    RekurencjaPlansza(planszaTymczasowa, ruchy, tymczasowaWspółrzędna, kolejneBicia);
                }
            }
        }
    }

    class Plansza
    {
        bool[,] ustawiniePionków;

        public Plansza(Współrzędne[] współrzędne, int rozmiarPlanszy)
        {
            if (współrzędne.Any(r => r.X >= rozmiarPlanszy))
                throw new System.ArgumentException("Wielkość współrzędnych nie może być większa od rozmiaru planszy.");
            if (współrzędne.Any(r => r.Y >= rozmiarPlanszy))
                throw new System.ArgumentException("Wielkość współrzędnych nie może być większa od rozmiaru planszy.");

            ustawiniePionków = new bool[rozmiarPlanszy, rozmiarPlanszy];
            foreach (var item in współrzędne)
            {
                ustawiniePionków[item.X, item.Y] = true;
            }
        }

        public bool CzyMożnaWykonaćBicie(Współrzędne współrzędne, Kierunek kierunek)
        {
            if (!ustawiniePionków[współrzędne.X, współrzędne.Y])
                throw new ArgumentException("W tym miejscu nie ma pionka");

            switch (kierunek)
            {
                case Kierunek.Gora:
                    if ((współrzędne.X - 2 > -1) && this.ustawiniePionków[współrzędne.X - 1, współrzędne.Y] && !this.ustawiniePionków[współrzędne.X - 2, współrzędne.Y])
                        return true;
                    else
                        return false;
                case Kierunek.Dół:
                    if ((współrzędne.X + 2 < this.ustawiniePionków.GetLength(0)) && this.ustawiniePionków[współrzędne.X + 1, współrzędne.Y] && !this.ustawiniePionków[współrzędne.X + 2, współrzędne.Y])
                        return true;
                    else
                        return false;
                case Kierunek.Lewo:
                    if ((współrzędne.Y - 2 > -1) && this.ustawiniePionków[współrzędne.X, współrzędne.Y - 1] && !this.ustawiniePionków[współrzędne.X, współrzędne.Y - 2])
                        return true;
                    else
                        return false;
                case Kierunek.Prawo:
                    if ((współrzędne.Y + 2 < this.ustawiniePionków.GetLength(0)) && this.ustawiniePionków[współrzędne.X, współrzędne.Y + 1] && !this.ustawiniePionków[współrzędne.X, współrzędne.Y + 2])
                        return true;
                    else
                        return false;
                default:
                    return false;
            }
        }

        public IEnumerable<Kierunek> PobierzNastępnyKierunkBicia(Współrzędne współrzędne)
        {
            foreach (Kierunek kierunek in Enum.GetValues(typeof(Kierunek)))
            {
                if (CzyMożnaWykonaćBicie(współrzędne, kierunek))
                    yield return kierunek;
            }
        }


        public int LiczbaPionkówNaPlanszy()
        {
            int wynik = 0;
            foreach (var item in this.ustawiniePionków)
            {
                if (item)
                    wynik++;
            }
            return wynik;
        }

        public Współrzędne WykonajBicie(Współrzędne współrzędne, Kierunek kierunek)
        {
            if (!CzyMożnaWykonaćBicie(współrzędne, kierunek))
                throw new ArgumentException("Mie można wykonac takiego bicia");

            switch (kierunek)
            {
                case Kierunek.Gora:
                    this.ustawiniePionków[współrzędne.X, współrzędne.Y] = false;
                    this.ustawiniePionków[współrzędne.X - 1, współrzędne.Y] = false;
                    this.ustawiniePionków[współrzędne.X - 2, współrzędne.Y] = true;
                    return new Współrzędne(współrzędne.X - 2, współrzędne.Y);
                case Kierunek.Dół:
                    this.ustawiniePionków[współrzędne.X, współrzędne.Y] = false;
                    this.ustawiniePionków[współrzędne.X + 1, współrzędne.Y] = false;
                    this.ustawiniePionków[współrzędne.X + 2, współrzędne.Y] = true;
                    return new Współrzędne(współrzędne.X + 2, współrzędne.Y);
                case Kierunek.Lewo:
                    this.ustawiniePionków[współrzędne.X, współrzędne.Y] = false;
                    this.ustawiniePionków[współrzędne.X, współrzędne.Y - 1] = false;
                    this.ustawiniePionków[współrzędne.X, współrzędne.Y - 2] = true;
                    return new Współrzędne(współrzędne.X, współrzędne.Y - 2);
                case Kierunek.Prawo:
                    this.ustawiniePionków[współrzędne.X, współrzędne.Y] = false;
                    this.ustawiniePionków[współrzędne.X, współrzędne.Y + 1] = false;
                    this.ustawiniePionków[współrzędne.X, współrzędne.Y + 2] = true;
                    return new Współrzędne(współrzędne.X, współrzędne.Y + 2);
                default:
                    return null;
            }

        }

        public void WyłączPionek(Współrzędne współrzędne)
        {
            this.ustawiniePionków[współrzędne.X, współrzędne.Y] = false;
        }

        public void WłączPionek(Współrzędne współrzędne)
        {
            this.ustawiniePionków[współrzędne.X, współrzędne.Y] = true;
        }

        public IEnumerable<Plansza> PobierzKolejneUstawienieStartowe()
        {
            foreach (var pionek in this.PobierzKolejnaPozycjęPionka())
            {
                foreach (var pustePole in this.PobierzKolejnaPustąPozycję())
                {
                    Plansza temp = this.Kopia();
                    temp.WłączPionek(pustePole);
                    temp.WyłączPionek(pionek);
                    yield return temp;
                }
            }
        }

        public IEnumerable<Współrzędne> PobierzKolejnaPozycjęPionka()
        {
            for (int i = 0; i < ustawiniePionków.GetLength(0); i++)
            {
                for (int j = 0; j < ustawiniePionków.GetLength(1); j++)
                {
                    if (ustawiniePionków[i, j])
                        yield return new Współrzędne(i, j);
                }
            }
        }

        public IEnumerable<Współrzędne> PobierzKolejnaPustąPozycję()
        {
            for (int i = 0; i < ustawiniePionków.GetLength(0); i++)
            {
                for (int j = 0; j < ustawiniePionków.GetLength(1); j++)
                {
                    if (!ustawiniePionków[i, j])
                        yield return new Współrzędne(i, j);
                }
            }
        }

        public Plansza Kopia()
        {
            return new Plansza(this.PobierzKolejnaPozycjęPionka().ToArray(), this.ustawiniePionków.GetLength(0));
        }

        public void Wypisz()
        {
            for (int i = 0; i < ustawiniePionków.GetLength(0); i++)
            {
                if (i == 0)
                {
                    Console.Write("╔");
                    for (int j = 0; j < ustawiniePionków.GetLength(0) - 1; j++)
                    {
                        Console.Write("═══╦");
                    }
                    Console.WriteLine("═══╗");
                }
                for (int j = 0; j < ustawiniePionków.GetLength(0); j++)
                {
                    Console.Write("║" + (ustawiniePionków[i, j] ? " O " : "   ").ToString());
                }
                Console.WriteLine("║");
                if (i != ustawiniePionków.GetLength(0) - 1)
                {
                    Console.Write("╠");
                    for (int j = 0; j < ustawiniePionków.GetLength(0) - 1; j++)
                    {
                        Console.Write("═══╬");
                    }
                    Console.WriteLine("═══╣");
                }
                else
                {
                    Console.Write("╚");
                    for (int j = 0; j < ustawiniePionków.GetLength(0) - 1; j++)
                    {
                        Console.Write("═══╩");
                    }
                    Console.WriteLine("═══╝");
                }
            }
        }
    }

    class Współrzędne : IEquatable<Współrzędne>
    {
        int x, y;

        public int X
        {
            get { return x; }
            private set { x = value; }
        }

        public int Y
        {
            get { return y; }
            private set { y = value; }
        }

        public Współrzędne(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public bool Equals(Współrzędne other)
        {
            if ((this.X == other.X) && (this.Y == other.Y))
                return true;
            else
                return false;
        }
    }

    enum Kierunek
    {
        Gora,
        Dół,
        Lewo,
        Prawo
    }
}

