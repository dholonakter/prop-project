-- phpMyAdmin SQL Dump
-- version 4.6.4
-- https://www.phpmyadmin.net/
--
-- Host: studmysql01.fhict.local
-- Generation Time: May 22, 2018 at 08:07 PM
-- Server version: 5.7.13-log
-- PHP Version: 5.6.33

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `dbi364365`
--

-- --------------------------------------------------------

--
-- Table structure for table `deletedvisitor`
--

CREATE TABLE `deletedvisitor` (
  `Id` int(11) NOT NULL,
  `FullName` varchar(255) NOT NULL,
  `EmailAddress` varchar(255) NOT NULL,
  `PhoneNumber` int(10) NOT NULL,
  `Balance` double NOT NULL,
  `RFID` varchar(255) NOT NULL,
  `VisitorId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

--
-- Dumping data for table `deletedvisitor`
--

INSERT INTO `deletedvisitor` (`Id`, `FullName`, `EmailAddress`, `PhoneNumber`, `Balance`, `RFID`, `VisitorId`) VALUES
(2, 'xzcxzc', 'cvcxvb', 53636, 10, '0a006f489c', 17),
(3, 'hello', 'fdgfd', 2414, 10, '1a008cceb6', 18),
(4, 'dfsdg', 'dfgfg', 1224, 20, '0a006f489c', 20);

--
-- Indexes for dumped tables
--

--
-- Indexes for table `deletedvisitor`
--
ALTER TABLE `deletedvisitor`
  ADD PRIMARY KEY (`Id`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `deletedvisitor`
--
ALTER TABLE `deletedvisitor`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
