using Godot;
using System;
using System.Collections.Generic;

struct Cmd 
{
    public float move_forward;
    public float move_right;
    public float move_up;
}

class DiseasedData
{
    public Player Attacker;
    public float TimeSinceDiseased;
    public Weapon Inflictor;

    public DiseasedData(Player attacker, Weapon inflictor, float timeSinceDiseased)
    {
        Attacker = attacker;
        TimeSinceDiseased = timeSinceDiseased;
        Inflictor = inflictor;
    }
}

public class Player : KinematicBody
{
    float mouseSensitivity = 0.2f;
    float cameraAngle = 0F;

    private int _pipebombLimit = 7;

    // states
    private float _tranquilisedLength = 0f;
    private bool _tranquilised = false;
    private float _timeSinceTranquilised = 0f;
    public float TimeSinceTranquilised {
        get { return _timeSinceTranquilised; }
        set {
            _timeSinceTranquilised = value;
            if (_tranquilised)
            {
                if (_timeSinceTranquilised >= _tranquilisedLength)
                {
                    Tranquilised = false;
                }
            }
        }
    }
    public bool Tranquilised {
        get { return _tranquilised; }
        set {
            _tranquilised = value;
            _timeSinceTranquilised = 0f;
        }
    }

    private List<DiseasedData> _diseasedBy = new List<DiseasedData>();
    private float _diseasedInterval = 0f;

    // physics
    private float gravity = 27.0f;
    private float friction = 6;
    private Vector3 up = new Vector3(0,1,0);
    // stairs
    float maxStairAngle = 20F;
    float stairJumpHeight = 9F;

    // movement
    public float moveSpeed = 15.0f;               // Ground move speed
    public float runAcceleration = 14.0f;         // Ground accel
    public float runDeacceleration = 10.0f;       // Deacceleration that occurs when running on the ground
    public float airAcceleration = 2.0f;          // Air accel
    public float airDecceleration = 2.0f;         // Deacceleration experienced when opposite strafing
    public float airControl = 0.3f;               // How precise air control is
    public float sideStrafeAcceleration = 50.0f;  // How fast acceleration occurs to get up to sideStrafeSpeed
    public float sideStrafeSpeed = 1.0f;          // What the max speed to generate when side strafing
    public float jumpSpeed = 8.0f;                // The speed at which the character's up axis gains when hitting jump
    public float moveScale = 1.0f;
   
    private Vector3 moveDirectionNorm = new Vector3();
    private Vector3 playerVelocity = new Vector3();
    
    // Player commands, stores wish commands that the player asks for (Forward, back, jump, etc)
    private Cmd _cmd;

    // Nodes
    Spatial head;
    Camera camera;
    RayCast stairCatcher;
    Label HealthLabel;
    Label ArmourLabel;

    private Main _mainNode;
    private Main MainNode {
        get {
            if (_mainNode == null)
            {
                _mainNode = (Main)GetNode("/root/OpenFortress/Main");
            }
            return _mainNode;
        }
    }

 


    Network _network;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here

        _network = (Network)GetNode("/root/OpenFortress/Network");
    }

    public override void _Input(InputEvent e)
    {

    }

    public override void _PhysicsProcess(float delta)
    {

    }


    public void Spawn(Vector3 loc)
    {
        this.SetTranslation(loc);
        // do other stuff around being dead etc 
        
        GD.Print(this.Class.ToString());
        this.CurrentHealth = this.Class.Health;
        this.CurrentArmour = this.Class.Armour / 2;
        this._currentShells = Math.Abs(this.Class.MaxShells / 2);
        this._currentNails = Math.Abs(this.Class.MaxNails / 2);
        this._currentRockets = Math.Abs(this.Class.MaxRockets / 2);
        this._currentCells = Math.Abs(this.Class.MaxCells / 2);
        this._currentGren1 = Math.Abs(this.Class.MaxGren1 / 2);
        this._currentGren2 = Math.Abs(this.Class.MaxGren2 / 2);
    }


}
