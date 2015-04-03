# Controller and simulator for a robot with an MD49 motor driver (RobX Project)

This project is a combination of several smaller projects that are aimed for hardware connection, control over network and simulation of a differential drive robot with *Devantech*'s MD49 motor driver. 

The *RobX project* is completely written in C#.Net (Microsoft .Net Framework 4.0) and consists of the following smaller projects:

1. **RobX Interface:** a hardware interface and a TCP server that can connect to the MD49 motor driver and transfers the commands received from clients to the robot motor for hardware execution. *It needs RobX Library as a dependency.*

1. **RobX Simulator:** a simulator and a TCP server that (almost) exactly mimics the behaviour of the real RobX robot. It can interpret and simulate all the MD49 motor driver commands. *It needs RobX Library and Microsoft XNA Game Studio 4.0 as the dependency.*

1. **RobX Controller:** a client that can control the robot (real or simulated) over the network. It provides many useful functions for robot control on low and high levels. *It needs RobX Library as a dependency.*

1. **RobX Library:** a dynamic link library (dll) that provides common classes for all the above subprojects. 


## RobX robot

The project is written for a robot called *RobX*. The robot uses MD49 motor driver and EMG49 motor kit. 

*RobX* is designed and implemented in September 2012 by Sharif University of Technology and Shahid Beheshti University students for the purpose of participating in the Service Robots league of the 4th National Khwarizmi Robotics Competitions and the 3rd International Amir-Kabir University Robotics Competitions (AUTCup 2012).

Currently RobX is located at the Image Processing and Computer Vision Lab of the Computer Engineering Department of Sharif University of Technology.

Works done with RobX include:

* MSc thesis by Kourosh Sartipi: "Indoor Office Environment Mapping using a Mobile Robot" under supervision of Dr. Mansour Jamzad, Sharif University of Technology.

* 1st Place in the Service Robots league of the 4th National Khwarizmi Robotics Competitions and the 3rd International Amir-Kabir University Robotics Competitions (AUTCup 2012).

* 2nd Place in the 4th International Amir-Kabir University Robotics Competitions (AUTCup 2013). 


## Credentials

*Developer:* Azarakhsh Keipour ( keipour @gmail.com )

*Special thanks to:* Mohammad Reza Mousaei ( m.mousaei @gmail.com )


## Copyright

*This software is developed at Image Processing and Computer Vision Laboratory, Computer Engineering Department, Sharif University of Technology.*

The project is released under MIT license, which means you can do (almost) everything with the code.

------------

Copyright (c) 2015 Azarakhsh Keipour (keipour @gmail.com)
