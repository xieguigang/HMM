@echo off

REM git remote add gitlink https://gitlink.org.cn/xieguigang/HMM.git

git pull gitlink HEAD:master
git push gitlink

git pull origin
git push origin