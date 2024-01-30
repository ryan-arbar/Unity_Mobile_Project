__________________________________________________________________________________________

Package "Legs Animator"
Version 1.0.1.4

Made by FImpossible Creations - Filip Moeglich
http://fimpossiblecreations.pl
FImpossibleCreations@Gmail.com or FImpossibleGames@Gmail.com

__________________________________________________________________________________________

Asset Store Page: https://assetstore.unity.com/publishers/37262
Youtube: https://www.youtube.com/channel/UCDvDWSr6MAu1Qy9vX4w8jkw
Facebook: https://www.facebook.com/FImpossibleCreations
Twitter (@FimpossibleC): https://twitter.com/FImpossibleC
Discord Server: https://www.discord.gg/Y3WrzQp

___________________________________________________


Package Contests:

User Manual: Check the .pdf file

DEMO - Legs Animator.unitypackage
Package contains scenes with features examples of the plugin.

Legs Animator - Assembly Definitions.unitypackage (Supported since Unity 2017)
Assembly Definition files to speed up compilation time of your project (Fimpossible Directories will have no influence on compilation time of whole project)


Link to the Legs Animator implementations with other IK plugins (Final IK):
https://drive.google.com/drive/folders/1M5FZvrLCqUlsVa8iqNvdtDMmfr4uzDNV?usp=sharing


__________________________________________________________________________________________
Description:

Solve all of your leg animating problems with Legs Animator!

Legs Animator is component which provides a lot of features for characters with legs... so for almost all kinds of creatures.

List of features:

Aligning legs on uneven terrain
Handling leg attachement points (gluing)
Executing complex attachement transition animations (idle gluing)
Automatic turning-rotating in place leg animation (idle gluing)
Fixing sliding feet for no-root motion animations (movement gluing)
Animating hips stability giving realistic feel to the animations
Providing API for custom extensions of Legs Animator
Automatic strafe and 360 movement animating module (using single clip)
Push Impulses API (for landing bend impacts and others)
Extra helper features for automatic animations enchancing
Step Events handling for step sounds and particles
Fast setup and setup speedup tools
Works on any type of rig
Highly Optimized
Check Manual for more


__________________________________________________________________________________________

Version 1.0.1.4
- Added "Unity Humanoid IK" hint mode
- Added 'Override Direction' switch (coing) for 360 directional movement module

Version 1.0.1.3
- Added "Start Bone" raycast origin mode
- Added "Redirect Raycasting" module

Version 1.0.1.2
- Added buttons on the right to animator variables fields for quick select animator properies.
- Added Animal preset button (slightly different than insect preset)

Version 1.0.1.1
- Added Inverse Hint toggle under Setup->IK->IK Leg Settings
It will reverse bend direction for leg.
- Added support for script recompilling on script reload (unity 2022.3+)

Version 1.0.1 (containing hotfixes since v1.0.0)
- Added unscaled delta time switch
- Body Matrix Module - Selective Axis option
- (1.0.0.2.6) Now the Base module for auto-change parameters on animator states/tags has a feature to automatically detect top most weight layer to read animator states from
- (1.0.0.2.5) When gluing fade is almost value zero and becomes deactivated, its sheduling attachement refresh for next activation
- (1.0.0.2.4) Added Base module for auto-change parameters on animator states/tags being played
- New 2 modules: Utilities/Parameters -> Fade Gluing On Animator and Fade Legs System On Animator
- (1.0.0.2.3) Replaced float.MaxValue inside Mathf.SmoothDamp methods. float.MaxValue was unsafe and in rare cases was generating NaN errors.
- Few extra protections for NaN exceptions
- (1.0.0.2.2) Added User Teleport method
- Calibration for the optional spine and chest bone
- Updated 'All Fimpossible Creations Assembly Definitions' package file
- Fixed few GUI errors
- (1.0.0.2.1) Motion -> Gluing -> Unglue On - Now detects Yaw feet angle difference more precisely
- (1.0.0.2.0) Added hips NaN protections.
- Added Gluing Fade on animation tag and animator property value modules (can be added using project file field)
- (1.0.0.1.0) 360 movement extra refresh on re-activate
- User_OverwriteIKCoords() method for custom feet IK position control

Version 1.0.0:
> Initial release

