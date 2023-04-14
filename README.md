# HowMuchLeft.App
This program allows you to track your working time and automatically calculate when you can call it a day.
 
 ## How it works
The program starts by specifying the start time of work and the duration of working time. Breaks can be taken during the working time, which will be deducted from the working time. The program automatically calculates when the working time ends and gives an estimate of when you can call it a day.

## Usage
The program can be started from the command line and takes the following arguments:

```bash
-s <starttime>: The start time of the working time in the format HH:mm.
-b <breakstart1>,<breakend1>,<breakstart2>,<breakend2>,...: The breaks during the working time as a list of start and stop times in the format HH:mm,HH:mm,HH:mm,HH:mm,...
-w <worktime>: The expected working time e.g. '8.0'.
```

If no arguments are passed, the program will look for the required arguments in the appsettings.
