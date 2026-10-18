using KutuphaneOtomasyonu.App.DataAccess.Interfaces;
using KutuphaneOtomasyonu.App.Domain.Entities;
using KutuphaneOtomasyonu.App.Domain.Enums;

namespace KutuphaneOtomasyonu.App.DataAccess.Repositories;

public class InMemoryOduncRepository : IOduncRepository
{
    private readonly List<OduncIslemi> _oduncler = new();

    public InMemoryOduncRepository(IKullaniciRepository kullaniciRepo, IKitapRepository kitapRepo)
    {
        SeedData(kullaniciRepo, kitapRepo);
    }

    private void SeedData(IKullaniciRepository kullaniciRepo, IKitapRepository kitapRepo)
    {
        // Gecikme raporlama özelliğini test etmek için geçmiş tarihli 1 adet ödünç kaydı ekleyelim:
        var mehmet = kullaniciRepo.GetOgrenciById(103);
        var donusum = kitapRepo.GetByIsbn("978-975-07-3889-0");

        if (mehmet != null && donusum != null && donusum.StokAzalt())
        {
            // 25 gün önce ödünç alınmış, 11 gün gecikmiş işlem
            var gecikmisIslem = new OduncIslemi(
                "ODNC-SEED-001",
                mehmet,
                donusum,
                DateTime.Now.AddDays(-25),
                14
            );
            _oduncler.Add(gecikmisIslem);
        }
    }

    public IEnumerable<OduncIslemi> GetAll() => _oduncler;

    public OduncIslemi? GetById(string islemId)
    {
        return _oduncler.FirstOrDefault(o => o.IslemId.Equals(islemId.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<OduncIslemi> GetByOgrenciId(int ogrenciId)
    {
        return _oduncler.Where(o => o.Ogrenci.Id == ogrenciId);
    }

    public IEnumerable<OduncIslemi> GetAktifOduncler()
    {
        return _oduncler.Where(o => o.Durum == OduncDurumu.Aktif);
    }

    public void Add(OduncIslemi islem)
    {
        if (GetById(islem.IslemId) != null)
            throw new InvalidOperationException($"'{islem.IslemId}' numaralı ödünç işlemi zaten mevcut.");

        _oduncler.Add(islem);
    }

    public void Update(OduncIslemi islem)
    {
        var existing = GetById(islem.IslemId);
        if (existing == null)
            throw new KeyNotFoundException($"'{islem.IslemId}' numaralı işlem bulunamadı.");
    }
}
