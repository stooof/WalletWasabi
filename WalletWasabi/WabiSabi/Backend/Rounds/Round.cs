using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletWasabi.WabiSabi.Backend.Rounds
{
	public class Round
	{
		public Guid Id { get; } = Guid.NewGuid();
		public Phase Phase { get; set; } = Phase.InputRegistration;
	}
}
