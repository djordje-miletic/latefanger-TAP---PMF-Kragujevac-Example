using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TAP.BL.Classes;
using TAP.BL.Data;

namespace TAP.ConsoleApp
{
    class Program
    {
        public static async System.Threading.Tasks.Task Main(string[] args)
        {
            Console.WriteLine("Krećem sa radom");
            Korisnik[] korisnici = new Korisnik[1000];

            KreirajKorisnike(korisnici);

            WorkClass workClass = new WorkClass();

            //Kreiranje cancelation tokena
            CancellationTokenSource source = new CancellationTokenSource();
            CancellationToken token = source.Token;

            //Kreiramo listu koja će sadržati sve naše taskove
            List<Task<List<Korisnik>>> tasks = new List<Task<List<Korisnik>>>();
            TaskFactory factory = new TaskFactory(token);
            var progressIndicator = new Progress<ReportProgressPodaci>(workClass.ReportProgress);


            for (int i = 0; i < 4; i++)
            {
                int idTaska = i + 1;

                tasks.Add(factory.StartNew(() => 
                {
                    List<Korisnik> rezultat = new List<Korisnik>();

                    switch (idTaska)
                    {
                        case 1:
                            rezultat = workClass.DoWork(idTaska, korisnici, 0, 250, token, progressIndicator);
                            break;
                        case 2:
                            rezultat = workClass.DoWork(idTaska, korisnici, 251, 500, token, progressIndicator);
                            break;
                        case 3:
                            rezultat = workClass.DoWork(idTaska, korisnici, 501, 750, token, progressIndicator);
                            break;
                        case 4:
                            rezultat = workClass.DoWork(idTaska, korisnici, 751, 1000, token, progressIndicator);
                            break;
                        default:
                            break;
                    }

                    return rezultat;
                }));
            }

            try
            {
                Task kolektor = factory.ContinueWhenAll(tasks.ToArray(), (rezultati) =>
                {
                    Console.WriteLine("Sakupljam sve taskove...");

                    foreach (var t in rezultati)
                    {
                        foreach (var rezultat in t.Result)
                        {
                            Console.WriteLine($"Korisnik: { rezultat.Id }, sa kordinatama: X: { rezultat.X }, Y: { rezultat.Y } ne pripada nijednom emiteru");
                        }
                    }
                }, token);

                kolektor.Wait();
            }
            catch (AggregateException ae)
            {
                foreach (Exception e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException)
                        Console.WriteLine("Došlo je do prekida u izvršavanju taska: {0}",
                                          ((TaskCanceledException)e).Message);
                    else
                        Console.WriteLine("Exception: " + e.GetType().Name);
                }
            }
            finally
            {
                //Na kraju uništavamo token koji smo kreirali
                source.Dispose();
            }

            Console.WriteLine("Završio sam!");
        }

        public static void KreirajKorisnike(Korisnik[] korisnici)
        {
            Random randomNumberGenerator = new Random();

            for (int i = 0; i < 1000; i++)
            {
                korisnici[i] = new Korisnik()
                {
                    Id = i,
                    X = randomNumberGenerator.Next(-1000, 1000),
                    Y = randomNumberGenerator.Next(-1000, 1000)
                };
            }
        }
    }
}
