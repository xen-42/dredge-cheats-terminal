using BepInEx;
using CommandTerminal;
using HarmonyLib;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DredgeCheatsTerminal
{
	[BepInPlugin("com.xen-42.dredge.cheatsterminal", "Cheats Terminal", "0.0.1")]
	[BepInProcess("DREDGE.exe")]
	[HarmonyPatch]
	public class Plugin : BaseUnityPlugin
	{
		public static Plugin Instance { get; private set; }

		public static GameObject terminal;

		private string[] _forbiddenCommands = new string[]
		{
			"func.map",
			"func.photo",
			"func.custom",
			"dlc.own"
		};

		private void Awake()
		{
			Instance = this;

			// Plugin startup logic
			Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
			Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
		}

		private void MakeTerminal()
		{
			try
			{
				terminal = new GameObject("Terminal");
				terminal.AddComponent<Terminal>();
			}
			catch (Exception e)
			{
				Instance.Logger.LogError(e);
			}
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(CommandShell), nameof(CommandShell.AddCommand), new Type[] { typeof(string), typeof(CommandInfo) })]
		public static bool CommandShell_AddCommand(CommandShell __instance, string name)
		{
			if (terminal == null)
			{
				Instance.MakeTerminal();
			}

			if (Instance._forbiddenCommands.Contains(name))
			{
				return false;
			}
			else
			{
				Instance.Logger.LogInfo($"Command added: {name}");
				return true;
			}
		}
	}
}
