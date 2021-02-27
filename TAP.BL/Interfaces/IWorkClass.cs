using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TAP.BL.Data;

namespace TAP.BL.Interfaces
{
    public interface IWorkClass
    {
        List<Korisnik> DoWork(int idTaska, Korisnik[] korisnici, int pocetak, int kraj, CancellationToken cancellationToken, IProgress<ReportProgressPodaci> progress);
    }
}
