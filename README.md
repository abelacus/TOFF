# qBittorrent Orphan File Finder
A python script for finding files in a directory which aren't associated with a torrent

## Why?
Sonarr and Radarr don't seem to delete files when removing a torrent from the client in some cases, and as such can leave a lot of large files that build up over time taking up storage space.
Alternatively, maybe there's been instances you've removed a torrent from the client without deleting data and you no longer remember its name.
This script enables you to identify these files for removal. 

## Assumptions
- All files within the target directory were, at some point, downloaded as part of a torrent.
- Your qBittorrent instance has the WebUI enabled and username + password set up.
- Only one directory needs to be searched for files.

## Prerequisites
- Set the `qb_base_url`, `username`, `password`, and `torrent_directory` variables to match your instance and desired search directory.
- Configure any `path_translations` in the format of `'client path':'system path'`. Useful for when the torrent client is running in a docker container and your defined volumes don't exactly match the systems.
- Set optional `ignore_directories`. These are directories of downloaded files you don't want to check as they appear in the torrent client (Not necessary, just reduces the number of information requests made to the torrent client).