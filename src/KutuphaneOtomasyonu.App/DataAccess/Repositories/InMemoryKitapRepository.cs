using KutuphaneOtomasyonu.App.DataAccess.Interfaces;
using KutuphaneOtomasyonu.App.Domain.Entities;

namespace KutuphaneOtomasyonu.App.DataAccess.Repositories;

public class InMemoryKitapRepository : IKitapRepository
{
    private readonly List<Kitap> _kitaplar = new();

    public InMemoryKitapRepository()
    {
        SeedData();
    }

    private void SeedData()
    {
        _kitaplar.Add(new Kitap("978-975-07-2645-3", "Sefiller", "Victor Hugo", 1862, 5));
        _kitaplar.Add(new Kitap("978-975-07-1853-3", "Suç ve Ceza", "Fyodor Dostoyevski", 1866, 4));
        _kitaplar.Add(new Kitap("978-975-10-2000-0", "Nutuk", "Mustafa Kemal Atatürk", 1927, 10));
        _kitaplar.Add(new Kitap("978-975-07-3889-0", "Dönüşüm", "Franz Kafka", 1915, 3));
        _kitaplar.Add(new Kitap("978-975-570-585-9", "Simyacı", "Paulo Coelho", 1988, 6));
    }

    public IEnumerable<Kitap> GetAll() => _kitaplar.OrderBy(k => k.Baslik);

    public Kitap? GetByIsbn(string isbn)
    {
        return _kitaplar.FirstOrDefault(k => k.ISBN.Equals(isbn.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Kitap> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return GetAll();

        return _kitaplar.Where(k =>
            k.Baslik.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            k.Yazar.Contains(query, StringComparison.OrdinalIgnoreCase) ||
            k.ISBN.Contains(query, StringComparison.OrdinalIgnoreCase));
    }

    public void Add(Kitap kitap)
    {
        if (GetByIsbn(kitap.ISBN) != null)
            throw new InvalidOperationException($"'{kitap.ISBN}' numaralı kitap zaten sistemde kayıtlı.");

        _kitaplar.Add(kitap);
    }

    public void Update(Kitap kitap)
    {
        var existing = GetByIsbn(kitap.ISBN);
        if (existing == null)
            throw new KeyNotFoundException($"'{kitap.ISBN}' numaralı kitap bulunamadı.");

        // Referans güncellemeleri nesne üzerinde metotlarla yapıldığı için listede kalması yeterlidir.
    }
}
