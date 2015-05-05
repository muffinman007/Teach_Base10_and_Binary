/** 
  *  Author: Minh Pham
  *  Date: 02/18/2014
  *
  * This class control the rendering of the binary information and calcuation
  * There's a 25px margin from the top and the bottom of the game window with a 50px margin in the middle to separate the area that will show base 10 information from the binary area
  * 
  * 
  */



using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using pwr = Teach_BaseNumberSystem.ControlVariables.Pwr;

namespace Teach_BaseNumberSystem{
	class BinaryControl{

		#region Fields
		Texture2D switchTexture;
		Texture2D lineTexture;

		class binaryEntity{
			public pwr power;							// the binary power
			public Rectangle destinationRect;			// where to draw the switch
			public Rectangle sourceRect;				// control which switch image will be render
			public float basePowerXCoord;				// coordinate to where to draw the base and it's power
			public float basePowerYCoord;				// coordinate to where to draw the base and it's power     
			public float calculationXCoord;				// coordinate to where to draw the binary calcuation
			public float calculationYCoord;				// coordinate to where to draw the binary calcuation
			public bool on;								// is the binary in an on state or an off state
		}

		Dictionary<int, binaryEntity> twos;				// easy accessing of each bit position information

		List<string> bitIsOn;
		List<string> bitIsOff;
		List<string> basePower;

		// coordinate to help with rendering
		int x = 320;
        int y = 348;

		#endregion


		#region Properties
		public bool DrawSwitch{ get; set; }
		public bool DrawPower{ get; set; }
		public bool DrawCalculation{ get; set; }
		public bool DrawTotalValue{ get; set; }
		#endregion


		#region Constructor
		public BinaryControl(Texture2D switchTexture, Texture2D lineTexture){
			this.switchTexture = switchTexture;
			this.lineTexture = lineTexture;

			DrawSwitch = false;
			DrawPower = false;
			DrawCalculation = false;
			DrawTotalValue = false;

			twos = new Dictionary<int,binaryEntity>(8);
			Initialize();

			InitalizeString();
		}
		#endregion


		#region Methods
		void Initialize(){

			for( int i = 0; i <= 7; ++i ) {

				twos.Add(i, new binaryEntity());

				#region switch to assign the power
				switch(i){
					case 7:
						twos[i].power = pwr.ZeroPwr;
						break;

					case 6:
						twos[i].power = pwr.FirstPwr;
						break;

					case 5:
						twos[i].power = pwr.SecondPwr;
						break;

					case 4:
						twos[i].power = pwr.ThirdPwr;
						break;

					case 3:
						twos[i].power = pwr.FourthPwr;
						break;

					case 2:
						twos[i].power = pwr.FifthPwr;
						break;

					case 1:
						twos[i].power = pwr.SixthPwr;
						break;

					case 0:
						twos[i].power = pwr.SeventhPwr;
						break;
				}
				#endregion


				twos[i].destinationRect = new Rectangle( x + (i * 110), y + 65, 100, 100 );
				twos[i].sourceRect = new Rectangle(0, 0, 100, 100);
				twos[i].basePowerXCoord = (x + (i * 110)) + 50;
				twos[i].basePowerYCoord = y + 15;
				twos[i].calculationXCoord = (x + (i * 110)) + 5;
				twos[i].calculationYCoord = y + 160;
				twos[i].on = false;
			}//end for
		}//end Initialize


		void InitalizeString(){

			// text to render when bit is on the ON position
			bitIsOn = new List<string>(8){ {" 128\nx   1\n 128"},
										   {"   64\nx   1\n   64"},
										   {"   32\nx   1\n   32"},
										   {"   16\nx   1\n   16"},
										   {"     8\nx   1\n     8"},
										   {"     4\nx   1\n     4"},
										   {"     2\nx   1\n     2"},
										   {"     1\nx   1\n     1"} };

			// text to render when bit is on the OFF position
			bitIsOff = new List<string>(8){ {" 128\nx   0\n     0"},
										    {"   64\nx   0\n     0"},
										    {"   32\nx   0\n     0"},
										    {"   16\nx   0\n     0"},
										    {"     8\nx   0\n     0"},
										    {"     4\nx   0\n     0"},
										    {"     2\nx   0\n     0"},
										    {"     1\nx   0\n     0"} };

			// the powers of two 
			basePower = new List<string>(8){ {"2⁷"},
											 {"2⁶"},
											 {"2⁵"},
											 {"2⁴"},
											 {"2³"},
											 {"2²"},
											 {"2¹"},
											 {"2⁰"} };
		}// end InitalizeString


		public void SwitchManagement(Point mousePos){
			if(DrawSwitch){
				foreach(KeyValuePair<int, binaryEntity> bit in twos){
					if( bit.Value.destinationRect.Contains(mousePos) ){
						bit.Value.on = !bit.Value.on;

						if( bit.Value.on ){
							bit.Value.sourceRect.X = 100;
						}
						else{
							bit.Value.sourceRect.X = 0;
						}

						break;
					}
				}//end foreach
			}// end if
		}//end SwitchManagement

		// allow user to turn switch on/off with the mouse scroll
		public void SwitchManagement(Point mousePos, float wheelValue, float wheelLastValue){
			if(  (wheelValue != wheelLastValue) && (wheelValue % ControlVariables.mouseWheelNotch == 0) ){
				SwitchManagement(mousePos);
			}
		}

		// allow key combo to turn swith on or off all at the same time
		public void SwitchManagement(bool turnAllSwitchOn){
			for( int i = 0; i <= 7 ; ++i ){
				if(turnAllSwitchOn){
					twos[i].on = true;
					twos[i].sourceRect.X = 100;
				}
				else{ // turn all off
					twos[i].on = false;
					twos[i].sourceRect.X = 0;
				}
			}
		}


		// allow to turn switch on/off with the number key
		public void SwitchManagement(KeyboardState currentKb, KeyboardState previousKb){
			if( (currentKb.IsKeyDown(Keys.D0) && previousKb.IsKeyUp(Keys.D0)) || (currentKb.IsKeyDown(Keys.NumPad0) && previousKb.IsKeyUp(Keys.NumPad0)) ){
				twos[7].on = !twos[7].on;
				if(twos[7].on){
					twos[7].sourceRect.X = 100;
				}
				else{
					twos[7].sourceRect.X = 0;
				}
			}// 0 

			if( (currentKb.IsKeyDown(Keys.D1) && previousKb.IsKeyUp(Keys.D1)) || (currentKb.IsKeyDown(Keys.NumPad1) && previousKb.IsKeyUp(Keys.NumPad1)) ){
				twos[6].on = !twos[6].on;
				if(twos[6].on){
					twos[6].sourceRect.X = 100;
				}
				else{
					twos[6].sourceRect.X = 0;
				}
			}// 1 

			if( (currentKb.IsKeyDown(Keys.D2) && previousKb.IsKeyUp(Keys.D2)) || (currentKb.IsKeyDown(Keys.NumPad2) && previousKb.IsKeyUp(Keys.NumPad2)) ){
				twos[5].on = !twos[5].on;
				if(twos[5].on){
					twos[5].sourceRect.X = 100;
				}
				else{
					twos[5].sourceRect.X = 0;
				}
			}// 2 

			if( (currentKb.IsKeyDown(Keys.D3) && previousKb.IsKeyUp(Keys.D3)) || (currentKb.IsKeyDown(Keys.NumPad3) && previousKb.IsKeyUp(Keys.NumPad3)) ){
				twos[4].on = !twos[4].on;
				if(twos[4].on){
					twos[4].sourceRect.X = 100;
				}
				else{
					twos[4].sourceRect.X = 0;
				}
			}//  3

			if( (currentKb.IsKeyDown(Keys.D4) && previousKb.IsKeyUp(Keys.D4)) || (currentKb.IsKeyDown(Keys.NumPad4) && previousKb.IsKeyUp(Keys.NumPad4)) ){
				twos[3].on = !twos[3].on;
				if(twos[3].on){
					twos[3].sourceRect.X = 100;
				}
				else{
					twos[3].sourceRect.X = 0;
				}
			}//  4

			if( (currentKb.IsKeyDown(Keys.D5) && previousKb.IsKeyUp(Keys.D5)) || (currentKb.IsKeyDown(Keys.NumPad5) && previousKb.IsKeyUp(Keys.NumPad5)) ){
				twos[2].on = !twos[2].on;
				if(twos[2].on){
					twos[2].sourceRect.X = 100;
				}
				else{
					twos[2].sourceRect.X = 0;
				}
			}//  5

			if( (currentKb.IsKeyDown(Keys.D6) && previousKb.IsKeyUp(Keys.D6)) || (currentKb.IsKeyDown(Keys.NumPad6) && previousKb.IsKeyUp(Keys.NumPad6)) ){
				twos[1].on = !twos[1].on;
				if(twos[1].on){
					twos[1].sourceRect.X = 100;
				}
				else{
					twos[1].sourceRect.X = 0;
				}
			}// 6

			if( (currentKb.IsKeyDown(Keys.D7) && previousKb.IsKeyUp(Keys.D7)) || (currentKb.IsKeyDown(Keys.NumPad7) && previousKb.IsKeyUp(Keys.NumPad7)) ){
				twos[0].on = !twos[0].on;
				if(twos[0].on){
					twos[0].sourceRect.X = 100;
				}
				else{
					twos[0].sourceRect.X = 0;
				}
			}//  7
		}
		
		#endregion


		#region Draw
		public void Draw(ref SpriteBatch spriteBatch, ref SpriteFont font){

			if(DrawSwitch){
				foreach(KeyValuePair<int, binaryEntity> bit in twos){
					spriteBatch.Draw(switchTexture, bit.Value.destinationRect, bit.Value.sourceRect, Color.White);
				}
			}

			if(DrawPower){
				foreach(KeyValuePair<int, binaryEntity> bit in twos){
					float xOffset = font.MeasureString(basePower[bit.Key]).X / 2;					// offset to middle align the base relatively
					spriteBatch.DrawString(font, basePower[bit.Key], new Vector2(bit.Value.basePowerXCoord - xOffset, bit.Value.basePowerYCoord), Color.Black);
				}
			}

			if(DrawCalculation && DrawSwitch){
				foreach(KeyValuePair<int, binaryEntity> bit in twos){

					// draw the equal line
					spriteBatch.Draw(lineTexture, new Rectangle( (x + (bit.Key * 110)), y + 270, 90, 1), Color.Black);

					if(bit.Value.on){
						spriteBatch.DrawString(font, bitIsOn[bit.Key], new Vector2(bit.Value.calculationXCoord, bit.Value.calculationYCoord), Color.Black);
					}
					else{
						spriteBatch.DrawString(font, bitIsOff[bit.Key], new Vector2(bit.Value.calculationXCoord, bit.Value.calculationYCoord), Color.Black);
					}
				}
			}

			if( DrawTotalValue && (DrawSwitch || (DrawCalculation && DrawSwitch)) ){
				byte total = 0;

				for( int i = 0; i <= 7; ++i ){
					if(twos[i].on) {

						#region switch to calcuation total
						switch(i){
							case 7:
								total += 1;
								break;

							case 6:
								total += 2;
								break;

							case 5:
								total += 4;
								break;

							case 4:
								total += 8;
								break;

							case 3:
								total += 16;
								break;

							case 2:
								total += 32;
								break;

							case 1:
								total += 64;
								break;

							case 0:
								total += 128;
								break;
						}
						#endregion
					}
				}

				Vector2 strMeasure = font.MeasureString(total.ToString());

				spriteBatch.DrawString(font, total.ToString(), new Vector2( 280 - strMeasure.X, 675 - strMeasure.Y), Color.Black);
			}

		}//end Draw
		#endregion

	}// end BinaryControl
}
