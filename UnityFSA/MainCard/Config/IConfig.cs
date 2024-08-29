namespace XMeta.Foundation.Config
{
    public interface IConfig
    {
        IObjectTableConfig.IItem Config { get; }
    }


    public interface IConfigAsset
    {
        public string AssetPath { get; }
    }



    //TODO:不要这个
    public interface ILived
    {
        public double MaxHitPoint { get; }
    } 
}