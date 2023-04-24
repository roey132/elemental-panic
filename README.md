# elemental-panic

## cloning repo:
to clone the repository, use git command:

```
git clone https://github.com/roey132/elemental-panic
```

after cloning project, open the project using unity hub, then, inside the project open a scene


## working on branches
when working on the project, you need to work on branches.
* `MAKE SURE NOT TO WORK ON MASTER DIRECTLY`

create a branch using git command:
```
git branch branch_name
git checkout branch_name
```

* try naming the branch to something that makes sense, and connected to what you are working on, 
* for example: if you work on a down attack for a character, branch name could be "down_attack"

## commiting and pushing to repo:
when working, try saving your files often using 
```
git add -A
```
to save what you made in the repo, you need to use git commit :
```
git commit -m "commit message"
```
* in the commit message, write something that explains what was created, fixed, deleted.
* try to use commits often, so commits wouldn't be too big and hard to manage

after commiting your changes, you need to push the changes into the branch:
```
git push
```

## merging into master
after you finish working on a branch, you need to create a pull request
use the github UI to create a pull request from your branch into main

`do not approve the pull request, just make it`


## general notes
* Make sure you commit your changes often, with meaningful messages
* Make sure to work on your own branch, you can use `git status` to see what branch you work on
* If you have any doubts about what you need to do, ask before doing it
* Its fine to make mistakes since we are all beginners here, so remember that if you mess stuff up, ask for help.
