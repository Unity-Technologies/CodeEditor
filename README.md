# CodeEditor

An embedded text editor for [Unity](http://unity3d.com/).

## Installing

CodeEditor is developed using the new [kaizen dependency management framework](http://github.com/Unity-Technologies/kaizen) for Unity extensions:
* import [kaizen.unityPackage](https://github.com/downloads/Unity-Technologies/kaizen/kaizen.unityPackage) into your project
* open a command line shell (proper Unity GUI to come soon, maybe you want to help?) into the newly created _Assets/kaizen_ directory
* ./gradlew update

## Hacking

    git clone it
    cd CodeEditor
    ./gradlew update
    ./gradlew test
    
### Updating a local Unity project with the latest CodeEditor libraries

    ./gradlew updateUnityProject

The location of the unity project can be set through the _unityProjectDir_ gradle property which by default is assumed to be in _../UnityProject_

The property can be set through the _-P_ command line argument:

    ./gradlew -PunityProjectDir=/path/to/my/unity/project updateUnityProject
    
or more permanently through a _gradle.properties_ file.

For more information on setting gradle project properties check the [gradle tutorial](http://www.gradle.org/docs/current/userguide/tutorial_this_and_that.html).

## Contributors

See: https://github.com/Unity-Technologies/CodeEditor/graphs/contributors