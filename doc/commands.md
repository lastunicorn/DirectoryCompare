# Calculate hashes (read)

## Description

Calculates hashes for all the files from the specified <source-path> and all its children.

## Signature

```
read-disk <source-path> <destination-json-file> <blacklist-file>
```

<source-path>

- Mandatory
- Possible values: path of a directory on disk

<destination-json-file>

- Mandatory
- Possible values: path of a file on disk
- In this file will be written the resulted hashes.
- If the file already exists on disk, it will be overwritten.

<blacklist-file>

- Optional
- Possible values: path of a file on disk
- A file that contains a list of paths that should be ignored.

## Example

Windows:

```
read-disk d:\Pictures pictures.json
```

Linux:

```
read-disk /media/Totoro/Pictures pictures.json
```

# Display a hash file (view)

## Description

Opens the specified hashes file and displays its content.

## Signature

```
read-file <json-file>
```

## Example

Windows:

```
read-file d:\disk-hashes\pictures.json
```

Linux:

```
read-file /home/user/disk-hashes/pictures.json
```

# Compare disk with hash file? (check)

## Description

Compares the specified <disk-path> with the data stored in the <json-file> and displays the differences.

## Signature

```
verify-disk <disk-path> <json-file>
```

## Example

Windows:

```
verify-disk d:\Pictures pictures.json
```

Linux:

```
verify-disk /media/Totoro/Pictures pictures.json
```

# Compare two paths (compare)

## Description

Compares two paths and displays the difference.

## Signature

```
compare-disks <path1> <path2>
```

# Compare the files from two hash files (compare)

## Description

Compares the content of two hash files.

## Signature

```
compare-files <json-file-1> <json-file-2> <results-directory>
```

<json-file-1>

- Mandatory
- Possible values: path of a file on disk

<json-file-2>

- Mandatory
- Possible values: path of a file on disk

<results-directory>

- Optional
- Possible values: path of a directory on disk
- Default value: null
- This is the parent directory in which a “results” directory is created.
- If this parameter is not provided, the results are displayed in the console.

# Display duplicate files (find-duplicates)

## Description

Compares the content of two hash files and displays the duplicate files.

## Signature

```
find-duplicates <json-file-left> <json-file-right> <check-if-files-exists>
```

<json-file-left>

- Mandatory
- Possible values: path of a file on disk

<json-file-right>

- Optional  
- Possible values: path of a file on disk
- Default value: null
- If the hashes right file is not provided, the algorithm searches for duplicates only in the left hashes file.

<check-if-files-exists>

- Optional
- Possible values: “true” or “false”
- Default value: “false”
- If it is provided, considers two files with the same hash to be duplicates only if they still exist on the disk.

# Removes the duplicate files (remove-duplicates)

## Description

Compares the content of two hash files and displays the duplicate files.

## Signature

```
remove-duplicates <json-file-left> <json-file-right> <file-remove>
```

<json-file-left>

- Mandatory
- Possible values: path of a file on disk

<json-file-right>

- Optional
- Possible values: path of a file on disk
- Default value: null
- If the hashes right file is not provided, the algorithm searches for duplicates only in the left hashes file.

<file-remove>

- Optional
- Possible values: “Left” or “Right”
- Default value: “Right”
- Specifies the part from where to remove the found duplicate files.

