using KutuphaneOtomasyonu.App.BusinessLogic.Interfaces;
using KutuphaneOtomasyonu.App.DataAccess.Interfaces;
using KutuphaneOtomasyonu.App.Domain.Entities;
using KutuphaneOtomasyonu.App.Domain.Enums;

namespace KutuphaneOtomasyonu.App.BusinessLogic.Services;

public class RaporServisi : IRaporServisi
{
    private readonly IOduncRepository _oduncRepository;
    private readonly IKitapRepository _kitapRepository;

    public RaporServisi(IOduncRepository oduncRepository, IKitapRepository kitapRepository)
    {
        _oduncRepository = oduncRepository;
        _kitapRepository = kitapRepository;
    }

    public IEnumerable<OduncIslemi> GecikenKitaplariRaporla()
    {
        // Aktif olup son teslim tarihi geçmiş olan kayıtları getir
        return _oduncRepository.GetAktifOduncler()
            .Where(o => o.GecikmeGunuHesapla() > 0)
            .OrderByDescending(o => o.GecikmeGunuHesapla());
    }

    public (int ToplamKitapCesidi, int ToplamFizikselKitap, int OdunctekiKitap, int GecikenAdet, double ToplamTahakkukEdenCeza) OzetIstatistikleriGetir()
    {
        var kitaplar = _kitapRepository.GetAll().ToList();
        var aktifOduncler = _oduncRepository.GetAktifOduncler().ToList();
        var gecikenler = GecikenKitaplariRaporla().ToList();

        int toplamCesit = kitaplar.Count;
        int toplamFiziksel = kitaplar.Sum(k => k.ToplamStok);
        int oduncteki = aktifOduncler.Count;
        int gecikenAdet = gecikenler.Count;
        double toplamCeza = gecikenler.Sum(g => g.CezaUcretiHesapla());

        return (toplamCesit, toplamFiziksel, oduncteki, gecikenAdet, toplamCeza);
    }
}
