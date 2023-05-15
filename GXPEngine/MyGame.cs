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

	public Player player;
	public Camera camera;
	public List<Line> lines;
	public List<Interactable> interactables;
	public List<Door> doors;

	public MyGame() : base(1920, 1080, false, false) {
		targetFps = 60;

		lines = new List<Line>();
		interactables = new List<Interactable>();
		doors = new List<Door>();

        LoadLevel("tiledLevel.tmx");

        ControllerSetup();

		/*for (int i = 0; i < width / 32; i++) {
			solid = new Solid();
			AddChild(solid);
			solid.SetXY(i * 32 + 16, 16);

            solid = new Solid();
            AddChild(solid);
            solid.SetXY(i * 32 + 16, height - 16);
        }

		for (int i = 1; i < height / 32; i++) {
			solid = new Solid();
			AddChild(solid);
			solid.SetXY(16, i * 32 + 16);

			solid = new Solid();
			AddChild(solid);
			solid.SetXY(width - 16, i * 32 + 16);
		}

        interactable = new Interactable("map1");
		AddChild(interactable);
        interactable.SetXY(game.width / 3, game.height / 3);
		interactables.Add(interactable);

        interactable = new Interactable("map2");
        AddChild(interactable);
        interactable.SetXY(game.width / 3 * 2, game.height / 3);
		interactables.Add(interactable);

        interactable = new Interactable("code");
        AddChild(interactable);
        interactable.SetXY(game.width / 3, game.height / 3 * 2);
		interactables.Add(interactable);

        solid = new Solid("goal");
        AddChild(solid);
        solid.SetXY(game.width / 3 * 2, game.height / 3 * 2);

		solid = new Solid();
		AddChild(solid);
		solid.SetXY(game.width / 2, game.height / 2);

        item = new Item("areaBall", lines);
		AddChild(item);
		item.pos = new Vec2(game.width - 900, 200);*/

		player = new Player(joystickGuid, lines, interactables);
		AddChild(player);

		camera = new Camera(0, 0, 1920, 1080);
		player.AddChild(camera);
    }

	void Update() {
        joystick.Acquire();
        state = joystick.GetCurrentState();

        camera.rotation = -player.rotation;
	}

	void DestroyAll() {
		List<GameObject> children = GetChildren(true);
		foreach (GameObject child in children) { 
			child.Destroy();
		}
	}

	void LoadLevel(string name) {
		DestroyAll();
		AddChild(new Level(name));
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