﻿using AvalonStudio.Commands;
using AvalonStudio.Extensibility;
using AvalonStudio.Shell;
using ReactiveUI;
using System;
using System.Composition;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using WalletWasabi.Gui.Tabs;
using WalletWasabi.Gui.Tabs.WalletManager;

namespace WalletWasabi.Gui.Shell.Commands
{
	internal class ToolCommands
	{
		[ImportingConstructor]
		public ToolCommands(CommandIconService commandIconService)
		{
			var walletManagerCommand = ReactiveCommand.Create(OnWalletManager);

			var transactionBroadcasterCommand = ReactiveCommand.Create(() =>
			{
				IoC.Get<IShell>().AddOrSelectDocument(() => new TransactionBroadcasterViewModel());
			});

			var settingsCommand = ReactiveCommand.Create(() =>
			{
				IoC.Get<IShell>().AddOrSelectDocument(() => new SettingsViewModel());
			});

			Observable
				.Merge(walletManagerCommand.ThrownExceptions)
				.Merge(transactionBroadcasterCommand.ThrownExceptions)
				.Merge(settingsCommand.ThrownExceptions)
				.Subscribe(OnFileOpenException);

			WalletManagerCommand = new CommandDefinition(
				"Wallet Manager",
				commandIconService.GetCompletionKindImage("WalletManager"),
				walletManagerCommand);

			TransactionBroadcasterCommand = new CommandDefinition(
				"Transaction Broadcaster",
				commandIconService.GetCompletionKindImage("TransactionBroadcaster"),
				transactionBroadcasterCommand);

			SettingsCommand = new CommandDefinition(
				"Settings",
				commandIconService.GetCompletionKindImage("Settings"),
				settingsCommand);
		}

		private void OnWalletManager()
		{
			var isAnyWalletAvailable = Directory.Exists(Global.WalletsDir) && Directory.EnumerateFiles(Global.WalletsDir).Any();

			var walletManagerViewModel = IoC.Get<IShell>().GetOrCreate<WalletManagerViewModel>();
			if (Directory.Exists(Global.WalletsDir) && Directory.EnumerateFiles(Global.WalletsDir).Any())
			{
				walletManagerViewModel.SelectLoadWallet();
			}
			else
			{
				walletManagerViewModel.SelectGenerateWallet();
			}
		}

		private void OnFileOpenException(Exception ex)
		{
			Logging.Logger.LogError<ToolCommands>(ex);
		}

		[ExportCommandDefinition("Tools.WalletManager")]
		public CommandDefinition WalletManagerCommand { get; }

		[ExportCommandDefinition("Tools.TransactionBroadcaster")]
		public CommandDefinition TransactionBroadcasterCommand { get; }

		[ExportCommandDefinition("Tools.Settings")]
		public CommandDefinition SettingsCommand { get; }
	}
}
