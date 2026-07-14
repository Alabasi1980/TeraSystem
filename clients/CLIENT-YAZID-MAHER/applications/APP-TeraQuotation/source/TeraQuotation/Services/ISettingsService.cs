using TeraQuotation.Models;

namespace TeraQuotation.Services;

public interface ISettingsService
{
    // Suppliers
    Task<List<Supplier>> GetAllSuppliersAsync();
    Task<Supplier> AddSupplierAsync(Supplier supplier);
    Task UpdateSupplierAsync(Supplier supplier);
    Task DeleteSupplierAsync(int id);
    // Items
    Task<List<Item>> GetAllItemsAsync();
    Task<List<Item>> SearchItemsAsync(string search);
    Task<Item> AddItemAsync(Item item);
    Task UpdateItemAsync(Item item);
    Task DeleteItemAsync(int id);
    // Signatures
    Task<List<Signature>> GetAllSignaturesAsync();
    Task<Signature> AddSignatureAsync(Signature signature);
    Task DeleteSignatureAsync(int id);
    Task MoveSignatureUpAsync(int id);
    Task MoveSignatureDownAsync(int id);
    // Settings
    Task<string?> GetSettingAsync(string key);
    Task SetSettingAsync(string key, string value);
}
