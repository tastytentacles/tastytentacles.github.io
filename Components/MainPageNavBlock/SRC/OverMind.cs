using Godot;
using System;

public class OverMind : Spatial {
    JavaScriptObject j_callback;
    
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        //JavaScript.Eval("window.location = 'https;//www.google.com'");
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }

    public void test_func() {
        GD.Print("test");
    }
}
