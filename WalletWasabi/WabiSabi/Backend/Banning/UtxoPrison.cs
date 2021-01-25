using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WalletWasabi.Logging;

namespace WalletWasabi.WabiSabi.Backend.Banning
{
	/// <summary>
	/// Malicious UTXOs are sent here.
	/// </summary>
	public class UtxoPrison
	{
		private UtxoPrison(string filePath, Dictionary<OutPoint, Inmate> inmates)
		{
			FilePath = filePath;
			Inmates = inmates;
		}

		private Dictionary<OutPoint, Inmate> Inmates { get; }

		private string FilePath { get; }

		public static UtxoPrison FromFileOrEmpty(string filePath)
		{
			var inmates = new Dictionary<OutPoint, Inmate>();
			if (File.Exists(filePath))
			{
				try
				{
					foreach (var inmate in File.ReadAllLines(filePath).Select(Inmate.FromString))
					{
						inmates.Add(inmate.Utxo, inmate);
					}
				}
				catch (Exception ex)
				{
					Logger.LogError(ex);
					Logger.LogWarning($"Deleting {filePath}");
					File.Delete(filePath);
				}
			}

			return new UtxoPrison(filePath, inmates);
		}

		public async Task ToFileAsync()
		{
			IoHelpers.EnsureContainingDirectoryExists(FilePath);
			await File.WriteAllLinesAsync(FilePath, Inmates.Select(x => x.ToString())).ConfigureAwait(false);
		}

		public int CountInmates(Punishment? punishment = null)
		{
			if (punishment is null)
			{
				return Inmates.Count;
			}
			else
			{
				return Inmates.Where(x => x.Value.Punishment == punishment).Count();
			}
		}
	}
}
