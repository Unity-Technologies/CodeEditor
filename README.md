# CodeEditor

An embedded text editor for [Unity](http://unity3d.com/).

## Hacking

Join the [conversation](https://groups.google.com/forum/?fromgroups#!forum/codeeditor).

    git clone it
    cd CodeEditor
    ./gradlew assemble vs test
    ./gradlew open
    
You might need to set the _unityDir_ property for the build to work (see how to set properties below).
    
### Updating a local Unity project with the latest CodeEditor libraries

    ./gradlew updateUnityProject

The location of the unity project can be set through the _unityProjectDir_ gradle property which by default is assumed to be in _../UnityProject_

The property can be set through the _-P_ command line argument:

    ./gradlew -PunityProjectDir=/path/to/my/unity/project updateUnityProject
    
or more permanently through a _gradle.properties_ file.

For more information on setting gradle project properties check the [gradle tutorial](http://www.gradle.org/docs/current/userguide/tutorial_this_and_that.html).

## Contributors

See: https://github.com/Unity-Technologies/CodeEditor/graphs/contributors
