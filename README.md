# Deletion
A simple command line based project to delete files from a given directory by either searching for a given extension, or substring\
This program will not delete any folders\
A user may optionally use -r to search recursively so as to include subdirectories\
A user may optionally use -t to check what files are flagged for deletion, without actually deleting any files\
To use:\
"dotnet run -- [-s][-e] [-r?] [-t?] < path > < Extension/SubString >"
# IMPORTANT
There is no warning, nor confirmation message given when deleting files. Be very careful when deleting files
