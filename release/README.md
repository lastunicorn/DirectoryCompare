# Directory Compare - Release Steps

## 0) Verify the version

Verify that the correct version number is specified in the file:

- `{repo}/Directory.Build.props`

## 1) Build

Build in Release mode

## 2) Deploy Files

Copy the result of the build from

-  `{repo}/DirectoryCompare.Cli.Bootstrapper/bin/Release/net6.0/`

to

- `~/Programs/Directory Compare/`

## 3) Create link

Create a link file to easily launching the application:

- `~/bin/dircmp`

Target file of the link:

- `~/Programs/Directory Compare/dircmp`

## 4) Increase the version

Increase the version number in the file:

- `{repo}/Directory.Build.props`