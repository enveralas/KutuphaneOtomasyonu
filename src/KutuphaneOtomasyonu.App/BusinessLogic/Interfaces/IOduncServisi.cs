using KutuphaneOtomasyonu.App.Domain.Entities;

namespace KutuphaneOtomasyonu.App.BusinessLogic.Interfaces;

public interface IOduncServisi
{
    (bool Basarili, string Mesaj, OduncIslemi? Islem) KitapOduncAl(int ogrenciId, string isbn, int gun = 14);
    (bool Basarili, string Mesaj, double Ceza) KitapIadeEt(string islemId);
    IEnumerable<OduncIslemi> OgrencininOduncleriniGetir(int ogrenciId);
    IEnumerable<OduncIslemi> OgrencininAktifOduncleriniGetir(int ogrenciId);
}
