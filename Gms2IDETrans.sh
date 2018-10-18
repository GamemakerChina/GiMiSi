#!/bin/bash
# 分隔符
separator="----------------------------------------------------"
# Title
echo $separator
echo "欢迎使用 GameMaker Studio 2 IDE 汉化脚本 by tpxxn！"
echo $separator

# 获取用户名
username=`who am i | awk '{print $1}'`
# 版本变量
# 安装版
standVer=0
# Steam版
steamVer=0
# 判断下载版
if [[ -d "/Applications/GameMaker Studio 2.app/" ]]
then
	standVer=1
fi
# 判断Steam版
cd "/Users/${username}/Library/Application Support/"
if [[ -d "./Steam/" ]]
then
	if [[ -d "./Steam/steamapps/common/GameMaker Studio 2 Desktop/" ]]
	then
		steamVer=1
	fi
fi

downloadStand(){
	echo $separator
	echo "正在下载安装版IDE汉化文件..."
	cd "/Applications/GameMaker Studio 2.app/Contents/MonoBundle/Languages"
	sudo curl -O "https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/macos/chinese.csv"
	if [[ $? == 0 ]] 
	then
		echo "安装版IDE汉化文件下载完成"
		return 0
	else
		echo
		return 1
	fi
}

steamResult=0
downloadSteam(){
	echo "正在下载Steam版IDE汉化文件..."
	cd "/Users/${username}/Library/Application Support/Steam/steamapps/common/GameMaker Studio 2 Desktop/GameMaker Studio 2.app/Contents/MonoBundle/Languages"
	curl -O "https://raw.githubusercontent.com/GamemakerChina/gms2translation/gh-pages/macos/chinese.csv"
	if [[ $? == 0 ]] 
	then
		echo "Steam版IDE汉化文件下载完成"
		return 0
	else
		echo
		return 1
	fi
}

if [[ $standVer==1 ]]
then
	while :
	do
		echo
		echo $separator
		echo "检索到安装版"
		echo "输入 1 下载 IDE 汉化文件，输入 2 跳过"
		read standJmp
		if [[ "$standJmp" == "1" ]]
		then
			downloadStand
			if [[ $? != 0 ]] 
			then
				echo "下载失败，是否重新下载？输入 1 重新下载，输入 2 跳过"
				read standRedownload
				if [[ "$standRedownload" == "1" ]]
				then
					echo $separator
					continue
				elif [[ "$standRedownload" == "2" ]]
				then
					echo "跳过下载"
					echo $separator
					break
				else
					echo "输入指令有误！"
				fi
			else
				echo $separator
				break
			fi
		elif [[ "$standJmp" == "2" ]] 
		then
			echo "跳过下载"
			echo $separator
			break
		else
			echo "输入指令有误，请重新输入！"
		fi
	done
fi

if [[ $steamVer==1 ]]
then
	while :
	do
		echo
		echo $separator
		echo "检索到Steam版"
		echo "输入 1 下载 IDE 汉化文件，输入 2 跳过"
		read steamJmp
		if [[ "$steamJmp" == "1" ]]
		then
			downloadSteam
			if [[ $? != 0 ]] 
			then
				echo "下载失败，是否重新下载？输入 1 重新下载，输入 2 跳过"
				read steamRedownload
				if [[ "$steamRedownload" == "1" ]]
				then
					echo $separator
					continue
				elif [[ "$steamRedownload" == "2" ]]
				then
					echo "跳过下载"
					break
				else
					echo "输入指令有误！"
				fi
			else
				echo $separator
				break
			fi
		elif [[ "$steamJmp" == "2" ]] 
		then
			echo "跳过下载"
			echo $separator
			break
		else
			echo "输入指令有误，请重新输入！"
		fi
	done
fi

echo
echo "脚本结束！"

exit