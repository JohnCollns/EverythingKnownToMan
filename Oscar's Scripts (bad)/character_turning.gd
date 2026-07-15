extends Node2D

var scale_factor = 1.0

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.
	


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	scale_factor = clampf(scale_factor + (Input.get_axis("ui_left", "ui_right") * 0.1), -1, 1)
	transform.x = Vector2(scale_factor, 0.0)
