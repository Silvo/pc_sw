using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pc_sw.Model
{
    public interface IDataStorage
    {
        event EventHandler SourceAdded;

        List<DataSource> GetDataSources();
    }
}
