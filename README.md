<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/OminousLama/restic-restore-everything">
    <img src="assets/logo.svg" alt="Logo" width="80" height="80">
  </a>

  <h3 align="center">Restic Restore Everything</h3>

  <p align="center">
    CLI to restore everything that was ever checked into a Restic repo.
    <br />
    <br />
    <a href="assets/demo.mp4">View Demo</a>
    ·
    <a href="https://github.com/OminousLama/restic-restore-everything/issues/new?assignees=&labels=&projects=&template=bug_report.md&title=">Report Bug</a>
    ·
    <a href="https://github.com/OminousLama/restic-restore-everything/issues/new?assignees=&labels=&projects=&template=feature_request.md&title=">Request Feature</a>
  </p>
</div>


<!-- ABOUT THE PROJECT -->
## About The Project

https://github.com/user-attachments/assets/cf3998e2-6805-4e8e-9282-2a5f5516ee20

> THIS IS NOT A LOST DATA RECOVERY PROGRAM! It can not restore files from restic that restic has forgotten or purged!

Restic Restore Everything is a Command-Line Interface tool designed to restore every file that has ever been checked into a [Restic](https://restic.net/) repository - **and that is remembered**. This is useful for repo that are used as a fire-and-forget repositories; i.e. repositories that periodically receive files which are removed from the source immediately after completed backup and a means to save space.

More concrete example: Let's say I have a server that has about 1 TB of disk space available. After some time, the avaialble disk space runs out. So I set up an external storage bucket over at AWS or Backblaze in which I init a new restic repository. I'll then setup some script on my server that periodically backs up files that are no longer needed on the server to this bucket repository (like old documents, images, etc). After the backup completes, the files are deleted from the server to free up space, but live on long-term in the bucket repository.

Restic as of August 2024 does not offer an easy to use way of restoring all data that is stored this way. It is a very specific usecase, but from what I could find in the forums, it is not unique to me. 


### Limitations

This script can only restore files that restic remembers. It **CAN NOT** magically restore purged or forgotten data.

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

Before you begin using Restic Restore Everything, you will need:

- Restic (v0.16 or newer) installed on your machine
- Access to the restic repository you wish to restore from
- Sufficient storage space for the restored files


### Installation

1. Goto release page and download the [latest release](https://github.com/OminousLama/restic-restore-everything/releases/latest)
2. Extract the `rrev` binary to a directory of your choice
3. Give it exec permissions (e.g. `chmod 700 rrev`)
4. Run the program (e.g. ./rrev --help)


<!-- USAGE EXAMPLES -->
## Usage

You can view all options by providing the `--help` option.


### Example usage

1. First, you'll have to generate a **findings** file. That file contains every file that your restic repository remembers. You can create the file by using the following command:

```bash
restic -r YOUR_REPO_URL find "*" > findings
```

This will generate a file named `findings`.

2. Now you can go ahead and run the restic restore everything app:

> ⚠️ Only latest versions are restored: This app will only restore the **newest version of a file!** This means, if you have backed up a file called "Test", deleted this file and later added another file also called "Test" in the same location, this app will only restore the last "Test" file.
> ⚠️ Check available disk space: Please check if the available disk space is sufficient to restore the entire repo first! 

```bash
./rrev -r YOUR_REPO_URL -s REPOSITORY_PASSWORD -t DIRECTORY_WHERE_RESTORED_FILES_GO_TO -f PATH_TO_FINDINGS_FILE
```

This command will attempt to restore every file in the findings file to the target directory.

3. Wait for the program to finish. Keep an eye on the log output in case it failed to restore some files.

4. All done!


<!-- ACKNOWLEDGMENTS -->
## Acknowledgments

* [Remix Icon](https://remixicon.com/)
* [Best README Template](https://github.com/othneildrew/Best-README-Template)
