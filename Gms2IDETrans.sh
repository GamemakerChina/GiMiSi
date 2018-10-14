#!/bin/bash
chmod 777 ./Gms2IDETrans.sh
#获取用户名
username=`who am i | awk '{print $1}'`
# 版本变量
standVer=0
# 判断下载版
if [[ -d "/Applications/GameMaker Studio 2.app/" ]]
then
	let "standVer+=1"
fi
# 判断Steam版
cd "/Users/${username}/Library/Application Support/"
if [[ -d "./Steam/" ]]
then
	if [[ -d "./Steam/steamapps/common/GameMaker Studio 2 Desktop/" ]]
	then
		let "standVer+=2"
	fi
fi

downloadStand(){
	echo
	echo "------------------------------------------------"
	echo "正在下载安装版IDE汉化文件..."
	cd "/Applications/GameMaker Studio 2.app/Contents/MonoBundle/Languages"
	echo "写入安装版IDE汉化文件需要提升权限，请输入密码"
	sudo curl -O "https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/macos/chinese.csv"
	echo "安装版IDE汉化文件下载完成"
	echo "------------------------------------------------"
}

downloadSteam(){
	echo
	echo "------------------------------------------------"
	echo "正在下载Steam版IDE汉化文件..."
	cd "/Users/${username}/Library/Application Support/Steam/steamapps/common/GameMaker Studio 2 Desktop/GameMaker Studio 2.app/Contents/MonoBundle/Languages"
	curl -O "https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/macos/chinese.csv"
	echo "Steam版IDE汉化文件下载完成"
	echo "------------------------------------------------"
}

case $standVer in
	0) 
	echo "未检索到GMS2，脚本退出"
	;;
	1) 
	echo "检索到安装版"
	downloadStand
	echo "安装完成，脚本退出"
	;;
	2) 
	echo "检索到Steam版"
	downloadSteam
	echo "安装完成，脚本退出"
	;;
	*) 
	echo "检索到安装版和Steam版"
	downloadStand
	downloadSteam
	echo "安装完成，脚本退出"
	;;
esac

exit