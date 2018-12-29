-- phpMyAdmin SQL Dump
-- version 4.6.6deb5
-- https://www.phpmyadmin.net/
--
-- Хост: localhost:3306
-- Время создания: Дек 29 2018 г., 16:03
-- Версия сервера: 5.7.24-0ubuntu0.18.04.1
-- Версия PHP: 7.2.10-0ubuntu0.18.04.1

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- База данных: `SMSSender_schema`
--

-- --------------------------------------------------------

--
-- Структура таблицы `GroupsTable`
--

CREATE TABLE `GroupsTable` (
  `id` int(11) NOT NULL,
  `name` varchar(45) CHARACTER SET utf8 DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Дамп данных таблицы `GroupsTable`
--

INSERT INTO `GroupsTable` (`id`, `name`) VALUES
(8, '1ОИБАС-1018'),
(5, '1ОИБАС-718'),
(6, '1ОИБАС-818'),
(7, '1ОИБАС-918'),
(1, '1ПКС-118'),
(2, '1ПКС-218'),
(3, '1ПКС-318'),
(4, '1ПКС-618'),
(13, '2ОИБАС-517'),
(14, '2ОИБАС-617'),
(15, '2ОИБАС-717'),
(9, '2ПКС-117'),
(10, '2ПКС-217'),
(11, '2ПКС-317'),
(12, '2ПКС-417'),
(20, '3ИБАС-516'),
(21, '3ИБАС-616'),
(22, '3ИБАС-716'),
(16, '3ПКС-116'),
(17, '3ПКС-216'),
(18, '3ПКС-316'),
(19, '3ПКС-416'),
(26, '4ИБАС-415'),
(27, '4ИБАС-515'),
(28, '4ИБАС-615'),
(23, '4ПКС-115'),
(24, '4ПКС-215'),
(25, '4ПКС-315');

-- --------------------------------------------------------

--
-- Структура таблицы `Outtable`
--

CREATE TABLE `Outtable` (
  `id` int(11) NOT NULL,
  `shortname` varchar(35) DEFAULT NULL,
  `localvalue` json NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Дамп данных таблицы `Outtable`
--

INSERT INTO `Outtable` (`id`, `shortname`, `localvalue`) VALUES
(1, 'outjson', '{\"716\": [\"Программно— аппаратные средства обеспечения информационной безопасности Поколодина Е.В.\", \"Основы философии Трусов Н.А.\", \"Иностранный язык Князева К.М. Записной Д.В.\", \"Программно— аппаратные средства обеспечения информационной безопасности Поколодина Е.В.\", \"-\"], \"4ПКС—115\": [\"Основы проектной деятельности Костиков П.А.\", \"Технология разработки программного обеспечения Морозова М.В.\", \"Документирование и сертификация Решетников В.Н.\", \"Документирование и сертификация Решетников В.Н.\", \"-\"], \"4ПКС—215\": [\"Документирование и сертификация Решетников В.Н.\", \"Технология разработки программного обеспечения _ Морозова М.В.\", \"Технология разработки программного обеспечения _ Морозова М.В.\", \"Технология разработки программного обеспечения _ Морозова М.В.\", \"-\"], \"4ПКС—315\": [\"Технология разработки программного обеспечения _ Морозова М.В.\", \"Документирование и сертификация Решетников В.Н.\", \"Технология разработки программного обеспечения _ Морозова М.В.\", \"Технология разработки программного обеспечения _ Морозова М.В.\", \"-\"], \"4ИБАС—415\": [\"Эксплуатация компьютерных сетей Володин С.М.\", \"Эксплуатация подсистем безопасности автоматизированных систем Рой А.В.\", \"Психология в профессиональной деятельности . Долгова Г.А.\", \"Психология в профессиональной деятельности . Долгова Г.А.\", \"-\"], \"4ИБАС—515\": [\"Экномика организации Потапова О.А.\", \"Эксплуатация компьютерных сетей Володин С.М.\", \"Эксплуатация подсистем безопасности автоматизированных систем Рой А.В.\", \"Эксплуатация подсистем безопасности автоматизированных систем Рой А.В.\", \"-\"], \"4ИБАС—615\": [\"Эксплуатация подсистем безопасности автоматизированных систем Рой А.В.\", \"Экномика организации Потапова О.А.\", \"Эксплуатация компьютерных сетей Володин С.М.\", \"Эксплуатация компьютерных сетей Володин С.М.\", \"-\"], \"2ОИБАС—517\": [\"Информатика Ожигова Н.И.\", \"Математика Белоглазов А.И.\", \"Физическая культура Коврижкин О.В.\", \"Классный час\", \"-\"]}'),
(2, 'grouplength', '5');

-- --------------------------------------------------------

--
-- Структура таблицы `Phones`
--

CREATE TABLE `Phones` (
  `id` int(11) NOT NULL,
  `phone` varchar(45) NOT NULL,
  `groups` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `Phones`
--

INSERT INTO `Phones` (`id`, `phone`, `groups`) VALUES
(1, '+75685856565', '2ОИБАС-517'),
(2, '+79077890709', '4ИБАС-515'),
(3, '+79897806889', '4ИБАС-515'),
(5, '+79809798689', '4ПКС-315');

-- --------------------------------------------------------

--
-- Структура таблицы `Proxies`
--

CREATE TABLE `Proxies` (
  `id` int(11) NOT NULL,
  `Server` varchar(45) NOT NULL,
  `Port` varchar(45) NOT NULL,
  `User` varchar(45) DEFAULT NULL,
  `Pass` varchar(45) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Дамп данных таблицы `Proxies`
--

INSERT INTO `Proxies` (`id`, `Server`, `Port`, `User`, `Pass`) VALUES
(1, '178.128.238.35', '2016', 'KOT', 'KRIS');

-- --------------------------------------------------------

--
-- Структура таблицы `ServiceTable`
--

CREATE TABLE `ServiceTable` (
  `id` int(11) NOT NULL,
  `ServiceName` varchar(45) NOT NULL,
  `ServiceKey` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `ServiceTable`
--

INSERT INTO `ServiceTable` (`id`, `ServiceName`, `ServiceKey`) VALUES
(1, 'SMSAeroLogin', 'SMSAeroLogin'),
(2, 'SMSAeroAPIKey', 'SMSAeroAPIKey'),
(3, 'TelegramStandaloneAPIKey', 'TelegramStandaloneAPIKey'),
(4, 'VKAPIKey', 'VKAPIKey'),
(5, 'TelegramAPIKey', 'TelegramAPIKey'),
(6, 'MasterPassword', 'MasterPassword'),
(7, 'EmailLogin', 'EmailLogin'),
(8, 'EmailPassword', 'EmailPassword');

-- --------------------------------------------------------

--
-- Структура таблицы `TGAdmins`
--

CREATE TABLE `TGAdmins` (
  `id` int(11) NOT NULL,
  `AdminTG` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Дамп данных таблицы `TGAdmins`
--

INSERT INTO `TGAdmins` (`id`, `AdminTG`) VALUES
(4, '394253'),
(3, '690476');

-- --------------------------------------------------------

--
-- Структура таблицы `Users`
--

CREATE TABLE `Users` (
  `id` int(11) NOT NULL,
  `Phone` varchar(100) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `Name` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Дамп данных таблицы `Users`
--

INSERT INTO `Users` (`id`, `Phone`, `Email`, `Password`, `Name`) VALUES
(3, '9eb0d9b44f6e3762b9959900a6de2012', '9eb0d9b44f6e3762b9959900a6de2012', '8491beedfed1ba98b144f193d5101bdd', '0JrQntCi0JjQmtCY'),
(4, '8cbb847f828d5d2f28a939f4611f650e', '8cbb847f828d5d2f28a939f4611f650e', '8491beedfed1ba98b144f193d5101bdd', '0JzQr9CjINCc0K/QoyDQnNCv0KM=');

-- --------------------------------------------------------

--
-- Структура таблицы `VKAdmins`
--

CREATE TABLE `VKAdmins` (
  `id` int(11) NOT NULL,
  `AdminVK` varchar(45) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

--
-- Дамп данных таблицы `VKAdmins`
--

INSERT INTO `VKAdmins` (`id`, `AdminVK`) VALUES
(5, '228'),
(1, '257350143'),
(2, '334479550');

--
-- Индексы сохранённых таблиц
--

--
-- Индексы таблицы `GroupsTable`
--
ALTER TABLE `GroupsTable`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `name_UNIQUE` (`name`);

--
-- Индексы таблицы `Outtable`
--
ALTER TABLE `Outtable`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `shortname_UNIQUE` (`shortname`);

--
-- Индексы таблицы `Phones`
--
ALTER TABLE `Phones`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `Phone_UNIQUE` (`phone`);

--
-- Индексы таблицы `Proxies`
--
ALTER TABLE `Proxies`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `id_UNIQUE` (`id`);

--
-- Индексы таблицы `ServiceTable`
--
ALTER TABLE `ServiceTable`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `ServiceName_UNIQUE` (`ServiceName`);

--
-- Индексы таблицы `TGAdmins`
--
ALTER TABLE `TGAdmins`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `AdminTG_UNIQUE` (`AdminTG`);

--
-- Индексы таблицы `Users`
--
ALTER TABLE `Users`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `Name_UNIQUE` (`Email`);

--
-- Индексы таблицы `VKAdmins`
--
ALTER TABLE `VKAdmins`
  ADD PRIMARY KEY (`id`),
  ADD UNIQUE KEY `AdminVK_UNIQUE` (`AdminVK`);

--
-- AUTO_INCREMENT для сохранённых таблиц
--

--
-- AUTO_INCREMENT для таблицы `Outtable`
--
ALTER TABLE `Outtable`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;
--
-- AUTO_INCREMENT для таблицы `Phones`
--
ALTER TABLE `Phones`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
--
-- AUTO_INCREMENT для таблицы `Proxies`
--
ALTER TABLE `Proxies`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=2;
--
-- AUTO_INCREMENT для таблицы `ServiceTable`
--
ALTER TABLE `ServiceTable`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;
--
-- AUTO_INCREMENT для таблицы `TGAdmins`
--
ALTER TABLE `TGAdmins`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
--
-- AUTO_INCREMENT для таблицы `Users`
--
ALTER TABLE `Users`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
--
-- AUTO_INCREMENT для таблицы `VKAdmins`
--
ALTER TABLE `VKAdmins`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
