
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExStorSys;
using static ExStorSys.PropertyId;
using static ExStorSys.ExSysStatus;
using static ExStorSys.ValidateSchema;
using static ExStorSys.ValidateDataStorage;



// user name: jeffs
// created:   11/18/2025 11:58:19 PM

namespace ExStoreTest2026.ExStorSys
{
	public abstract class APropChgdEvt
	{
		public delegate void PropChgdEventHandler(object sender, PropChgEvtArgs e);

		public event ExStorMgr.PropChgdEventHandler PropChgd;


		protected virtual void OnPropChgd(PropChgEvtArgs e)
		{
			PropChgd?.Invoke(this, e);
		}

		protected void OnPropChgd(PropertyId pid, DynaValue value)
		{
			OnPropChgd(new PropChgEvtArgs( pid, value));
		}

		protected void OnPropChgd(PropertyId pid, bool value)
		{
			OnPropChgd(new PropChgEvtArgs( pid, value));
		}

		protected void OnPropChgdRn(RunningStatus rnStat)
		{
			OnPropChgd(new PropChgEvtArgs(PI_GEN_RUNNING_STAT, rnStat));
		}

		protected void OnPropChgdExs(ExSysStatus rnStat)
		{
			OnPropChgd(new PropChgEvtArgs(PI_XSYS_STATUS, rnStat));
		}

		protected void OnPropChgdWsc(ValidateSchema wSc)
		{
			OnPropChgd(new PropChgEvtArgs(PI_VFY_WBK_SC, wSc));
		}

		protected void OnPropChgdWds(ValidateDataStorage wDs)
		{
			OnPropChgd(new PropChgEvtArgs(PI_VFY_WBK_DS, wDs));
		}

		protected void OnPropChgdSsc(ValidateSchema sSc)
		{
			OnPropChgd(new PropChgEvtArgs(PI_VFY_SHT_SC, sSc));
		}

		protected void OnPropChgdSds(ValidateDataStorage sDs)
		{
			OnPropChgd(new PropChgEvtArgs(PI_VFY_SHT_DS, sDs));
		}

	}
}
