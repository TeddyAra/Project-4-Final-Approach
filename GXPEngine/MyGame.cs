using System;
using GXPEngine;
using System.Drawing;
using System.Collections.Generic;
using SharpDX.DirectInput;

public class MyGame : Game {
	DirectInput input;
	Guid joystickGuid;
	Joystick joystick;
	JoystickState state;

	Sprite sprite1;
	Sprite sprite2;

	Solid solid;
	Item item;

	Player player;
	public List<Line> lines;

	public MyGame() : base(1920, 1080, false, false) {
		targetFps = 60;

		lines = new List<Line>();

		ControllerSetup();

		sprite1 = new Sprite("solid.png");
		AddChild(sprite1);
        sprite1.SetOrigin(sprite1.width / 2, sprite1.height / 2);

		sprite2 = new Sprite("solid.png");
		AddChild(sprite2);
		sprite2.SetOrigin(sprite2.width / 2, sprite2.height / 2);

		solid = new Solid();
		AddChild(solid);
		solid.SetXY(900, 200);

		item = new Item();
		AddChild(item);
		item.SetXY(game.width - 900, 200);

        player = new Player(joystickGuid, lines);
		AddChild(player);
    }

	void Update() {
		ReadController();
	}

	static void Main() {
		new MyGame().Start();
	}

	void ControllerSetup() { 
		input = new DirectInput();
		joystickGuid = Guid.Empty;

		// Checks for 'Gamepads'
		foreach (var deviceInstance in input.GetDevices(
			DeviceType.Gamepad, DeviceEnumerationFlags.AllDevices)) {
			joystickGuid = deviceInstance.InstanceGuid;
		}

		// Checks for 'Joysticks' if there are no 'Gamepads'
		if (joystickGuid == Guid.Empty) { 
			foreach (var deviceInstance in input.GetDevices(
				DeviceType.Joystick, DeviceEnumerationFlags.AllDevices)) {
				joystickGuid = deviceInstance.InstanceGuid;
			}
		}

		// If a 'Gamepad' or 'Joystick' hasn't been found, close the window
		if (joystickGuid == Guid.Empty) {
			Console.WriteLine("No controller detected");
			Environment.Exit(1);
		}

		// Setup controller
		joystick = new Joystick(input, joystickGuid);
		Console.WriteLine($"Controller {joystickGuid}");

		joystick.Properties.BufferSize = 128;
		joystick.Properties.DeadZone = 2000;
	}

	void ReadController() {
		// Read controller input
        joystick.Acquire();
		state = joystick.GetCurrentState();

        sprite1.SetXY(player.leftHandPos.x, player.leftHandPos.y);
        sprite2.SetXY(player.rightHandPos.x, player.rightHandPos.y);
    }
}