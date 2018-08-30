#!/bin/bash
projects=$(find `pwd` -regex .*\.csproj$)
echo "Compiling projects:"
echo "$projects"

echo "Restoring projects"
for project in $projects 
do
    echo "Restore for $project"
    dotnet restore $project
    
    if [ $? -ne 0 ]; then
        exit $?
    fi
done

echo "Building projects: "
for project in $projects 
do
    echo "Build for $project"
    dotnet build $project /warnaserror
    
    if [ $? -ne 0 ]; then
        exit 1
    fi
done

testprojects=$(find `pwd` -regex .*\.Tests\..*\.csproj$)
echo "Runnings Tests:"
for project in $testprojects 
do
    echo "Build for $project"
    dotnet test $project
    
    if [ $? -ne 0 ]; then
        exit 1
    fi
done