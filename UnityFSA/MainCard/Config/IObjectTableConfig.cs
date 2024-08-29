using System.Collections;
using System.Collections.Generic;

namespace XMeta.Foundation.Config
{
    
    public interface IObjectTableConfig
    {
        public interface IItem
        {
            int Id { get; }
        }
        
        public IList GetItems();
        
    }
    

    
}