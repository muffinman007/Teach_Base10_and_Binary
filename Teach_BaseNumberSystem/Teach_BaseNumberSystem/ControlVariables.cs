/*
 * Author: Minh Pham
 * Date: 02/16/2014
 * 
 * it's better to have those variables listed below to be in a separate class then it have multiple objects containing the same information within each objects
 */

using System;
using System.Collections.Generic;


namespace Teach_BaseNumberSystem{
	static class ControlVariables{
		#region Fields

		public static readonly int mouseWheelNotch = 120;
		
		// selection is passed along with the mouse pointer location to PlaceHolder.Update(). 
		// the selection help PlaceHolder decided what action to execuate within the Update method.
		public enum Manipulate{ CreateEntity, UpdateEntity, DeleteEntity }


		// Controls where the container will be held at and help with auto calcuation within the container class
		// used by Container and PlaceHolder class
		public enum Pwr{ ZeroPwr, FirstPwr, SecondPwr, ThirdPwr, FourthPwr, FifthPwr, SixthPwr, SeventhPwr, NULL }


		// holds strings value for easy retrival 
		// usbed by the Container class
		public static readonly Dictionary<Pwr, string> stringDict = new Dictionary<Pwr,string>(9){	{Pwr.ZeroPwr,    "10⁰"},
																									{Pwr.FirstPwr,   "10¹"},
																									{Pwr.SecondPwr,  "10²"},
																									{Pwr.ThirdPwr,   "10³"},
																									{Pwr.FourthPwr,  "10⁴"},
																									{Pwr.FifthPwr,   "10⁵"},
																									{Pwr.SixthPwr,   "10⁶"},
																									{Pwr.SeventhPwr, "10⁷"},
																									{Pwr.NULL,	    "NULL"} };
		#endregion

	}
}
