// features:	hold the char 'D' and click on any existing entity to delet it.
//				hover the mouse over any input box and increment/decrement the value with the mouse wheel.
//				hover the mouse over any switch and turn on/off the switch with the mouse wheel.
//				turn all binary switch off by pressing letter 'O' + 'F'
//				turn all binary switch on by pressing letter 'O' + 'N'

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using manipulate = Teach_BaseNumberSystem.ControlVariables.Manipulate;

namespace Teach_BaseNumberSystem{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1:Microsoft.Xna.Framework.Game {

		#region Fields

		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		int displayWidth = 1200;
		int displayHeight = 700;


		//font 
		SpriteFont segeoUIFont;
		SpriteFont smallSegeoUIFont;


		// textures
		Texture2D backgroundText;

		EntityControl entityController;
		BinaryControl binaryController;

		//switch
		bool inputPooling = false;
		bool isInputDrag = false;

		// options text
		struct option{
			public Texture2D checkBoxTexture;
			public Rectangle destinationRect;				// location where the check box will be drawn
			public Rectangle sourceRect;					// coordinate within the texture file to render
			public string text;								// the option that is avalible for selection
		}
		option base10Option;
		option calculationOption;
		option binarySwitchOption;
		option binaryPwrOption;
		option binaryCalcuOption;

		// Entity: input box and addition sign
		struct entity{
			public bool isSelected;
			public Texture2D texture;
			public Rectangle destinationRect;
		}
		entity plusSign;
		entity inputBox;


		// drag coordinate
		Point dragCoordinate = new Point();


		// Inorder for user to create a new inputBox entity at a specific place value position, the user must click on the inputBox icon,
		// hold the left mouse click and bring the pointer into the hot zone and release the left mouse click. Once an inputBox entity is placed
		// at a specific place value position the user is then allowed to enter a digit from 0 - 9
		// the entityHotZone also help control entity selection and deletion
		Rectangle entityHotZone = new Rectangle(320, 25, 870, 300);

		// the hot zone for binary operation control
		Rectangle binaryHotZone = new Rectangle(320, 375, 870, 300);
		
		MouseState previousMouseState;

		KeyboardState kb;
		KeyboardState previousKB;
		#endregion 



		public Game1() {
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			graphics.PreferredBackBufferWidth = displayWidth;
			graphics.PreferredBackBufferHeight = displayHeight;
			graphics.ApplyChanges();

			this.Window.Title = "ILP Binary and Decimals Concepts";
			this.IsMouseVisible = true;
		}


		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize(){

			base.Initialize();
		}


		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent(){
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// fonts
			segeoUIFont = Content.Load<SpriteFont>(@"Fonts/segoeUI");
			smallSegeoUIFont = Content.Load<SpriteFont>(@"Fonts/smallSegoeUI");

			backgroundText = Content.Load<Texture2D>(@"Textures/theSystemBg");


			Texture2D checkboxoption = Content.Load<Texture2D>(@"Textures/checkBox");

			// properites belonging to "Show base and place value." option
			base10Option.checkBoxTexture = checkboxoption;
			base10Option.destinationRect = new Rectangle(5, 5, 30, 30);
			base10Option.sourceRect = new Rectangle(0, 0, 30, 30);
			base10Option.text = "Show base and place value.";

			// properties belonging to "Show Calculation." option
			calculationOption.checkBoxTexture = checkboxoption;
			calculationOption.destinationRect = new Rectangle(5, 40, 30, 30);
			calculationOption.sourceRect = new Rectangle(0, 0, 30, 30);
			calculationOption.text = "Show base calculation.";

			// properties belonging to "Show binary switch." option
			binarySwitchOption.checkBoxTexture = checkboxoption;
			binarySwitchOption.destinationRect = new Rectangle(5, 75, 30, 30);
			binarySwitchOption.sourceRect = new Rectangle(0, 0, 30, 30);
			binarySwitchOption.text = "Show binary switch.";

			// properties belonging to "Show binary power." option
			binaryPwrOption.checkBoxTexture = checkboxoption;
			binaryPwrOption.destinationRect = new Rectangle(5, 110, 30, 30);
			binaryPwrOption.sourceRect = new Rectangle(0, 0, 30, 30);
			binaryPwrOption.text = "Show binary power.";

			// properties belonging to "show binary calcuation." option
			binaryCalcuOption.checkBoxTexture = checkboxoption;
			binaryCalcuOption.destinationRect = new Rectangle(5, 145, 30, 30);
			binaryCalcuOption.sourceRect = new Rectangle(0, 0, 30, 30);
			binaryCalcuOption.text = "Show binary calculation.";
		

			inputBox.texture = Content.Load<Texture2D>(@"Textures/inputBox");	
			inputBox.destinationRect = new Rectangle(120, 350, 80, 60);
			inputBox.isSelected = false;

			plusSign.texture = Content.Load<Texture2D>(@"Textures/plusSign");	
			plusSign.destinationRect = new Rectangle(140, 450, 40, 40);
			plusSign.isSelected = false;
			

			entityController = new EntityControl( inputBox.texture, Content.Load<Texture2D>(@"Textures/redCursor"), Content.Load<Texture2D>(@"Textures/blackLine") );
			binaryController = new BinaryControl( Content.Load<Texture2D>(@"Textures/onOffSwitch"), Content.Load<Texture2D>(@"Textures/blackLine") );
		}


		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent(){
			// TODO: Unload any non ContentManager content here
		}


		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime){

			// Allows the game to exit
			if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();

			
			kb = Keyboard.GetState();

			// key combo to turn all switch on or off
			if( kb.IsKeyDown(Keys.F) && kb.IsKeyDown(Keys.O) ){			// turn all switch off key, combo char 'O' + 'F'
				binaryController.SwitchManagement(false);
			}
			else if( kb.IsKeyDown(Keys.N) && kb.IsKeyDown(Keys.O) ){	// turn all switch on, key combo char 'O' + 'N'
				binaryController.SwitchManagement(true);
			}


			MouseState currentMouseState = Mouse.GetState();

			Point point = new Point(currentMouseState.X, currentMouseState.Y);
			dragCoordinate = point;

			if( (currentMouseState.LeftButton == ButtonState.Pressed) && (previousMouseState.LeftButton == ButtonState.Released) ){

				// stop pooling for kb input when user click outside the input box area
				entityController.DrawCursor = false;
				inputPooling = false;

				#region base option
				if( base10Option.destinationRect.Contains(point) ){
					entityController.DrawBase = !entityController.DrawBase;

					if(entityController.DrawBase){
						base10Option.sourceRect.Y = 30;						// mark the selection
					}
					else{
						base10Option.sourceRect.Y = 0;						// unselect
					}
				}
				#endregion

				#region base calculation option
				else if( calculationOption.destinationRect.Contains(point) ){
					entityController.DrawCalculation = !entityController.DrawCalculation;

					if(entityController.DrawCalculation){	
						calculationOption.sourceRect.Y = 30;				// mark the selection
					}
					else{
						calculationOption.sourceRect.Y = 0;					// unselect
					}
				}
				#endregion

				#region binary switch option
				else if( binarySwitchOption.destinationRect.Contains(point) ){
					binaryController.DrawSwitch = !binaryController.DrawSwitch;

					if(binaryController.DrawSwitch){
						binarySwitchOption.sourceRect.Y = 30;				// mark the selection
					}
					else{
						binarySwitchOption.sourceRect.Y = 0;				// unselect
					}
				}
				#endregion

				#region binary power option
				else if( binaryPwrOption.destinationRect.Contains(point) ){
					binaryController.DrawPower = !binaryController.DrawPower;

					if(binaryController.DrawPower){
						binaryPwrOption.sourceRect.Y = 30;					// mark the selection
					}
					else{
						binaryPwrOption.sourceRect.Y = 0;					// unselect
					}
				}
				#endregion

				#region binary calculation option
				else if( binaryCalcuOption.destinationRect.Contains(point) ){
					binaryController.DrawCalculation = !binaryController.DrawCalculation;

					if(binaryController.DrawCalculation){
						binaryCalcuOption.sourceRect.Y = 30;					// mark the selection
					}
					else{
						binaryCalcuOption.sourceRect.Y = 0;					// unselect
					}
				}
				#endregion


				#region inputBox icon clicked on
				else if( inputBox.destinationRect.Contains(point) ){
					inputBox.isSelected = true;
					isInputDrag = true;
				}
				#endregion

				#region plus sign icon clicked on
				else if( plusSign.destinationRect.Contains(point) ){
					entityController.DrawTotalValue = !entityController.DrawTotalValue;
					binaryController.DrawTotalValue = !binaryController.DrawTotalValue;
				}
				#endregion


				#region Entity hot zone, the interative area of the game
				else if( entityHotZone.Contains(point) && kb.IsKeyDown(Keys.D) ){		// deletion of an existing entity
					entityController.EntityManagement(point, manipulate.DeleteEntity);	
				}
				else if( entityHotZone.Contains(point) ){								// did user click within the input box area?
					inputPooling = entityController.InputManagement(point);
				}
				#endregion


				#region Binary hot zone, the interative area of the game
				else if( binaryHotZone.Contains(point) ){								// did sure click on the on/off swtich?
					binaryController.SwitchManagement(point);
				}
				#endregion

			}// end mouse press event

			else if(inputPooling){
				entityController.DrawCursor = true;
				entityController.InputManagement(ref kb, ref previousKB);
			}


			// allow the user to change the value of the entity with the mouse scroll while hovering over the input box
			// the mouse pointer has to be inside the enitty hot zone
			if( entityHotZone.Contains(point) ){
				entityController.InputMangement(point, currentMouseState.ScrollWheelValue, previousMouseState.ScrollWheelValue);
			}

			// allow the user to turn on/off the binary switch with the mouse scroll while hovering over the switch
			else if( binaryHotZone.Contains(point) ){
				binaryController.SwitchManagement(point, currentMouseState.ScrollWheelValue, previousMouseState.ScrollWheelValue);
			}


			// "dragging" of the icon

			// "dropping" part of the "drag and drop" of the icon
			if( (currentMouseState.LeftButton == ButtonState.Released) && (previousMouseState.LeftButton == ButtonState.Pressed) ){
				isInputDrag = false;

				// did the user drop the icon in the hotzone?
				if( entityHotZone.Contains(point) && inputBox.isSelected ){
					entityController.EntityManagement(point, manipulate.CreateEntity);
				}

				inputBox.isSelected = false;
			}



			// control binary switch with the number key
			if( binaryController.DrawSwitch && !inputPooling ){
				binaryController.SwitchManagement(kb, previousKB);
			}


			// control the rate of the inputBox border animation 
			entityController.Update(gameTime);

			previousKB = kb;
			previousMouseState = currentMouseState;

			base.Update(gameTime);
		}


		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.White);

			spriteBatch.Begin();

				spriteBatch.Draw(backgroundText, Vector2.Zero, Color.White);

				// "Show base and place value" option 
				spriteBatch.Draw(base10Option.checkBoxTexture, base10Option.destinationRect, base10Option.sourceRect, Color.White);
				spriteBatch.DrawString(smallSegeoUIFont, base10Option.text, new Vector2(base10Option.destinationRect.X + base10Option.checkBoxTexture.Width + 5.0f, base10Option.destinationRect.Y), Color.Black);
				
				// "Show base calculation" option
				spriteBatch.Draw(calculationOption.checkBoxTexture, calculationOption.destinationRect, calculationOption.sourceRect, Color.White);
				spriteBatch.DrawString(smallSegeoUIFont, calculationOption.text, new Vector2(calculationOption.destinationRect.X + calculationOption.checkBoxTexture.Width + 5.0f, calculationOption.destinationRect.Y), Color.Black);

				// "Show binary switch" option
				spriteBatch.Draw(binarySwitchOption.checkBoxTexture, binarySwitchOption.destinationRect, binarySwitchOption.sourceRect, Color.White);
			    spriteBatch.DrawString(smallSegeoUIFont, binarySwitchOption.text, new Vector2(binarySwitchOption.destinationRect.X + binarySwitchOption.checkBoxTexture.Width + 5.0f, binarySwitchOption.destinationRect.Y), Color.Black);

				// "Show binary power" option
				spriteBatch.Draw(binaryPwrOption.checkBoxTexture, binaryPwrOption.destinationRect, binaryPwrOption.sourceRect, Color.White);
				spriteBatch.DrawString(smallSegeoUIFont, binaryPwrOption.text, new Vector2(binaryPwrOption.destinationRect.X + binaryPwrOption.checkBoxTexture.Width + 5.0f, binaryPwrOption.destinationRect.Y), Color.Black);

				// "Show binary calculation" option
				spriteBatch.Draw(binaryCalcuOption.checkBoxTexture, binaryCalcuOption.destinationRect, binaryCalcuOption.sourceRect, Color.White);
				spriteBatch.DrawString(smallSegeoUIFont, binaryCalcuOption.text, new Vector2(binaryCalcuOption.destinationRect.X + binaryCalcuOption.checkBoxTexture.Width + 5.0f, binaryCalcuOption.destinationRect.Y), Color.Black);
					
				// user input box 
				spriteBatch.Draw(inputBox.texture, inputBox.destinationRect, Color.White);

				// dragging of the input box icon
				if(isInputDrag){
					spriteBatch.Draw(inputBox.texture, new Vector2( (float)dragCoordinate.X - (inputBox.texture.Width / 2.0f), (float)dragCoordinate.Y - (inputBox.texture.Height / 2.0f) ), new Color(255, 255, 255, 80) );
				}

				// plus symbol
				if(binaryController.DrawTotalValue){
					spriteBatch.Draw(plusSign.texture, plusSign.destinationRect, new Rectangle(40, 0, 40, 40), Color.White);
				}
				else{
					spriteBatch.Draw(plusSign.texture, plusSign.destinationRect, new Rectangle(0, 0, 40, 40), Color.White);
				}


				entityController.Draw(ref spriteBatch, ref segeoUIFont);

				binaryController.Draw(ref spriteBatch, ref segeoUIFont);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
