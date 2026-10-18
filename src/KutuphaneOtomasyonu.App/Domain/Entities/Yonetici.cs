using KutuphaneOtomasyonu.App.Domain.Entities.Base;

namespace KutuphaneOtomasyonu.App.Domain.Entities;

/// <summary>
/// Kütüphane personeli/yönetici sınıfı (UML: Yonetici). Kullanici sınıfından türer.
/// </summary>
public class Yonetici : Kullanici
{
    public string PersonelKodu { get; private set; }
    public string YetkiSeviyesi { get; private set; }

    public Yonetici(int id, string ad, string soyad, string email, string sifre, string personelKodu, string yetkiSeviyesi = "Admin")
        : base(id, ad, soyad, email, sifre)
    {
        PersonelKodu = personelKodu;
        YetkiSeviyesi = yetkiSeviyesi;
    }

    public override string ToString()
    {
        return $"[Yönetici] Kod: {PersonelKodu} | {TamAd} | Yetki: {YetkiSeviyesi}";
    }
}
