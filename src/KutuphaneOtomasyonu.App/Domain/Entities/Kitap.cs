namespace KutuphaneOtomasyonu.App.Domain.Entities;

/// <summary>
/// Kütüphanedeki kitap varlığı (UML: Kitap).
/// </summary>
public class Kitap
{
    public string ISBN { get; private set; }
    public string Baslik { get; private set; }
    public string Yazar { get; private set; }
    public int YayinYili { get; private set; }
    public int ToplamStok { get; private set; }
    public int MevcutStok { get; private set; }

    public Kitap(string isbn, string baslik, string yazar, int yayinYili, int toplamStok)
    {
        if (string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("ISBN boş olamaz.", nameof(isbn));

        if (toplamStok < 0)
            throw new ArgumentException("Toplam stok negatif olamaz.", nameof(toplamStok));

        ISBN = isbn;
        Baslik = baslik;
        Yazar = yazar;
        YayinYili = yayinYili;
        ToplamStok = toplamStok;
        MevcutStok = toplamStok; // Başlangıçta hepsi rafta
    }

    public bool MusaitMi() => MevcutStok > 0;

    public bool StokAzalt()
    {
        if (MevcutStok > 0)
        {
            MevcutStok--;
            return true;
        }
        return false;
    }

    public void StokArtir()
    {
        if (MevcutStok < ToplamStok)
        {
            MevcutStok++;
        }
    }

    public void StokMiktariniGuncelle(int yeniToplamStok)
    {
        if (yeniToplamStok < 0)
            throw new ArgumentException("Stok negatif olamaz.");

        int odunctekiAdet = ToplamStok - MevcutStok;
        if (yeniToplamStok < odunctekiAdet)
            throw new InvalidOperationException($"Şu anda {odunctekiAdet} adet kitap ödünçte olduğu için toplam stok bu değerin altına düşürülemez.");

        ToplamStok = yeniToplamStok;
        MevcutStok = yeniToplamStok - odunctekiAdet;
    }

    public override string ToString()
    {
        return $"[{ISBN}] \"{Baslik}\" - {Yazar} ({YayinYili}) | Stok: {MevcutStok}/{ToplamStok}";
    }
}
