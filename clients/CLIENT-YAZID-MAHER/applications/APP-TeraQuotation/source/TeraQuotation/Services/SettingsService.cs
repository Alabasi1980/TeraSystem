using Microsoft.EntityFrameworkCore;
using TeraQuotation.Data;
using TeraQuotation.Models;

namespace TeraQuotation.Services;

public class SettingsService : ISettingsService
{
    private readonly AppDbContext _db;

    public SettingsService(AppDbContext db)
    {
        _db = db;
    }

    // ---- Suppliers ----

    public async Task<List<Supplier>> GetAllSuppliersAsync()
    {
        return await _db.Suppliers.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Supplier> AddSupplierAsync(Supplier supplier)
    {
        supplier.CreatedAt = DateTime.UtcNow;
        _db.Suppliers.Add(supplier);
        await _db.SaveChangesAsync();
        return supplier;
    }

    public async Task UpdateSupplierAsync(Supplier supplier)
    {
        _db.Suppliers.Update(supplier);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteSupplierAsync(int id)
    {
        var supplier = await _db.Suppliers.FindAsync(id);
        if (supplier != null)
        {
            _db.Suppliers.Remove(supplier);
            await _db.SaveChangesAsync();
        }
    }

    // ---- Items ----

    public async Task<List<Item>> GetAllItemsAsync()
    {
        return await _db.Items.OrderBy(i => i.Name).ToListAsync();
    }

    public async Task<List<Item>> SearchItemsAsync(string search)
    {
        if (string.IsNullOrWhiteSpace(search))
            return await GetAllItemsAsync();

        return await _db.Items
            .Where(i => i.Name.Contains(search) || (i.Description != null && i.Description.Contains(search)))
            .OrderBy(i => i.Name)
            .ToListAsync();
    }

    public async Task<Item> AddItemAsync(Item item)
    {
        item.CreatedAt = DateTime.UtcNow;
        item.UpdatedAt = DateTime.UtcNow;
        _db.Items.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task UpdateItemAsync(Item item)
    {
        item.UpdatedAt = DateTime.UtcNow;
        _db.Items.Update(item);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteItemAsync(int id)
    {
        var item = await _db.Items.FindAsync(id);
        if (item != null)
        {
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }
    }

    // ---- Signatures ----

    public async Task<List<Signature>> GetAllSignaturesAsync()
    {
        return await _db.Signatures.OrderBy(s => s.OrderIndex).ThenBy(s => s.Name).ToListAsync();
    }

    public async Task<Signature> AddSignatureAsync(Signature signature)
    {
        var maxOrder = await _db.Signatures.MaxAsync(s => (int?)s.OrderIndex) ?? -1;
        signature.OrderIndex = maxOrder + 1;
        signature.CreatedAt = DateTime.UtcNow;
        _db.Signatures.Add(signature);
        await _db.SaveChangesAsync();
        return signature;
    }

    public async Task DeleteSignatureAsync(int id)
    {
        var signature = await _db.Signatures.FindAsync(id);
        if (signature != null)
        {
            _db.Signatures.Remove(signature);
            await _db.SaveChangesAsync();
        }
    }

    public async Task MoveSignatureUpAsync(int id)
    {
        var current = await _db.Signatures.FindAsync(id);
        if (current == null || current.OrderIndex <= 0) return;

        var previous = await _db.Signatures
            .Where(s => s.OrderIndex < current.OrderIndex)
            .OrderByDescending(s => s.OrderIndex)
            .FirstOrDefaultAsync();

        if (previous != null)
        {
            (current.OrderIndex, previous.OrderIndex) = (previous.OrderIndex, current.OrderIndex);
            await _db.SaveChangesAsync();
        }
    }

    public async Task MoveSignatureDownAsync(int id)
    {
        var current = await _db.Signatures.FindAsync(id);
        if (current == null) return;

        var next = await _db.Signatures
            .Where(s => s.OrderIndex > current.OrderIndex)
            .OrderBy(s => s.OrderIndex)
            .FirstOrDefaultAsync();

        if (next != null)
        {
            (current.OrderIndex, next.OrderIndex) = (next.OrderIndex, current.OrderIndex);
            await _db.SaveChangesAsync();
        }
    }

    // ---- Settings (key-value store for letterhead etc.) ----

    public async Task<string?> GetSettingAsync(string key)
    {
        var setting = await _db.Settings.FindAsync(key);
        return setting?.Value;
    }

    public async Task SetSettingAsync(string key, string value)
    {
        var existing = await _db.Settings.FindAsync(key);
        if (existing != null)
        {
            existing.Value = value;
        }
        else
        {
            _db.Settings.Add(new Setting { Key = key, Value = value });
        }
        await _db.SaveChangesAsync();
    }
}
