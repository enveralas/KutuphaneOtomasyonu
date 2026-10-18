using KutuphaneOtomasyonu.App.Domain.Enums;

namespace KutuphaneOtomasyonu.App.Domain.Entities;

/// <summary>
/// Öğrenci ile Kitap arasındaki ödünç alma işlemini temsil eden varlık (UML: OduncIslemi).
/// </summary>
public class OduncIslemi
{
    public string IslemId { get; private set; }
    public Ogrenci Ogrenci { get; private set; }
    public Kitap Kitap { get; private set; }
    public DateTime OduncTarihi { get; private set; }
    public DateTime SonTeslimTarihi { get; private set; }
    public DateTime? IadeTarihi { get; private set; }
    public OduncDurumu Durum { get; private set; }

    public const double GunlukCezaTutari = 5.0; // Günlük 5 TL gecikme cezası

    public OduncIslemi(string islemId, Ogrenci ogrenci, Kitap kitap, DateTime oduncTarihi, int oduncGunu = 14)
    {
        IslemId = islemId;
        Ogrenci = ogrenci ?? throw new ArgumentNullException(nameof(ogrenci));
        Kitap = kitap ?? throw new ArgumentNullException(nameof(kitap));
        OduncTarihi = oduncTarihi;
        SonTeslimTarihi = oduncTarihi.AddDays(oduncGunu);
        IadeTarihi = null;
        Durum = OduncDurumu.Aktif;
    }

    /// <summary>
    /// Gecikme gün sayısını hesaplar.
    /// </summary>
    public int GecikmeGunuHesapla(DateTime? referansTarih = null)
    {
        DateTime kontrolTarihi = IadeTarihi ?? referansTarih ?? DateTime.Now;

        if (kontrolTarihi > SonTeslimTarihi)
        {
            return (int)(kontrolTarihi.Date - SonTeslimTarihi.Date).TotalDays;
        }

        return 0;
    }

    /// <summary>
    /// Gecikme durumunda ceza tutarını hesaplar.
    /// </summary>
    public double CezaUcretiHesapla(DateTime? referansTarih = null)
    {
        int gecikmeGunu = GecikmeGunuHesapla(referansTarih);
        return gecikmeGunu * GunlukCezaTutari;
    }

    /// <summary>
    /// Kitabın teslim alınma işlemini tamamlar.
    /// </summary>
    public void TeslimAl(DateTime iadeTarihi)
    {
        IadeTarihi = iadeTarihi;
        int gecikme = GecikmeGunuHesapla(iadeTarihi);
        Durum = gecikme > 0 ? OduncDurumu.Gecikmis : OduncDurumu.IadeEdildi;
    }

    public override string ToString()
    {
        string durumStr = Durum switch
        {
            OduncDurumu.Aktif => "Aktif (Teslim Bekliyor)",
            OduncDurumu.IadeEdildi => $"İade Edildi ({IadeTarihi:dd.MM.yyyy})",
            OduncDurumu.Gecikmis => $"Gecikmeli İade Edildi (Ceza: {CezaUcretiHesapla():C2})",
            _ => Durum.ToString()
        };

        return $"[İşlem: {IslemId}] Ögr: {Ogrenci.TamAd} | Kitap: \"{Kitap.Baslik}\" | Son Teslim: {SonTeslimTarihi:dd.MM.yyyy} | Durum: {durumStr}";
    }
}
