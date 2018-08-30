#!/bin/bash

echo "Begin deployment execution"

if [ "$TRAVIS_TAG" != "" ]; then
    echo "Tags detected '$TRAVIS_TAG' ... running deployment"

    echo "Running deployment"

    tagarray=$(echo $TRAVIS_TAG | tr "-" "\n")

    count=0
    postfix=""
    for prefix in $tagarray
    do
        if [ "$count" == "0" ]; then
            assemblyversion=$prefix
        else
            if [ "$postfix" == "" ]; then
                postfix="$prefix"
            else
                postfix="$postfix-$prefix"
            fi
        fi
        count=$count+1
    done

    echo "Assembly Version: $assemblyversion"
    echo "Version Suffix: $postfix"


    # Now find all of the directories with a nuspec...
    NugetProjectFolders=$(find `pwd` -regex .*.*\.nuspec | grep -v '/obj/' | grep -v '/bin/')
    for nuspec in $NugetProjectFolders
    do
        projdir=$(dirname $nuspec)

        echo "Packing project $projdir"

        dotnet pack --version-suffix "'$postfix'" /p:version=$assemblyversion $projdir
        if [ $? -ne 0 ]; then
            exit 1
        fi
    done

    # Now find all of the nuget packages.
    NugetPackages=$(find `pwd` -regex .*Sulucz\..*.*\.nupkg | grep -v '/obj/')
    for package in $NugetPackages
    do
        echo "Pushing package $package"
        
        dotnet nuget push --api-key "$NUGET_API_KEY" --source 'https://www.nuget.org' $package
        if [ $? -ne 0 ]; then
            exit 1
        fi
    done
fi