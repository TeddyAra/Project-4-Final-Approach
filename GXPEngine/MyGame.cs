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
	Interactable interactable;
	Level level;

	public Sprite space;

	public Player player;
	public Camera camera;
	public List<Line> lines;
	public List<Interactable> interactables;
	public List<Door> doors;

	Sound ambiance;

	public MyGame() : base(1920, 1080, false, false) {
		targetFps = 60;

        lines = new List<Line>();
		interactables = new List<Interactable>();
		doors = new List<Door>();

        LoadLevel("tiledLevel.tmx");

        ControllerSetup();

		player = new Player(joystickGuid, lines, interactables);
		AddChild(player);

		camera = new Camera(0, 0, 1920, 1080);
		player.AddChild(camera);

		ambiance = new Sound("game ambiance final.wav", true);		
		ambiance.Play();
    }

	void Update() {
        joystick.Acquire();
        state = joystick.GetCurrentState();

		if (space != null) {
			space.SetXY(player.pos.x, player.pos.y);
		}
	}

	void DestroyAll() {
		List<GameObject> children = GetChildren(true);
		foreach (GameObject child in children) { 
			child.Destroy();
		}
	}

	void LoadLevel(string name) {
		DestroyAll();
		level = new Level(name);
		AddChild(level);
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
}