using KutuphaneOtomasyonu.App.Domain.Entities;

namespace KutuphaneOtomasyonu.App.BusinessLogic.Interfaces;

public interface IRaporServisi
{
    IEnumerable<OduncIslemi> GecikenKitaplariRaporla();
    (int ToplamKitapCesidi, int ToplamFizikselKitap, int OdunctekiKitap, int GecikenAdet, double ToplamTahakkukEdenCeza) OzetIstatistikleriGetir();
}
