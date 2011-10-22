using System.Collections.Generic;

namespace LightDataAccess
{
	public class CmdParams : Dictionary<string, object>
	{
        public CmdParams()
        {
        }

        public CmdParams(Dictionary<string, object> init): base(init)
        {
        }
	}
}
