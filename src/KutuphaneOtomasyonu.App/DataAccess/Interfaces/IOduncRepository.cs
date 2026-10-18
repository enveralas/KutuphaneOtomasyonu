using KutuphaneOtomasyonu.App.Domain.Entities;

namespace KutuphaneOtomasyonu.App.DataAccess.Interfaces;

public interface IOduncRepository
{
    IEnumerable<OduncIslemi> GetAll();
    OduncIslemi? GetById(string islemId);
    IEnumerable<OduncIslemi> GetByOgrenciId(int ogrenciId);
    IEnumerable<OduncIslemi> GetAktifOduncler();
    void Add(OduncIslemi islem);
    void Update(OduncIslemi islem);
}
