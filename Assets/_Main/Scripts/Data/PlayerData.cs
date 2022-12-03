using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;

public class PlayerData
{
    private Dictionary<string, string> getData;

    private Dictionary<string, object> putData;

    private List<PlayerBalance> getBalances;

    public PlayerData()
    {
        putData = new Dictionary<string, object>();
    }

    public async Task<PlayerData> Get()
    {
        getData = await CloudSaveService.Instance.Data.LoadAllAsync();

        getBalances = (await EconomyService.Instance.PlayerBalances.GetBalancesAsync()).Balances;

        return this;
    }

    public async Task Put()
    {
        await CloudSaveService.Instance.Data.ForceSaveAsync(putData);
    }

    public bool IsInitialized
    {
        get => getData.TryGetValue("isInitialized", out string strValue)
            ? bool.TryParse(strValue, out bool value) ? value : false
            : false;
    }

    public int Level
    {
        get => getData.TryGetValue("level", out string strValue)
            ? int.TryParse(strValue, out int value) ? value : 0
            : 0;
    }

    public int Exp
    {
        get => getData.TryGetValue("exp", out string strValue)
            ? int.TryParse(strValue, out int value) ? value : 0
            : 0;
    }

    public string SelectedShipID
    {
        get => getData.TryGetValue("selectedShipId", out string strValue)
            ? strValue
            : "";
    }

    public long Coins
    {
        get => getBalances.FirstOrDefault(i => i.CurrencyId == "COINS")?.Balance ?? 0;
    }

    public long Gems
    {
        get => getBalances.FirstOrDefault(i => i.CurrencyId == "GEMS")?.Balance ?? 0;
    }

    public PlayerData SetInitialized(bool value)
    {
        putData["isInitialized"] = value;

        return this;
    }

    public PlayerData SetLevel(int value)
    {
        putData["level"] = value;

        return this;
    }

    public PlayerData SetExp(int value)
    {
        putData["exp"] = value;

        return this;
    }

    public PlayerData SetSelectedShipID(string value)
    {
        putData["selectedShipId"] = value;

        return this;
    }

    public async Task<PlayerData> AddCoins(int value)
    {
        var result = await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("COINS", value);

        var targetIndex = getBalances.FindIndex(i => i.CurrencyId == "COINS");

        getBalances[targetIndex] = result;

        return this;
    }

    public async Task<PlayerData> AddGems(int value)
    {
        var result = await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync("GEMS", value);

        var targetIndex = getBalances.FindIndex(i => i.CurrencyId == "GEMS");

        getBalances[targetIndex] = result;

        return this;
    }


}
