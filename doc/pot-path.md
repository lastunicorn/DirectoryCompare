Last snapshot
--------------------------------------------------------------------------------

**Template**

Write just the name of the pot.

```
potname
```

**Example**

For a pot that is called "books":

	dircmp find-duplicates books

A specific sub-path from the last snapshot
--------------------------------------------------------------------------------

**Template**

The `>` character is used to specify the full path after the pot name.

```
potname>directorypath
```

**Example**

	dircmp find-duplicates books>/comicbooks/warcraft
	dircmp find-duplicates books>c:\comicbooks\warcraft

A previous snapshot - by index
--------------------------------------------------------------------------------

**Template**

Use `~` character to specify the index of the snapshot after the pot name.

The index is zero based:

- Index 0 is the last snapshot. 
- Index 1 is the snapshot before the last one.
- Index 2 is the second snapshot before the last one.
- And so on.

```
potname~index
```

**Example**

The following path specifies the `/comicbooks/warcraft` directory from the snapshot created before the last one.

	dircmp find-duplicates books~1>/comicbooks/warcraft
	dircmp find-duplicates books~1>c:\comicbooks\warcraft

A previous snapshot - by date
--------------------------------------------------------------------------------

**Template**

Use `~` character to specify the date of the snapshot after the pot name.

The date format is: `yyyy-MM-dd-HHmm`

- `yyyy` - The year
- `MM` - Two digit month number starting with 01 that is January.
  - Note: If the month number is one digit, a 0 is added in front. Ex: 03
- `dd` - Two digit day of month.
- `HH` - Two digit hour in 24h format. Possible values: [00-23]
- `mm` - Two digit minute of the hour. Possible values: [00-59]

```
potname~date
```

**Example**

The following path specifies the `/comicbooks/warcraft` directory from the snapshot created in date 2020-01-03-1340.

	dircmp find-duplicates books~2020-01-03-1340>/comicbooks/warcraft
	dircmp find-duplicates books~2020-01-03-1340>c:\comicbooks\warcraft

A previous snapshot - by GUID (not yet implemented)
--------------------------------------------------------------------------------

**Template**

Use `~` character to specify the GUID of the snapshot after the pot name.

```
potname~guid
```

**Example**

	dircmp find-duplicates books~c09989f5>/comicbooks/warcraft
	dircmp find-duplicates books~c09989f5>c:\comicbooks\warcraft
	dircmp find-duplicates books~c09989f5-2a65-4ecf-bca9-e7ceaf230b4f>c:\comicbooks\warcraft