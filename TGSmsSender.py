# -*- coding: utf-8 -*-
from telegram.ext import (Updater, CommandHandler, MessageHandler, RegexHandler)
from telegram import (ReplyKeyboardMarkup, ReplyKeyboardRemove)
import pymysql.cursors, time, datetime, json, requests

#####Параметры MySQL######
HOST = "HOST"            #
USER = "USER"            #
PASSWORD = "PASSWORD"    #
DB = "DB"                #
##########################

BeautifulNumbers = {
	"1": "1️⃣",
	"2": "2️⃣",
	"3": "3️⃣",
	"4": "4️⃣",
	"5": "5️⃣",
}

#Проверка словаря админов
def CheckAdmin(d, value):
    for i in range(len(d)):
    	if int((d[i]["AdminVK"])) == value:
            return True
    return False

#Функция для взаимодействия с одним элементом БД
def MySQLFetchOne(SQLString):
	
	connection = pymysql.connect(host=HOST,user=USER,password=PASSWORD,db=DB,cursorclass=pymysql.cursors.DictCursor)

	try:

	    with connection.cursor() as cursor:
	        cursor.execute(SQLString)
	        result = cursor.fetchone()
	finally:
	    connection.close()
	return result;

#Функция для взаимодействия со всеми элементами БД
def MySQLFetchAll(SQLString):
	
	connection = pymysql.connect(host=HOST,user=USER,password=PASSWORD,db=DB,cursorclass=pymysql.cursors.DictCursor)
	try:
	    with connection.cursor() as cursor:
	        cursor.execute(SQLString)
	        result = cursor.fetchall()
	finally:
	    connection.close()
	return result;


#Основная конфигурация
TGUpdater = Updater(token=MySQLFetchOne("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='TelegramAPIKey'")["ServiceKey"])
TGAdmins = (MySQLFetchAll("SELECT AdminTG FROM TGAdmins"))

SMSLogin = MySQLFetchOne("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroLogin'")["ServiceKey"]
SMSAPIKey = MySQLFetchOne("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroAPIKey'")["ServiceKey"]
SMSAuth = SMSLogin+":"+SMSAPIKey
all_url = "https://"+SMSAuth+"@gate.smsaero.ru/v2"

GENDER, PHOTO, LOCATION, BIO = range(4)
#Процедуры для обработок команд бота
def StartHandler(bot, update):
	bot.send_message(chat_id=update.message.chat_id, text="Привет, список команд бота:\n/sms - отправка сообщения\n/balance - получаение текущего баланса\n/history - 5 последних отправленных смс\n/users - управление пользователями")

def SMSHandler(bot, update):
	reply_keyboard = [["Да", "Нет"]]
	try:
		SMSNumber = update.message.text.split(" ")[1]
		SMSMessage = update.message.text.split("\"")[1]
		bot.send_message(chat_id=update.message.chat_id, text="Вы действительно хотите отправить сообщение \""+SMSMessage+"\" на номер "+SMSNumber+"?",reply_markup=ReplyKeyboardMarkup(reply_keyboard, one_time_keyboard=True))
	except:
		bot.send_message(chat_id=update.message.chat_id, text="Синтаксис использования команды:\n/sms номер_телефона \"Сообщение\"",reply_markup=ReplyKeyboardRemove())

def AcceptHandler(bot, update):
	sms = requests.get(all_url+"/sms/send?number="+SMSNumber+"&text="+SMSMessage+"&sign=SMS Aero&channel=DIRECT").json()
	if sms["success"]==True:
		SMSBalance = str(requests.get(all_url+"/balance").json()["data"]["balance"])
		bot.send_message(chat_id=update.message.chat_id, text="Сообщение успешно отправлено!\nТекущий баланс: "+SMSBalance+"₽")
	else:
		bot.send_message(chat_id=update.message.chat_id, text="Что-то пошло не так при отправке сообщения 😔\nПовторно воспользуйтесь командой /sms")
	SMSNumber = None;
	SMSMessage = None;

def BalanceHandler(bot, update):
	balance = str(requests.get(all_url+"/balance").json()["data"]["balance"])
	bot.send_message(chat_id=update.message.chat_id, text="Текущий баланс: "+balance+"₽")

def HistroyHandler(bot,update):
	history = requests.get(all_url+"/sms/list").json()["data"]
	OutMessage = ""
	for i in range(5):
		OutMessage += BeautifulNumbers[str(i+1)]+" +"+str(history[str(i)]["number"])+" \""+history[str(i)]["text"]+"\"\n\n"			
	bot.send_message(chat_id=update.message.chat_id, text="Последние 5 отправленных сообщений:\n"+OutMessage)

def MainUsersHandler(bot,update):
	UsersKeyboard = [["Добавление", "Удаление"]]
	bot.send_message(chat_id=update.message.chat_id, text="Управление пользователями\nКакое действие с пользователем вы хотите осуществить?",reply_markup=ReplyKeyboardMarkup(UsersKeyboard, one_time_keyboard=True))

def AddUserHandler(bot,update):
	update.message.reply_text('Bye! I hope we can talk again some day.',reply_markup=ReplyKeyboardRemove())

def RemoveUserHandler(bot,update):
	update.message.reply_text("MOEW")

dispatcher = TGUpdater.dispatcher
dispatcher.add_handler(CommandHandler('start', StartHandler))
dispatcher.add_handler(CommandHandler('sms', SMSHandler))
dispatcher.add_handler(CommandHandler('balance', BalanceHandler))
dispatcher.add_handler(CommandHandler('users', MainUsersHandler))
dispatcher.add_handler(CommandHandler('history', HistroyHandler))
dispatcher.add_handler(RegexHandler('^(Да)$',AcceptHandler))
dispatcher.add_handler(RegexHandler('^(Добавление)$',AddUserHandler))
dispatcher.add_handler(RegexHandler('^(Удаление)$',RemoveUserHandler))
TGUpdater.start_polling()