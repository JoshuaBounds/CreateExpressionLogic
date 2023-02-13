# CreateExpressionLogic
Generates basic face expression logic for VRChat FX animator controllers

Installation:
  - In your unity project create a new folder in Assets and call it Editor, and place the file in there.
  - Once loaded, you can navigate in Unity to: Tools/Sidega/Create Expression Logic

Usage:
  - Animator Controller: The target FX animator controller
  - Target Layer: The layer to generate/regenerate state machine logic within
  - Default Animation Clip: Default motion given to all created states
  - Ambidextrous Logic: Changes the setup from 64 states where both controllers must give the correct index to achieve each expression. To a setup of just 36 states where the left and right controller indexes can be reversed and still achieve the same expression.
  - Only Transitions: Skips regenerating the layer and states, and instead clears and then regenerates all the animator transitions.
  - Transition duration: Transition duration given to every generated animator transition
