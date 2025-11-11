using Newtonsoft.Json;

public class PlayerData
{
    public string _name;
    public int indexAsset;
    public PlayerData()
    {

    }
    public PlayerData(string name, int index)
    {
        this._name = name;
        this.indexAsset = index;
    }
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this);
    }
}
