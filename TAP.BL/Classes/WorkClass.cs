using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TAP.BL.Data;
using TAP.BL.Interfaces;

namespace TAP.BL.Classes
{
    public class WorkClass : IWorkClass
    {
        private List<Emiter> emiteriA;
        private List<Emiter> emiteriB;
        private List<Emiter> emiteriC;
        private readonly Random randomNumberGenerator = new Random();

        private int RadiusA { get; set; }
        private int RadiusB { get; set; }
        private int RadiusC { get; set; }

        public WorkClass()
        {
            RadiusA = 10;
            RadiusB = 40;
            RadiusC = 25;
            emiteriA = SetujEmitere(KlasaEmitera.KlasaA);
            emiteriB = SetujEmitere(KlasaEmitera.KlasaB);
            emiteriC = SetujEmitere(KlasaEmitera.KlasaC);
        }
      
        public List<Korisnik> DoWork(int idTaska, Korisnik[] korisnici, int pocetak, int kraj, CancellationToken cancellationToken, IProgress<ReportProgressPodaci> progress)
        {
            List<Korisnik> korisniciKojinemajuDomet = new List<Korisnik>();

            int procenatBrojac = 0;

            for (int i = pocetak; i < kraj; i++)
            {
                procenatBrojac++;
                if(progress != null)
                {
                    if(procenatBrojac % 50 == 0)
                    {
                        progress.Report(new ReportProgressPodaci()
                        {
                            IdTaska = idTaska,
                            Procenat = (procenatBrojac * 100) / 250
                        });
                    }
                }

                int j;
                for (j = 0; j < 30; j++)
                {
                    if (j < 10)
                        if (ProveriKorisnika(korisnici[i].X, korisnici[j].Y, emiteriA[j], RadiusA)) 
                            break;
                    
                    if (j < 20)
                        if (ProveriKorisnika(korisnici[i].X, korisnici[j].Y, emiteriB[j], RadiusB)) 
                            break;

                    if (ProveriKorisnika(korisnici[i].X, korisnici[j].Y, emiteriC[j], RadiusC)) 
                        break;
                }

                if (j == 30)
                    korisniciKojinemajuDomet.Add(new Korisnik()
                    {
                        Id = korisnici[i].Id,
                        X = korisnici[i].X,
                        Y = korisnici[i].Y
                    });
            }

            return korisniciKojinemajuDomet;
        }

        private bool ProveriKorisnika(double x, double y, Emiter emiter, double radius)
        {
            double rez = Math.Sqrt(Math.Pow(emiter.X - x, 2) + Math.Pow(emiter.Y - y, 2));
            if (rez <= radius)
                return true;

            return false;
        }

        public List<Emiter> SetujEmitere(KlasaEmitera klasaEmitera)
        {
            List<Emiter> emiteri = new List<Emiter>();

            int n = 0;

            switch (klasaEmitera)
            {
                case KlasaEmitera.KlasaA:
                    n = 10;
                    break;
                case KlasaEmitera.KlasaB:
                    n = 20;
                    break;
                case KlasaEmitera.KlasaC:
                    n = 30;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < n; i++)
            {
                emiteri.Add(new Emiter()
                {
                    X = randomNumberGenerator.Next(-1000, 1000),
                    Y = randomNumberGenerator.Next(-1000, 1000)
                }); 
            }

            return emiteri;
        }

        public void ReportProgress(ReportProgressPodaci podaci)
        {
            Console.WriteLine($"Id Taska: { podaci.IdTaska }, završio je: { podaci.Procenat} %");
        }
    }
}
