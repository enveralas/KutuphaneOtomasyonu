namespace KutuphaneOtomasyonu.App.Domain.Entities.Base;

/// <summary>
/// Kütüphane sistemindeki tüm kullanıcılar için temel soyut sınıf (UML: Kullanici).
/// </summary>
public abstract class Kullanici
{
    public int Id { get; protected set; }
    public string Ad { get; protected set; } = string.Empty;
    public string Soyad { get; protected set; } = string.Empty;
    public string Email { get; protected set; } = string.Empty;
    public string Sifre { get; protected set; } = string.Empty;

    public string TamAd => $"{Ad} {Soyad}";

    protected Kullanici(int id, string ad, string soyad, string email, string sifre)
    {
        Id = id;
        Ad = ad;
        Soyad = soyad;
        Email = email;
        Sifre = sifre;
    }

    public virtual bool GirisYap(string email, string sifre)
    {
        return Email.Equals(email, StringComparison.OrdinalIgnoreCase) && Sifre == sifre;
    }

    public virtual void CikisYap()
    {
        // Oturum kapatma mantığı
    }

    public override string ToString()
    {
        return $"[{Id}] {TamAd} ({Email})";
    }
}
