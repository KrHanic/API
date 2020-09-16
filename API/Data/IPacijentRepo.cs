using API.DTOs;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Data
{
    public interface IPacijentRepo
    {
        bool SaveChanges();

        IEnumerable<Pacijent> GetAllPacijenti();
        Pacijent GetPacijentByOIB(long OIB);
        long CreatePacijent(Pacijent pacijent);
        Task UpdatePacijent(Pacijent pacijent);
        Task RemovePacijentFromIsolation(Pacijent pacijent);
        Pacijent GetPacijentByID(long ID);
        List<Pacijent> GetPacijentiByStatus(int status);
        int? GetBrojPacijenataUIzolaciji(PacijentFilterDTO filter);
        int? GetBrojPacijenataVanIzolacije(PacijentFilterDTO filter);
        int? GetBrojPacijenataKojiSuPrekrsiliIzolaciju(PacijentFilterDTO filter);
        int? GetBrojPacijenataSaSimptomima(PacijentFilterDTO filter);
        List<PacijentReadDTO> GetPacijenteKojiSuPrekrsiliIzolaciju(PacijentFilterDTO filter);
        List<PacijentReadDTO> GetPacijenteKojiImajuSimptome(PacijentFilterDTO filter);
    }
}
