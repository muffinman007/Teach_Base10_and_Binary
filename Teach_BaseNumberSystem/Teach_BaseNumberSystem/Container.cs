// Author: Minh Pham
// Date: 02/15/2014 
//
// this class holds the base and it's power hence the place value of a given number and the user numerical input.
// this also contains coordinates that will help the PlaceHolder class draw This itself.
// the virutal dimension of This is 100px width and 300 px height:
//         
//   how This will look like (not drawn to scale):
//
//			-----------
//			|   10^3  |    <-- the base with it's power to consitute the place value
//			|		  |
//			|  ------ |
//			|  |    | |    <--- where user will input a digit between 0 - 9
//			|  |  9 | |
//			|  ------ |
//			|   1000  |
//			|   x  9  |	   <--- the calcuation 
//			|   ----  |
//			|   9000  |       
//			|		  |
//			-----------
//
// the base with its power will be center align within the 100px width with the y coordinate of the where the text will be drawn to be 15px
// User input box is 80x60 with the top left corner coordinate of (10, 70)
// the text for the calcuation will start rendering at coordinate of (6, 160), the x coordinate will need to be adjusted for text length so 
// the rendering of the calucation will be pleasing to the eye.

// this class will automatically recalculate itself whenever it gets moved around to different location, that is crated by the PlaceHolder class

using System;
using System.Collections.Generic;

using pwr = Teach_BaseNumberSystem.ControlVariables.Pwr;

namespace Teach_BaseNumberSystem{
	class Container{

		#region Fields

		string calculationString = string.Empty;

		// coordinate where base string is to be render (relative)
		public readonly int baseYcoord = 5;

		// coordinate where input box is to be render (relative)
		public readonly int inputXCoord = 10;
		public readonly int inputYCoord = 70;

		// coordinate where calcuation string is to be render (relative)
		public readonly int calYCoord = 132;

		#endregion


		#region Constructure
		// paramter power will specified which base and its power will be drawn along with the calcuation of the user inputed value
		public Container(pwr power){
			UserInput = null;
			Update(power);
		}
		#endregion


		#region Properties
		// PlaceHolder class will check property to make sure the container information matches the plave value location it is occupying
		public pwr PowerAndPlaceValue{ get; private set; }

		// text to be render to screen
		public string BaseString{ get; private set; }

		public string CalculationString{ get{ return calculationString; } }

		public byte? UserInput{ get; private set; }
		#endregion


		#region Methods
		// if PlaceHolder finds that This class infomation doesn't reflect the place value location it is occupying, Placeholder
		// will call Update to get This class to update it's information. 
		public void Update(pwr newPower){
			PowerAndPlaceValue = newPower;

			BaseString = ControlVariables.stringDict[newPower];

			createCalcuString();
		}

		
		public void InputValue(byte? value){
			UserInput = value;

			createCalcuString();
		}
		#endregion


		#region Helper methods
		void createCalcuString(){
			switch(PowerAndPlaceValue){	
				case pwr.ZeroPwr:
					if( UserInput.HasValue ){
						calculationString = "      1\nx    " + UserInput.Value.ToString() + "\n      " + UserInput.Value.ToString();
					}
					break;

				case pwr.FirstPwr:
					if( UserInput.HasValue ){
						calculationString = "    10\nx    " + UserInput.Value.ToString() + "\n    " + UserInput.Value.ToString() + "0";
					}
					break;
					
				case pwr.SecondPwr:
					if( UserInput.HasValue ){
						calculationString = "  100\nx    " + UserInput.Value.ToString() + "\n  " + UserInput.Value.ToString() + "00";
					}
					break;
					
				case pwr.ThirdPwr:
					if( UserInput.HasValue ){
						calculationString = "1000\nx    " + UserInput.Value.ToString() + "\n" + UserInput.Value.ToString() + "000";
					}
					break;

				case pwr.FourthPwr:
					if( UserInput.HasValue ){
						calculationString = "  10t\nx    " + UserInput.Value.ToString() + "\n  " + UserInput.Value.ToString() + "0t";
					}
					break;
					
				case pwr.FifthPwr:
					if( UserInput.HasValue ){
						calculationString = "100t\nx    " + UserInput.Value.ToString() + "\n" + UserInput.Value.ToString() + "00t";
					}
					break;

				case pwr.SixthPwr:
					if( UserInput.HasValue ){
						calculationString = "   1m\nx    " + UserInput.Value.ToString() + "\n   " + UserInput.Value.ToString() + "m";
					}
					break;
					
				case pwr.SeventhPwr:
					if( UserInput.HasValue ){
						calculationString = " 10m\nx    " + UserInput.Value.ToString() + "\n " + UserInput.Value.ToString() + "0m";
					}
					break;

				case pwr.NULL:
					calculationString = string.Empty;
					break;
			}

		}


		public void Reset(){
			UserInput = null;
			this.Update(pwr.NULL);
		}
		#endregion

	}// end class Container
}
