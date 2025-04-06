import os
import requests

#base url and login details
qb_base_url = "http://ip.address:port"
username = "username"
password = "password"
torrent_directory = "/mnt/drive/Downloads" #the system directory to traverse for files

ignore_directories = ['/data/downloads'] 	#add directories you want to skip searching over here. e.g. if you know there are no orphan files in /data/downloads skip requests for torrent details which are saved to that directory
											# theses are directories as seen by the torrent client and before path translation

path_translations = { 		#useful for when the client is running in a docker container and paths don't relate exactly to the host disk layout
	"/data": "/mnt/drive" 	#e.g. will replace /data in paths provided by torrent client with /mnt/disk
}							#broadly relates to the volumes section in the torrent docker-compose file

#log in
session = requests.Session()
session.post(qb_base_url + '/api/v2/auth/login', data={'username': username, 'password': password})

#get torrent list
hash_response = session.get(qb_base_url + '/api/v2/torrents/info')
hash_list = hash_response.json()

active_files = set()

#uses path translations as defined above
def translate_path(path):
	for old_path, new_path in path_translations.items():
		if(path.startswith(old_path)):
			path = path.replace(old_path, new_path, 1)

	return path

#get list of files for each torrent, only include ones set to download in list
for torrent in hash_list:
	current_dir = torrent['save_path']

	if(current_dir in ignore_directories):
		continue

	file_list = session.get(qb_base_url + f"/api/v2/torrents/files?hash={torrent['hash']}").json()

	#check priority and only add to list of not 0
	for file in file_list:
		if file['priority'] != 0:
			#translate paths
			file_path = current_dir + '/' + file['name']
			path_translated = translate_path(file_path)
			active_files.add(path_translated)

with open('all_torrent_files.txt', 'w', encoding='utf-8') as f:
	for item in active_files:
		f.write(item + '\n')

#walk through all files in torrent directory
all_files_in_directory = set()
for root, dirs, files in os.walk(torrent_directory):
	for file in files:
		all_files_in_directory.add(root + '/' + file)

#remove all that are 
potential_orphans = all_files_in_directory - active_files

#write potential orphans to file
with open('potentially_orphaned.txt', 'w', encoding='utf-8') as f:
	for item in potential_orphans:
		f.write(item + '\n')

