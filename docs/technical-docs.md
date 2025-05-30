# Architecture and Design

See our website ([car](https://xlrseatingbuck-org.github.io/unity-car.html)) ([plane](https://xlrseatingbuck-org.github.io/unity-plane.html)).

![main activity diagram](images/main%20activity.drawio.png)

# Classes

![main class diagram](images/class%20diagram.drawio.png)

## ExperienceDirector

The top level class for the entire project.
Stores the general state of the simulation, and reports what to do next back to the user.
Also reports a win or loss back to the user, and restarts or exits the game.

## OsmLoader

Loads the OSM data, which contains information about specific points on the map (mainly buildings and forest).
This is used to place trees in the proper area, that can then be lit on fire.

## BeaconTrigger

Tracks whether a vehicle is in range of a given marker. Reports this state to the ExperienceDirector to change the status text.

## FireExtinguishTracker

Stores the “extinguished” state of an individual fire.
Used to track if all fires have been extinguished, and reports this state to ExperienceDirector.

## CrashController

Handles collision between a vehicle and any other surface.
Triggers haptic feedback on collision.

## CarMovement

Main movement class for the firetruck. Gets input from controllers and applies forces to the truck via its wheels.

## JetMovement

Main movement class for the aeriel firefighter.
Gets input from controllers and applies forces to the plane. Handles all of the flight physics, including
simulating drag, calculating lift based on area of attack, and controlling steering/flaps.
Finally, this applies basic haptic vibration depending on the speed of the plane.

# HoseController

Handles moving the firetruck hose and firing water from it.

# BringFireDown

Handles collision between water and fire, and properly extinguishing the fire.
This signals to FireExtinguishTracker when the fire is extinguished.
This also grows the fire over time, triggering a loss when it gets too big.

# GasCollision

Handles shooting foam out of the plane.
Extinguishes the fire when the foam touches the fire.
This signals to FireExtinguishTracker when the fire is extinguished.

# IncreaseFire

Similar to the behavior of BringFireDown, this grows the fire over time, triggering a loss when it gets too big.

# CameraController

Handles camera switching based on whether the user is in VR or not.

 