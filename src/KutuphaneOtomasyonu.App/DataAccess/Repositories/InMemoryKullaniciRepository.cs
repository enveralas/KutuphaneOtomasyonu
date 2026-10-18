using KutuphaneOtomasyonu.App.DataAccess.Interfaces;
using KutuphaneOtomasyonu.App.Domain.Entities;
using KutuphaneOtomasyonu.App.Domain.Entities.Base;

namespace KutuphaneOtomasyonu.App.DataAccess.Repositories;

public class InMemoryKullaniciRepository : IKullaniciRepository
{
    private readonly List<Ogrenci> _ogrenciler = new();
    private readonly List<Yonetici> _yoneticiler = new();

    public InMemoryKullaniciRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        _ogrenciler.Add(new Ogrenci(101, "Ali", "Yılmaz", "ali.yilmaz@ogr.edu.tr", "1234", "2023105021", "Bilgisayar Mühendisliği", 3));
        _ogrenciler.Add(new Ogrenci(102, "Zeynep", "Kaya", "zeynep.kaya@ogr.edu.tr", "1234", "2023105022", "Endüstri Mühendisliği", 3));
        _ogrenciler.Add(new Ogrenci(103, "Mehmet", "Öztürk", "mehmet.ozturk@ogr.edu.tr", "1234", "2023105023", "Hukuk", 3));

        _yoneticiler.Add(new Yonetici(201, "Ayşe", "Demir", "ayse.demir@uni.edu.tr", "admin123", "YON-001", "Baş Kütüphaneci"));
    }

    public Ogrenci? GetOgrenciById(int id) => _ogrenciler.FirstOrDefault(o => o.Id == id);

    public Ogrenci? GetOgrenciByNo(string ogrenciNo) =>
        _ogrenciler.FirstOrDefault(o => o.OgrenciNo.Equals(ogrenciNo.Trim(), StringComparison.OrdinalIgnoreCase));

    public IEnumerable<Ogrenci> GetAllOgrenciler() => _ogrenciler;

    public Yonetici? GetYoneticiById(int id) => _yoneticiler.FirstOrDefault(y => y.Id == id);

    public Kullanici? GetByEmail(string email)
    {
        Kullanici? user = _ogrenciler.FirstOrDefault(o => o.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
        return user ?? _yoneticiler.FirstOrDefault(y => y.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void AddOgrenci(Ogrenci ogrenci)
    {
        if (GetOgrenciByNo(ogrenci.OgrenciNo) != null)
            throw new InvalidOperationException($"'{ogrenci.OgrenciNo}' numaralı öğrenci zaten mevcut.");

        _ogrenciler.Add(ogrenci);
    }
}
