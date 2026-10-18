using KutuphaneOtomasyonu.App.Domain.Entities.Base;

namespace KutuphaneOtomasyonu.App.Domain.Entities;

/// <summary>
/// Öğrenci üye sınıfı (UML: Ogrenci). Kullanici sınıfından türer.
/// </summary>
public class Ogrenci : Kullanici
{
    public string OgrenciNo { get; private set; }
    public string Bolum { get; private set; }
    public int OduncLimiti { get; private set; }

    public Ogrenci(int id, string ad, string soyad, string email, string sifre, string ogrenciNo, string bolum, int oduncLimiti = 3)
        : base(id, ad, soyad, email, sifre)
    {
        OgrenciNo = ogrenciNo;
        Bolum = bolum;
        OduncLimiti = oduncLimiti;
    }

    public void OduncLimitiniGuncelle(int yeniLimit)
    {
        if (yeniLimit > 0)
        {
            OduncLimiti = yeniLimit;
        }
    }

    public override string ToString()
    {
        return $"[Öğrenci] No: {OgrenciNo} | {TamAd} | Bölüm: {Bolum} | Limit: {OduncLimiti}";
    }
}
