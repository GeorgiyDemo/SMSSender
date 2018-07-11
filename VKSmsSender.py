# -*- coding: utf-8 -*-
from pytils import numeral
import pymysql.cursors
import string, vk, time, datetime, json, requests, urllib3, dateutil.parser

#####Параметры MySQL######
HOST = "HOST"            #
USER = "USER"            #
PASSWORD = "PASSWORD"    #
DB = "DB"                #
##########################

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
VKSession = vk.Session(access_token=MySQLFetchOne("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='VKAPIKey'")["ServiceKey"])
VKAdmins = (MySQLFetchAll("SELECT AdminVK FROM VKAdmins"))

SMSLogin = MySQLFetchOne("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroLogin'")["ServiceKey"]
SMSAPIKey = MySQLFetchOne("SELECT ServiceKey FROM ServiceTable WHERE ServiceName='SMSAeroAPIKey'")["ServiceKey"]
SMSAuth = SMSLogin+":"+SMSAPIKey

api = vk.API(VKSession)
APIVersion = 5.73
message_longpoll = [0]

#Настройка лонгпула
server = None
key    = None
ts     = None

KEYBOARD = {
	"one_time":True,
	"buttons":[
		[
			{
			"action":{
				"type":"text",
				"payload":"{\"button\": \"1\"}",
				"label":"Да"
			},
			"color":"primary"
			},

			{
			"action":{
				"type":"text",
				"payload":"{\"button\": \"2\"}",
				"label":"Нет"
			},
			"color":"negative"
			},

		]
	]
}

while True:

	#Фикс лонпула по харду
	if server == None:
		cfg = api.messages.getLongPollServer(v=APIVersion)
		server = cfg['server']
		key = cfg['key']
		ts = cfg['ts']

	response = requests.post(
		"https://{server}?act=a_check&key={key}&ts={ts}&wait=25&mode={mode}&version=2".format(**{
			"server": server,
            "key": key,
            "ts": ts,
            "mode": 2
        }),
    timeout=30
   	).json()

	checker = False
	for i in range(len(response['updates'])):
		if checker != True:
			try:
				
				message_longpoll = response['updates'][i][5]
				chat_longpoll = response['updates'][i][3]
				checker = True

			except:
				pass
	if checker == False:
		message_longpoll = [0]
		chat_longpoll = [0]

	ts = response['ts']

    #Чекаем входящие сообщения
	if message_longpoll != [0]:
		if (CheckAdmin(VKAdmins,chat_longpoll) == True):
			all_url = "https://"+SMSAuth+"@gate.smsaero.ru/v2"

			if message_longpoll =='/balance':

				balance = str(requests.get(all_url+"/balance").json()["data"]["balance"])
				api.messages.send(user_id=chat_longpoll,message="Текущий баланс: "+balance+"₽",v=APIVersion)
			
			elif message_longpoll.split(" ")[0] == "/sms":
				try:
					SMSNumber = message_longpoll.split(" ")[1]
					SMSMessage = message_longpoll.split("&quot;")[1]
					api.messages.send(user_id=chat_longpoll,message="Вы действительно хотите отправить сообщение \""+SMSMessage+"\" на номер "+SMSNumber+"?",keyboard=json.dumps(KEYBOARD,ensure_ascii=False),v=APIVersion)
				except:
					api.messages.send(user_id=chat_longpoll,message="Синтаксис использования команды:\n/sms номер_телефона \"Сообщение\"",v=APIVersion)

			elif message_longpoll == "Да":

				sms = requests.get(all_url+"/sms/send?number="+SMSNumber+"&text="+SMSMessage+"&sign=SMS Aero&channel=DIRECT").json()
				if sms["success"]==True:
					SMSBalance = str(requests.get(all_url+"/balance").json()["data"]["balance"])
					api.messages.send(user_id=chat_longpoll,message="Сообщение успешно отправлено!\nТекущий баланс: "+SMSBalance+"₽",v=APIVersion)
				else:
					api.messages.send(user_id=chat_longpoll,message="",v=APIVersion)
				SMSNumber = None;
				SMSMessage = None;