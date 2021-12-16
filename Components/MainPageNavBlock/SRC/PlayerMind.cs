using Godot;
using System;

public class PlayerMind : KinematicBody {
    [Export] Vector2 mouse_scaler = Vector2.One;
    [Export] float move_scaler = 1;
    [Export] float jump_power = 1;
    [Export] Vector3 gravity = new Vector3(0, -.25f, 0);
    Vector2 mouse_in;
    int[] keys_in = new int[4];

    private Camera camera;
    private TextureButton screen_lock_unlock_button;
    private RayCast floor_check;


    private Vector3 move_vector = Vector3.Zero;



    public void screen_lock() {
        Input.SetMouseMode(Input.MouseMode.Captured);
        screen_lock_unlock_button.Visible = false;
    }

    public void screen_unlock() {
        screen_lock_unlock_button.Visible = true;
    }


    public override void _Ready() {
        camera = GetNode<Camera>("Camera");
        floor_check = GetNode<RayCast>("floor_check");

        screen_lock_unlock_button = GetNode<TextureButton>("lock_button");
        screen_lock_unlock_button.Connect("pressed", this, nameof(screen_lock));
    }

    public override void _Input(InputEvent e) {
        if (e is InputEventMouseMotion em) {
            mouse_in = (em.Relative / OS.WindowSize.y) * 128;
        }

        if (e is InputEventKey ek) {
            switch (ek.Scancode) {
                case (int) KeyList.W:
                    keys_in[0] = ek.Pressed ? 1 : 0;
                    break;

                case (int) KeyList.A:
                    keys_in[1] = ek.Pressed ? 1 : 0;
                    break;

                case (int) KeyList.S:
                    keys_in[2] = ek.Pressed ? 1 : 0;
                    break;

                case (int) KeyList.D:
                    keys_in[3] = ek.Pressed ? 1 : 0;
                    break;
                
                default: break;
            }
        }
    }

    public override void _Process(float delta) {
        if (Input.GetMouseMode() != Input.MouseMode.Captured) {
            screen_unlock();
        } else {
            RotateY((-mouse_in.x * mouse_scaler.x) * delta);
            camera.RotateX((-mouse_in.y * mouse_scaler.y) * delta);

            Vector3 rot_cahce = camera.Rotation;
            rot_cahce.x = (float) Math.Min(1.2, Math.Max(-1.2, rot_cahce.x));
            camera.Rotation = rot_cahce;

            mouse_in = Vector2.Zero;
        }
    }

    public override void _PhysicsProcess(float delta) {
        Vector3 keys_vector = Vector3.Zero;

        if (floor_check.IsColliding()) {
            if (Array.Exists<int>(keys_in, n => n == 1))
                if (Input.IsKeyPressed((int) KeyList.Shift))
                    move_vector = move_vector * .99f;
                else
                    move_vector = move_vector * .95f;
            else
                move_vector = move_vector * .75f;
            
            if (Input.GetMouseMode() == Input.MouseMode.Captured) {
                keys_vector = new Vector3(keys_in[3] - keys_in[1], 0, keys_in[2] - keys_in[0]);
                keys_vector = keys_vector.Rotated(Vector3.Up, Rotation.y);

                move_vector += keys_vector * delta;

                move_vector += Vector3.Up
                    *  jump_power
                    * (Input.IsKeyPressed((int) KeyList.Space) ? 1 : 0);
            }
        }

        move_vector += gravity * delta;

        var output = MoveAndCollide((move_vector.Normalized() * move_vector.Length()) * move_scaler);
        if (output is KinematicCollision kc) {
            move_vector += -kc.Normal * kc.Normal.Dot(move_vector);
        }
    }
}
