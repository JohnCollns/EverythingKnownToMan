extends PathFollow2D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _physics_process(delta: float) -> void:
		#move along the path if nothing is in the road
	if (!(get_node("ResearcherPlaceholder/RayCast2D").is_colliding())):
		progress_ratio += 0.015 * delta * Input.get_axis("ui_left", "ui_right")
	
