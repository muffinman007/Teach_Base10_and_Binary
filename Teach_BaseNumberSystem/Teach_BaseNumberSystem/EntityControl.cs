/*
 * Author: Minh Pham
 * Date: 02/16/2014
 * 
 * This class controls the rendering of the base 10 
 * User is allow to drag the "input box" icon into a specific entity place value area. 
 * There's a 25px margin from the top and the bottom of the game window with a 50px margin in the middle to separate the area that will show base 10 information from the binary area
 */


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using pwr = Teach_BaseNumberSystem.ControlVariables.Pwr;
using manipulate = Teach_BaseNumberSystem.ControlVariables.Manipulate;

namespace Teach_BaseNumberSystem{
	class EntityControl{

		#region Fields
		// a = area where the container entity will occupy and render
		// p = place value, the place value of that specific location
		// c = container, the entity itself
		struct acp{
			public bool renderInputBox;
			public bool hasEntity;
			public Rectangle area;
			public pwr power;
			public Container mass;
		}
		acp[] theEntities = new acp[8];

		Texture2D inputBoxTexture;
		Texture2D blackLineTexture;
 
		// cursor animation
		Texture2D cursorTexture;
		float timer = 0.0f;
		float timeInterval = 60.0f;					// length of time each image is display
		int currentFrame = 0;						// the image/frame number currently on
		int cursorWdith = 80;
		int cursorHeight = 60;
		Rectangle cursorRect;

		int? entityWaitingForInput = null;

		#endregion


		#region Properties
		public bool DrawCursor{ get; set; }
		public bool DrawBase{ get; set; }
		public bool DrawCalculation{ get; set; }
		public bool DrawTotalValue{ get; set; }
		#endregion



		#region Constructure
		public EntityControl(Texture2D inputBox, Texture2D cursor, Texture2D line){
			inputBoxTexture = inputBox;
			cursorTexture = cursor;
			blackLineTexture = line;

			DrawCursor = false;
			DrawBase = false;
			DrawCalculation = false;
			DrawTotalValue = false;

			int x = 320;
			for( int i = 0; i <= (theEntities.Length - 1) ; ++i ){
				theEntities[i].renderInputBox = false;
				theEntities[i].hasEntity = false;

				// area
				theEntities[i].area = new Rectangle(x, 25, 100, 300);

				#region power
				switch(i){
					case 0:
						theEntities[i].power = pwr.SeventhPwr;
						break;
					
					case 1:
						theEntities[i].power = pwr.SixthPwr;
						break;
					
					case 2:
						theEntities[i].power = pwr.FifthPwr;
						break;
					
					case 3:
						theEntities[i].power = pwr.FourthPwr;
						break;
					
					case 4:
						theEntities[i].power = pwr.ThirdPwr;
						break;
					
					case 5:
						theEntities[i].power = pwr.SecondPwr;
						break;
					
					case 6:
						theEntities[i].power = pwr.FirstPwr;
						break;
					
					case 7:
						theEntities[i].power = pwr.ZeroPwr;
						break;
				}
				#endregion

				// container
				theEntities[i].mass = new Container(pwr.NULL);

				x += 110;
			}

			// cursor animation 
			cursorRect = new Rectangle(currentFrame * cursorWdith, 0, cursorWdith, cursorHeight);
		}
		#endregion



		#region Update, timer for cursor animation
		public void Update(GameTime gameTime){
			if(DrawCursor){

				// cursor animation
				timer += (float)gameTime.ElapsedGameTime.Milliseconds;
				if( timer >= timeInterval ){
					timer = 0.0f;
					if( (++currentFrame) > 5 )
						currentFrame = 0;
				}

				cursorRect.X = currentFrame * cursorWdith;
			}
		}
		#endregion



		#region Methods
		// enum manipulate affacts what this class does in it's update section, it can either create a new entity, update an entity info,
		// or delete an entity, this update is called when the user clicked within the Game1.hotZone 
		public void EntityManagement(Point mousePos, manipulate action){

			if( action == manipulate.CreateEntity ){
				// check if user had drag the inputBox icon within a specific area that a place value occupy, by drag meaning that the mouse pointer  
				// has to be inside the area.
				for( int i = 0; i <= (theEntities.Length - 1); ++i ){
					if( (theEntities[i].area.Contains(mousePos)) && (theEntities[i].hasEntity == false) ){
						theEntities[i].renderInputBox = true;							// draw the inputBox

						theEntities[i].mass = new Container(theEntities[i].power);		// the entitiy is now ALIVE!!! WHOAHAHAHAHA

						theEntities[i].hasEntity = true;								// Spread the news it is ALIVE!!!	
						
						// once an entity had been created there is not need to continue on with the for loop
						// break also prevent the creation of an entity at two different place value at the same time. 
						break;															
					}
				}//end nested for
			}// end CreateEntity

			else if( action == manipulate.UpdateEntity ){								// user has moved an entity from one place value location to another

				// check each entity to see which one had the user moved around and then update its information accordantly
				for( int i = 0; i <= (theEntities.Length - 1); ++i ){
					if( theEntities[i].power != theEntities[i].mass.PowerAndPlaceValue ){
						theEntities[i].mass.Update(theEntities[i].power);
					}
				}
			}// end UpdateEntity

			else if( action == manipulate.DeleteEntity ){
				// user wants to delete the entity so before the deletion make sure the user has selected an existing entity 
				for( int i = 0; i <= (theEntities.Length - 1); ++i ){
					if( (theEntities[i].area.Contains(mousePos)) && (theEntities[i].hasEntity == true) ){
					    theEntities[i].mass.Reset();
						theEntities[i].hasEntity = false;
						theEntities[i].renderInputBox = false;
						DrawCursor = false;
						
						break;
					}
				}// end nested for
			}// end DeleteEntity

		}// end Update(Point mousePos, ref manipulate action)


		// Did the user selected the input box 
		public bool InputManagement(Point mousePos){
			entityWaitingForInput = null;
				
			// did user click on the input box, if user did then allow input of a digit from 0 - 9
			for( int i = 0; i <= (theEntities.Length - 1); ++i ){
				if( (theEntities[i].area.Contains(mousePos)) && (theEntities[i].hasEntity == true) ){					// make sure user click on an existing entity
						
					Rectangle inputRect = new Rectangle( theEntities[i].area.X + theEntities[i].mass.inputXCoord,		// X of the input box rect
														 theEntities[i].area.Y + theEntities[i].mass.inputYCoord,		// Y
														 inputBoxTexture.Width,											// width
														 inputBoxTexture.Height );										// height

					if( inputRect.Contains(mousePos) ){
						entityWaitingForInput = i;
						return true;
					}
						
					break;
				}
			}//end nested for

			DrawCursor = false;
			return false;
		}//end InputManagement


		// keyboard pooling for input of digits within the range of 0 - 9
		public void InputManagement(ref KeyboardState kb, ref KeyboardState previousKbState){
			bool hasUserEnterValue = false;

			if( (kb.IsKeyDown(Keys.D1) && previousKbState.IsKeyUp(Keys.D1)) || (kb.IsKeyDown(Keys.NumPad1) && previousKbState.IsKeyUp(Keys.NumPad1)) ){
 
				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 1) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}
					
				theEntities[entityWaitingForInput.Value].mass.InputValue(1);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D2) && previousKbState.IsKeyUp(Keys.D2)) || (kb.IsKeyDown(Keys.NumPad2) && previousKbState.IsKeyUp(Keys.NumPad2)) ){
					
				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 2) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}
					
				theEntities[entityWaitingForInput.Value].mass.InputValue(2);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D3) && previousKbState.IsKeyUp(Keys.D3)) || (kb.IsKeyDown(Keys.NumPad3) && previousKbState.IsKeyUp(Keys.NumPad3)) ){

				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 3) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}

				theEntities[entityWaitingForInput.Value].mass.InputValue(3);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D4) && previousKbState.IsKeyUp(Keys.D4)) || (kb.IsKeyDown(Keys.NumPad4) && previousKbState.IsKeyUp(Keys.NumPad4)) ){

				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 4) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}

				theEntities[entityWaitingForInput.Value].mass.InputValue(4);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D5) && previousKbState.IsKeyUp(Keys.D5)) || (kb.IsKeyDown(Keys.NumPad5) && previousKbState.IsKeyUp(Keys.NumPad5)) ){

				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 5) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}

				theEntities[entityWaitingForInput.Value].mass.InputValue(5);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D6) && previousKbState.IsKeyUp(Keys.D6)) || (kb.IsKeyDown(Keys.NumPad6) && previousKbState.IsKeyUp(Keys.NumPad6)) ){

				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 6) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}

				theEntities[entityWaitingForInput.Value].mass.InputValue(6);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D7) && previousKbState.IsKeyUp(Keys.D7)) || (kb.IsKeyDown(Keys.NumPad7) && previousKbState.IsKeyUp(Keys.NumPad7)) ){

				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 7) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}

				theEntities[entityWaitingForInput.Value].mass.InputValue(7);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D8) && previousKbState.IsKeyUp(Keys.D8)) || (kb.IsKeyDown(Keys.NumPad8) && previousKbState.IsKeyUp(Keys.NumPad8)) ){

				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 8) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}

				theEntities[entityWaitingForInput.Value].mass.InputValue(8);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D9) && previousKbState.IsKeyUp(Keys.D9)) || (kb.IsKeyDown(Keys.NumPad9) && previousKbState.IsKeyUp(Keys.NumPad9)) ){

				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 9) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}

				theEntities[entityWaitingForInput.Value].mass.InputValue(9);
				hasUserEnterValue = true;
			}
			else if( (kb.IsKeyDown(Keys.D0) && previousKbState.IsKeyUp(Keys.D0)) || (kb.IsKeyDown(Keys.NumPad0) && previousKbState.IsKeyUp(Keys.NumPad0)) ){

				// prevent unneccessary processing of the same information
				if( theEntities[entityWaitingForInput.Value].mass.UserInput.HasValue && (theEntities[entityWaitingForInput.Value].mass.UserInput.Value == 0) ){
					hasUserEnterValue = true;
					goto JUMP1;
				}

				theEntities[entityWaitingForInput.Value].mass.InputValue(0);
				hasUserEnterValue = true;
			}
				
		JUMP1:
			if(hasUserEnterValue){
				//drawCursor = false;
				//entityWaitingForInput = null;
			}

		}// end InputManagement(ref KeyboardState kb, ref KeyboardState previousKbState)


		// allow user to use the mouse scroll to change the value of an entity while hovering over its input box
		public void InputMangement(Point mousePos, float wheelValue, float wheelLastValue){
			// did the user hover the mouse over an input box
			for( int i = 0; i <= (theEntities.Length - 1); ++i ){
				if( (theEntities[i].area.Contains(mousePos)) && (theEntities[i].hasEntity == true) ){					// make sure user hover over an existing entity
						
					Rectangle inputRect = new Rectangle( theEntities[i].area.X + theEntities[i].mass.inputXCoord,		// X of the input box rect
														 theEntities[i].area.Y + theEntities[i].mass.inputYCoord,		// Y
														 inputBoxTexture.Width,											// width
														 inputBoxTexture.Height );										// height

					if( inputRect.Contains(mousePos) && theEntities[i].mass.UserInput.HasValue ){						// make sure the mouse pointer is within the input box
						
						if( (wheelValue > wheelLastValue) && (wheelValue % ControlVariables.mouseWheelNotch == 0) ){						// user scroll up, value increment, value change per notch rotation
							byte userValue = theEntities[i].mass.UserInput.Value;

							if( (++userValue) > 9 )																		// range checking
								userValue = 0;

							theEntities[i].mass.InputValue(userValue);
							
						}
						else if( (wheelValue < wheelLastValue) && (wheelValue % ControlVariables.mouseWheelNotch == 0) ){				// user scroll down so value decrement, value change per notch rotation
							sbyte userValue = (sbyte) theEntities[i].mass.UserInput.Value;

							if( (--userValue) < 0 )																		// range checking
								userValue = 9;

							theEntities[i].mass.InputValue((byte)userValue);
						}
					}
					else{ // entity doesn't have a value yet so the scroll action will start the entity with a zero.
						if( wheelValue != wheelLastValue ){
							theEntities[i].mass.InputValue(0);
						}
					}
						
					break;
				}//end nested if
			}//end for
		}//end InputMangement(Point mousePos, float wheelValue, float wheelLastValue)
		#endregion



		#region Methods - Draw
		public void Draw(ref SpriteBatch spriteBatch, ref SpriteFont font){
			int total = 0;

			for( int i = 0; i <= (theEntities.Length - 1); ++i ){
				if( theEntities[i].hasEntity ){

					spriteBatch.Draw( inputBoxTexture, 
									  new Rectangle( theEntities[i].area.X + theEntities[i].mass.inputXCoord, theEntities[i].area.Y + theEntities[i].mass.inputYCoord, inputBoxTexture.Width, inputBoxTexture.Height),
									  Color.White);

					if(DrawBase){
						float xOffset = font.MeasureString( theEntities[i].mass.BaseString ).X / 2 ;  
						spriteBatch.DrawString( font, theEntities[i].mass.BaseString, new Vector2( theEntities[i].area.X + (theEntities[i].area.Width / 2) - xOffset, theEntities[i].area.Y + theEntities[i].mass.baseYcoord), Color.Black);
					}
				
					if(DrawCalculation){
						spriteBatch.DrawString( font, theEntities[i].mass.CalculationString, new Vector2( theEntities[i].area.X + 3, theEntities[i].area.Y + theEntities[i].mass.calYCoord), Color.Black);
						
						if( theEntities[i].mass.CalculationString != string.Empty ){
							spriteBatch.Draw( blackLineTexture, new Rectangle(theEntities[i].area.X + 5, theEntities[i].area.Y + 240, 90, 1), null, Color.White);
						}
					}

					if(DrawCursor){
						spriteBatch.Draw( cursorTexture, 
										  new Rectangle( theEntities[entityWaitingForInput.Value].area.X + theEntities[entityWaitingForInput.Value].mass.inputXCoord, theEntities[entityWaitingForInput.Value].area.Y + theEntities[entityWaitingForInput.Value].mass.inputYCoord, cursorWdith, cursorHeight),
										  cursorRect,
										  Color.White );
					}

					
					// if the entity has a value then draw it within inside the input box, drawing the value with no base or calcuation show's that the value by itself is meaningless
					// it's only when we add in the concept of place value do those values has significant meaning.
					if( theEntities[i].mass.UserInput.HasValue ){
						byte value = theEntities[i].mass.UserInput.Value;
						
						Vector2 charProperties = font.MeasureString( value.ToString() );

						spriteBatch.DrawString(	font, 
												value.ToString(), 
												new Vector2( theEntities[i].area.X + theEntities[i].mass.inputXCoord + (inputBoxTexture.Width / 2) - (charProperties.X / 2) , theEntities[i].area.Y + theEntities[i].mass.inputYCoord + (inputBoxTexture.Height / 2) - (charProperties.Y / 2) ),
												Color.Red );

						// add to total
						#region switch for calculating total
						switch(theEntities[i].power){
							case pwr.ZeroPwr:
								total += 1 * value;
								break;

							case pwr.FirstPwr:
								total += 10 * value;
								break;

							case pwr.SecondPwr:
								total += 100 * value;
								break;

							case pwr.ThirdPwr:
								total += 1000 * value;
								break;

							case pwr.FourthPwr:
								total += 10000 * value;
								break;

							case pwr.FifthPwr:
								total += 100000 * value;
								break;

							case pwr.SixthPwr:
								total += 1000000 * value;
								break;

							case pwr.SeventhPwr:
								total += 10000000 * value;
								break;
						}
						#endregion
					}
				}// end nested if
			}// end for

			if(DrawTotalValue){
				Vector2 strMeasure = font.MeasureString(total.ToString());

				spriteBatch.DrawString(font, total.ToString(), new Vector2( 280 - strMeasure.X, 325 - strMeasure.Y), Color.Black);
			}

		}// end draw
		#endregion

	}
}
