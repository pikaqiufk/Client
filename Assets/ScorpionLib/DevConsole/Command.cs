using UnityEngine;
using System.Collections;

namespace DevConsole{
	[System.Serializable]
	public class Command {
		//=======================
		//DELEGATE TYPES
		//=======================
		public delegate void NoArgs();
		public delegate void OneStringArg(string args);
		enum DelegateTypes{NoArgs, OneStringArg};
		
		//=======================
		//PROPERTIES
		//=======================
		public string CommandName{
			get; set;
		}
		DelegateTypes DelegateType{
			get; set;
		}
		string HelpText{
			get; set;
		}
		NoArgs NoArgsDelegate{
			get; set;
		}
		OneStringArg OneStringArgDelegate{
			get; set;
		}
		NoArgs HelpFunction{
			get; set;
		}
		
		//==============================
		//CONSTRUCTORS
		//==============================
		public Command(string commandName,NoArgs function){
			CommandName = commandName;
			DelegateType = DelegateTypes.NoArgs;
			NoArgsDelegate = function;
		}
		public Command(string commandName,NoArgs function, string helpText){
			CommandName = commandName;
			DelegateType = DelegateTypes.NoArgs;
			NoArgsDelegate = function;
			HelpText =helpText;
		}
		public Command(string commandName,NoArgs function, NoArgs helpFunction){
			CommandName = commandName;
			DelegateType = DelegateTypes.NoArgs;
			NoArgsDelegate = function;
			HelpFunction = helpFunction;
		}


		public Command (string commandName,OneStringArg function){
			CommandName = commandName;
			DelegateType = DelegateTypes.OneStringArg;
			OneStringArgDelegate = function;
		}
		public Command (string commandName,OneStringArg function, string helpText){
			CommandName = commandName;
			DelegateType = DelegateTypes.OneStringArg;
			OneStringArgDelegate = function;
			HelpText = helpText;
		}
		public Command (string commandName,OneStringArg function, NoArgs helpFunction){
			CommandName = commandName;
			DelegateType = DelegateTypes.OneStringArg;
			OneStringArgDelegate = function;
			HelpFunction = helpFunction;
		}
		//=======================
		//EXECUTE
		//=======================
		internal void Execute(){
			NoArgsDelegate();
		}
		internal void Execute(string arg){
			if (DelegateType == DelegateTypes.NoArgs)
				NoArgsDelegate();
			else
				OneStringArgDelegate(arg);
		}
		internal void ShowHelp(){
			if (HelpFunction != null)
				HelpFunction();
			else
				Console.LogInfo("Command Info: " + (HelpText == null?"There's no help for this command":HelpText));
		}
	}
}