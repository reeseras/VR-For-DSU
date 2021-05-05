# VR-For-DSU
I was hired as a VR Software developer to work on a proof of concept VR project for Dixie State University. The project was meant to prove how VR could be incorporated into DSU classes. The entire project was created in unity, meaning the scripts were written in C#. Since the whole project isn't mine, I posted some of my code here as well as videos showcasing most everything that was implemented.

## Summary
VR is a great environment for teaching and learning almost anything. This project was built to prove how VR could be used in an electrical engineering class to teach students using visual and interactable models. MRTK was used as a basis for the project with Normcore to cover the network side. All code had to be written to support both MRTK and Normcore.

### PN Snap 2
This code detects the distance between to objects via a vector drawn between them. When the vector is within a given size, one object is moved to a "snap" position. Another version of this was written using colliders rather than a vector length, however, MRTK and Normcore don't work well together with colliders and moving objects. Positions are synched over Normcore.

### Reset Positon
When placed on an object, this code records the object's initial position and that of all its children. Upon calling the ResetPosition() function, the object and all of its children are placed back into their initial positions. Since Normcore needs a frame to update data, the ResetPosition() function only sets a boolean that is checked every frame within the Update() function. This allows Normcore time to synchronize the objects' positions.

### Reset Position Sync
A single sample of a "custom component" that had to be written to synchronize variables over Normcore.

(See Showcase Videos folder for videos on models implemented)
