using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightDataAccess
{
    public class FilterConstraints
    {
        List<string> _Constraints { get; set; }
        public CmdParams Params { get; set; }

        public FilterConstraints()
        {
            _Constraints = new List<string>();
            Params = new CmdParams();
        }

        public string BuildWhere()
        {
            return (_Constraints.Count > 0 ? "WHERE " : "") + _Constraints.Select( c => "(" + c + ")").ToCSV(" AND ");
        }

        public void Add(string constraint)
        {
            Add(constraint, null);
        }

        public void Add(string constraint, CmdParams Params)
        {
            _Constraints.Add(constraint);
            if(Params != null)
            {
                foreach( var p in Params )
                {
                    this.Params.Add(p.Key, p.Value);
                }
            }
        }
    }
}
