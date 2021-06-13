DROP TABLE SUPPLIER CASCADE CONSTRAINTS PURGE;
DROP TABLE JENIS_BAHAN CASCADE CONSTRAINTS PURGE;
DROP TABLE BAHAN CASCADE CONSTRAINTS PURGE;
DROP TABLE H_BELI_BAHAN CASCADE CONSTRAINTS PURGE;
DROP TABLE D_BELI_BAHAN CASCADE CONSTRAINTS PURGE;
DROP TABLE H_RESEP CASCADE CONSTRAINTS PURGE;
DROP TABLE D_RESEP CASCADE CONSTRAINTS PURGE;
DROP TABLE JENIS_ROTI CASCADE CONSTRAINTS PURGE;
DROP TABLE ROTI CASCADE CONSTRAINTS PURGE;
DROP TABLE JABATAN CASCADE CONSTRAINTS PURGE;
DROP TABLE KARYAWAN CASCADE CONSTRAINTS PURGE;
DROP TABLE PELANGGAN CASCADE CONSTRAINTS PURGE;
DROP TABLE MEMBERSHIP CASCADE CONSTRAINTS PURGE;
DROP TABLE VOUCHER CASCADE CONSTRAINTS PURGE;
DROP TABLE H_TRANS CASCADE CONSTRAINTS PURGE;
DROP TABLE D_TRANS CASCADE CONSTRAINTS PURGE;
DROP TABLE USER_VOUCHER CASCADE CONSTRAINTS PURGE;


CREATE TABLE SUPPLIER (
    ID VARCHAR2(10) PRIMARY KEY,
	KODE VARCHAR2(12) NOT NULL,
    NAMA VARCHAR(100) NOT NULL,
    ALAMAT VARCHAR2(100) NOT NULL,
    EMAIL VARCHAR(50),
    NO_TELP VARCHAR(15) NOT NULL
);

CREATE TABLE JENIS_BAHAN (
    ID VARCHAR2(10) PRIMARY KEY,
    NAMA_JENIS VARCHAR2(100) NOT NULL
);

CREATE TABLE BAHAN(
    ID VARCHAR2(10) PRIMARY KEY,
	KODE VARCHAR2(12) NOT NULL,
    MERK VARCHAR2(50) NOT NULL,
    QTY_STOK NUMBER(10) NOT NULL,
	HARGA NUMBER(10) NOT NULL,
    SATUAN VARCHAR2(10) NOT NULL,
    JENIS_BAHAN VARCHAR(10) REFERENCES JENIS_BAHAN(ID) NOT NULL,
	PICTURE_LOCATION VARCHAR2(255) NOT NULL,
	STATUS INT NOT NULL
);
-- STATUS BAHAN
-- 0 : DELETED
-- 1 : ADA

CREATE TABLE H_BELI_BAHAN(
    NOMOR_NOTA VARCHAR2(15) PRIMARY KEY,
    TANGGAL_TRANS DATE NOT NULL,
    TOTAL INT NOT NULL,
    FK_SUPPLIER VARCHAR2(10) REFERENCES SUPPLIER(ID)
);

CREATE TABLE D_BELI_BAHAN(
    NOMOR_NOTA VARCHAR2(15) REFERENCES H_BELI_BAHAN(NOMOR_NOTA),
    FK_BAHAN VARCHAR2(10) REFERENCES BAHAN(ID),
    QUANTITY INT NOT NULL,
    HARGA INT NOT NULL,
    SUBTOTAL INT NOT NULL
);

CREATE TABLE H_RESEP(
    ID VARCHAR2(10) PRIMARY KEY,
    NAMA VARCHAR2(100) NOT NULL
);

CREATE TABLE D_RESEP(
    ID_H_RESEP VARCHAR2(10) REFERENCES H_RESEP(ID) NOT NULL,
    ID_BAHAN VARCHAR2(10) REFERENCES BAHAN(ID) NOT NULL,
    QTY INT NOT NULL
);

CREATE TABLE JENIS_ROTI (
    ID VARCHAR2(10) PRIMARY KEY,
    NAMA_JENIS VARCHAR2(100) NOT NULL
);

CREATE TABLE ROTI(
    ID VARCHAR2(10) PRIMARY KEY,
	KODE VARCHAR2(12) NOT NULL,
    NAMA VARCHAR2(100) NOT NULL,
    DESKRIPSI VARCHAR2(200) NOT NULL,
    HARGA NUMBER(25) NOT NULL,
	STOK NUMBER(25) NOT NULL,
    STATUS INT NOT NULL,
    JENIS_ROTI VARCHAR(10) REFERENCES JENIS_ROTI(ID) NOT NULL,
    FK_RESEP VARCHAR2(10) REFERENCES H_RESEP(ID) NOT NULL,
	PICTURE_LOCATION VARCHAR2(255) NOT NULL
);
-- STATUS ROTI
-- 0 : TIDAK PRODUKSI
-- 1 : PRODUKSI

CREATE TABLE JABATAN(
    ID VARCHAR2(10) PRIMARY KEY,
    NAMA_JABATAN VARCHAR2(100) NOT NULL
);

CREATE TABLE KARYAWAN(
    ID VARCHAR2(10) PRIMARY KEY,
	KODE VARCHAR2(12) NOT NULL,
    USERNAME VARCHAR2(100) NOT NULL,
    PASSWORD VARCHAR2(100) NOT NULL,
    NAMA VARCHAR(100) NOT NULL,
    JENIS_KELAMIN VARCHAR(1) NOT NULL CONSTRAINT CH_JENIS_KELAMIN CHECK(JENIS_KELAMIN = 'L' OR JENIS_KELAMIN = 'P'),
    ALAMAT VARCHAR2(100) NOT NULL,
    EMAIL VARCHAR(50) NOT NULL,
    NO_TELP VARCHAR(15) NOT NULL,
    TANGGAL_LAHIR DATE NOT NULL,
    STATUS INT NOT NULL,
    FK_JABATAN VARCHAR(10) REFERENCES JABATAN(ID),
	PICTURE_LOCATION VARCHAR2(255) NOT NULL
);
-- STATUS KARYAWAN
-- 0 : TIDAK AKTIF
-- 1 : AKTIF

CREATE TABLE PELANGGAN(
    ID VARCHAR2(10) PRIMARY KEY,
	KODE VARCHAR2(12) NOT NULL,
    USERNAME VARCHAR2(100) NOT NULL,
    PASSWORD VARCHAR2(100) NOT NULL,
    NAMA VARCHAR(100) NOT NULL,
    JENIS_KELAMIN VARCHAR(1) NOT NULL CONSTRAINT CK_JENIS_KELAMIN CHECK(JENIS_KELAMIN = 'L' OR JENIS_KELAMIN = 'P'),
    ALAMAT VARCHAR2(100) NOT NULL,
    EMAIL VARCHAR(50) NOT NULL,
    NO_TELP VARCHAR(15) NOT NULL,
    TANGGAL_LAHIR DATE NOT NULL,
    STATUS INT NOT NULL,
	PICTURE_LOCATION VARCHAR2(255) NOT NULL
);
-- STATUS PELANGGAN
-- 0 : TIDAK AKTIF
-- 1 : AKTIF

CREATE TABLE MEMBERSHIP(
    ID INT PRIMARY KEY,
    NAMA_MEMBERSHIP VARCHAR2(30) NOT NULL,
    HARGA_MEMBERSHIP INT NOT NULL,
    WAKTU_EXPIRED_MEMBERSHIP INT NOT NULL
);

CREATE TABLE PEMBAYARAN_MEMBERSHIP(
    ID INT PRIMARY KEY,
    ROW_ID_USER INT REFERENCES USERS(ID),
    ROW_ID_MEMBERSHIP INT REFERENCES MEMBERSHIP(ID),
    NOMOR_NOTA VARCHAR2(15) NOT NULL,
    METODE_PEMBAYARAN VARCHAR2(20) NOT NULL,
    STATUS_TRANSAKSI INT NOT NULL,
    CREATED_AT DATE,
    UPDATED_AT DATE 
);

CREATE TABLE VOUCHER(
    ID VARCHAR2(10) PRIMARY KEY,
    NAMA VARCHAR(100) NOT NULL,
    JENIS VARCHAR(15) NOT NULL,
    POTONGAN INT NOT NULL
);

CREATE TABLE USER_VOUCHER(
    ID VARCHAR2(10) PRIMARY KEY,
    FK_PELANGGAN VARCHAR2(10) REFERENCES PELANGGAN(ID),
    FK_VOUCHER VARCHAR2(10) REFERENCES VOUCHER(ID),
    EXP_DATE DATE,
    STATUS INT NOT NULL
);
-- STATUS USER_VOUCHER
-- 0: Aktif
-- 1: Non-Aktif

CREATE TABLE H_TRANS(
    NOMOR_NOTA VARCHAR2(15) PRIMARY KEY,
    TANGGAL_TRANS DATE NOT NULL,
    TOTAL INT NOT NULL,
    FK_KARYAWAN VARCHAR2(10) REFERENCES KARYAWAN(ID),
    FK_PELANGGAN VARCHAR2(10) REFERENCES PELANGGAN(ID),
    METODE_PEMBAYARAN VARCHAR2(10) NOT NULL,
    FK_USER_VOUCHER VARCHAR2(10) REFERENCES USER_VOUCHER(ID),
    STATUS INT NOT NULL
);
-- STATUS H_TRANS 
-- 0 : Belum Bayar
-- 1 : Request Sudah Bayar
-- 2 : Sudah Bayar
-- 3 : Cancelled

CREATE TABLE D_TRANS(
    NOMOR_NOTA VARCHAR2(15) REFERENCES H_TRANS(NOMOR_NOTA),
    FK_ROTI VARCHAR2(10) REFERENCES ROTI(ID),
    QUANTITY INT NOT NULL,
    HARGA INT NOT NULL,
    SUBTOTAL INT NOT NULL
);

-- SUPPLIER
INSERT INTO SUPPLIER VALUES(1, 'PTCA00001', 'PT Cahaya Ku Terang', 'Jalan Sabang Sampai Merauka No. 54', 'cahayakuterang@gmail.co.id', 082234478423);
INSERT INTO SUPPLIER VALUES(2, 'TOBA00001', 'Toko Bahan Kue Sylvia', 'Jalan Jalan Terus Aja Bang No. 32', 'cangkulenak@gmail.com', 083111554038);
INSERT INTO SUPPLIER VALUES(3, 'TOHA00001', 'Toko Handi Jaya', 'Jalan Walikota Mustafa No. 2a', 'handihandikong@gmail.co.id', 081234459247);
INSERT INTO SUPPLIER VALUES(4, 'TOJA00001', 'Toko Jaya Abadi', 'Jalan Halo halo Surabaya No. 4d', 'jayaabadi@gmail.com', 087132612371);
INSERT INTO SUPPLIER VALUES(5, 'PTHA00001', 'PT Hariku Cerah', 'Jalan Diponorogo No. 37', 'cerahhariku@gmail.com', 08312314123);
INSERT INTO SUPPLIER VALUES(6, 'TOBA00002', 'Toko Bahan Kue Langkah', 'Jalan Wakil Walikota Mustofa No. 4a', 'uhsheup@gmail.com', 081241231237);
INSERT INTO SUPPLIER VALUES(7, 'TOBA00003', 'Toko Bahan Kue Gledek', 'Jalan Buderan IH No. 78', 'tokogledek@gmail.com', 088123659285);
INSERT INTO SUPPLIER VALUES(8, 'PTTI00001', 'PT Tiga Gajah', 'Jalan Tiga Semut IV/29', 'tigagajah@gmail.com', 0872658923658);
INSERT INTO SUPPLIER VALUES(9, 'PTSE00001', 'PT Segitiga Pelangi', 'Jalan Angkat Kaki V No. 42', 'segitigapelangi@gmail.com', 082572638259);
INSERT INTO SUPPLIER VALUES(10, 'PTSE00001', 'Toko Sering Laku', 'Jalan Bagel No. 65', 'seringlaku@gmail.com', 087265916824);
INSERT INTO SUPPLIER VALUES(11, 'TOBA00004', 'Toko Bahan Kue Dua Anak', 'Jalan Anak Kembar No. 241', 'duaanak@gmail.com', 085726382592);

-- JENIS_BAHAN
INSERT INTO JENIS_BAHAN VALUES(1, 'FERMIPAN');
INSERT INTO JENIS_BAHAN VALUES(2, 'BAKING POWDER');
INSERT INTO JENIS_BAHAN VALUES(3, 'TEPUNG TERIGU');
INSERT INTO JENIS_BAHAN VALUES(4, 'BUTTER');
INSERT INTO JENIS_BAHAN VALUES(5, 'MARGARIN');
INSERT INTO JENIS_BAHAN VALUES(6, 'SUSU CAIR');
INSERT INTO JENIS_BAHAN VALUES(7, 'SUSU BUBUK');
INSERT INTO JENIS_BAHAN VALUES(8, 'RAGI');

-- BAHAN
INSERT INTO BAHAN VALUES(1, 'KOKO00001', 'KOEPOE KOEPOE', 5, 3000, 'GRAM', 2, 'KOKO00001.jpg', 1);
INSERT INTO BAHAN VALUES(2, 'SEKU00001', 'SEDAP KUE', 3, 3500, 'GRAM', 1, 'SEKU00001.jpg', 1);
INSERT INTO BAHAN VALUES(3, 'HERC00001', 'HERCULES', 5, 6500, 'GRAM', 2, 'HERC00001.jpg', 1);
INSERT INTO BAHAN VALUES(4, 'SEBI00001', 'SEGITIGA BIRU', 3, 15000, 'GRAM', 3, 'SEBI00001.jpg', 1);
INSERT INTO BAHAN VALUES(5, 'CAKE00001', 'CAKRA KEMBAR', 4, 12000, 'GRAM', 3, 'CAKE00001.jpg', 1);
INSERT INTO BAHAN VALUES(6, 'KUBI00001', 'KUNCI BIRU', 4, 13000, 'GRAM', 3, 'KUBI00001.jpg', 1);
INSERT INTO BAHAN VALUES(7, 'ANCH00001', 'ANCHOR', 10, 5000, 'GRAM', 4, 'ANCH00001.jpg', 1);
INSERT INTO BAHAN VALUES(8, 'ELVI00001', 'ELLE VIRE', 10, 6000, 'GRAM', 4, 'ELVI00001.jpg', 1);
INSERT INTO BAHAN VALUES(9, 'ORCH00001', 'ORCHID', 6, 9000, 'GRAM', 4, 'ORCH00001.jpg', 1);
INSERT INTO BAHAN VALUES(10, 'BLBA00001', 'BLUE BAND', 10, 6000, 'GRAM', 5, 'BLBA00001.jpg', 1);
INSERT INTO BAHAN VALUES(11, 'PALM00001', 'PALMIA', 8, 15000, 'GRAM', 5, 'PALM00001.jpg', 1);
INSERT INTO BAHAN VALUES(12, 'DIUH00001', 'DIAMOND UHT', 5, 10000, 'mL', 6, 'DIUH00001.jpg', 1);
INSERT INTO BAHAN VALUES(13, 'GRSK00001', 'GREENFIELD SKIMMED', 5, 12000, 'mL', 6, 'GRSK00001.jpg', 1);
INSERT INTO BAHAN VALUES(14, 'DANC00001', 'DANCOW', 10, 25000, 'GRAM', 7, 'DANC00001.jpg', 1);
INSERT INTO BAHAN VALUES(15, 'INDO00001', 'INDOMILK', 10, 15000, 'GRAM', 7, 'INDO00001.jpg', 1);
INSERT INTO BAHAN VALUES(16, 'NEGO00001', 'NEVADA GOLD', 8, 30000, 'GRAM', 8, 'NEGO00001.jpg', 1);

-- H_BELI_BAHAN
INSERT INTO H_BELI_BAHAN VALUES('BELI20201226001', TO_DATE('26-12-2020', 'DD-MM-YYYY'), 31000, 1);
INSERT INTO H_BELI_BAHAN VALUES('BELI20201226002', TO_DATE('26-12-2020', 'DD-MM-YYYY'), 21000, 2);
INSERT INTO H_BELI_BAHAN VALUES('BELI20201227001', TO_DATE('27-12-2020', 'DD-MM-YYYY'), 67000, 4);
INSERT INTO H_BELI_BAHAN VALUES('BELI20201229001', TO_DATE('29-12-2020', 'DD-MM-YYYY'), 9000, 1);
INSERT INTO H_BELI_BAHAN VALUES('BELI20201229002', TO_DATE('29-12-2020', 'DD-MM-YYYY'), 135000, 3);
INSERT INTO H_BELI_BAHAN VALUES('BELI20210102001', TO_DATE('02-01-2021', 'DD-MM-YYYY'), 49000, 6);
INSERT INTO H_BELI_BAHAN VALUES('BELI20210102002', TO_DATE('02-01-2021', 'DD-MM-YYYY'), 40000, 5);
INSERT INTO H_BELI_BAHAN VALUES('BELI20210125001', TO_DATE('25-01-2021', 'DD-MM-YYYY'), 60000, 1);
INSERT INTO H_BELI_BAHAN VALUES('BELI20210212001', TO_DATE('12-02-2021', 'DD-MM-YYYY'), 115000, 2);
INSERT INTO H_BELI_BAHAN VALUES('BELI20210214001', TO_DATE('14-02-2021', 'DD-MM-YYYY'), 43500, 5);
INSERT INTO H_BELI_BAHAN VALUES('BELI20210223001', TO_DATE('23-02-2021', 'DD-MM-YYYY'), 135000, 8);

-- D_BELI_BAHAN
INSERT INTO D_BELI_BAHAN VALUES('BELI20201226001', 1, 3, 3000, 9000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201226001', 2, 2, 3500, 7000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201226001', 4, 1, 15000, 15000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201226002', 9, 1, 9000, 9000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201226002', 10, 2, 6000, 12000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201227001', 6, 1, 13000, 13000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201227001', 8, 5, 6000, 30000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201227001', 13, 2, 12000, 24000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201229001', 1, 3, 3000, 9000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201229002', 14, 3, 25000, 75000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20201229002', 15, 4, 15000, 60000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210102001', 6, 3, 13000, 39000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210102001', 7, 2, 5000, 10000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210102002', 1, 4, 3000, 12000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210102002', 4, 1, 15000, 15000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210102002', 3, 2, 6500, 13000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210125001', 8, 4, 6000, 24000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210125001', 13, 3, 12000, 36000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210212001', 10, 7, 6000, 42000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210212001', 3, 2, 6500, 13000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210212001', 5, 5, 12000, 60000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210214001', 2, 3, 3500, 10500);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210214001', 9, 1, 9000, 9000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210214001', 10, 4, 6000, 24000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210223001', 11, 6, 15000, 90000);
INSERT INTO D_BELI_BAHAN VALUES('BELI20210223001', 4, 3, 15000, 45000);




-- H_RESEP
INSERT INTO H_RESEP VALUES(1, 'Resep Croissant');
INSERT INTO H_RESEP VALUES(2, 'Resep Raisin Toast');
INSERT INTO H_RESEP VALUES(3, 'Resep Brioche');
INSERT INTO H_RESEP VALUES(4, 'Resep Belgian Waffle');
INSERT INTO H_RESEP VALUES(5, 'Resep Biscuit');
INSERT INTO H_RESEP VALUES(6, 'Resep Carrot Bread');
INSERT INTO H_RESEP VALUES(7, 'Resep Chapati');
INSERT INTO H_RESEP VALUES(8, 'Resep Crepe');
INSERT INTO H_RESEP VALUES(9, 'Resep Dorayaki');
INSERT INTO H_RESEP VALUES(10, 'Resep Fougasse');
INSERT INTO H_RESEP VALUES(11, 'Resep Roti Bakar');
INSERT INTO H_RESEP VALUES(12, 'Resep Tortilla');
INSERT INTO H_RESEP VALUES(13, 'Resep White Bread');
INSERT INTO H_RESEP VALUES(14, 'Resep Bing Zi');
INSERT INTO H_RESEP VALUES(15, 'Resep Potato Bread');

-- D_RESEP
INSERT INTO D_RESEP VALUES(1, 1, 3);
INSERT INTO D_RESEP VALUES(1, 2, 1);
INSERT INTO D_RESEP VALUES(2, 4, 24);
INSERT INTO D_RESEP VALUES(2, 3, 6);
INSERT INTO D_RESEP VALUES(2, 16, 7);
INSERT INTO D_RESEP VALUES(3, 5, 20);
INSERT INTO D_RESEP VALUES(3, 7, 5);
INSERT INTO D_RESEP VALUES(4, 3, 10);
INSERT INTO D_RESEP VALUES(4, 5, 20);
INSERT INTO D_RESEP VALUES(5, 5, 15);
INSERT INTO D_RESEP VALUES(5, 11, 5);
INSERT INTO D_RESEP VALUES(6, 4, 15);
INSERT INTO D_RESEP VALUES(6, 1, 6);
INSERT INTO D_RESEP VALUES(7, 4, 12);
INSERT INTO D_RESEP VALUES(8, 6, 15);
INSERT INTO D_RESEP VALUES(8, 12, 10);
INSERT INTO D_RESEP VALUES(8, 8, 5);
INSERT INTO D_RESEP VALUES(9, 13, 7);
INSERT INTO D_RESEP VALUES(9, 5, 15);
INSERT INTO D_RESEP VALUES(10, 3, 16);
INSERT INTO D_RESEP VALUES(10, 2, 5);
INSERT INTO D_RESEP VALUES(11, 6, 9);
INSERT INTO D_RESEP VALUES(11, 9, 3);
INSERT INTO D_RESEP VALUES(12, 5, 12);
INSERT INTO D_RESEP VALUES(13, 2, 7);
INSERT INTO D_RESEP VALUES(13, 4, 10);
INSERT INTO D_RESEP VALUES(14, 6, 9);
INSERT INTO D_RESEP VALUES(14, 11, 4);
INSERT INTO D_RESEP VALUES(15, 4, 12);
INSERT INTO D_RESEP VALUES(15, 9, 7);

-- JENIS_ROTI
INSERT INTO JENIS_ROTI VALUES(1, 'BREAD');
INSERT INTO JENIS_ROTI VALUES(2, 'TOAST');
INSERT INTO JENIS_ROTI VALUES(3, 'FLAT BREAD');
INSERT INTO JENIS_ROTI VALUES(4, 'DRY CAKE');
INSERT INTO JENIS_ROTI VALUES(5, 'COOKIES');
INSERT INTO JENIS_ROTI VALUES(6, 'WAFFLE');
INSERT INTO JENIS_ROTI VALUES(7, 'PANCAKE');

-- ROTI
INSERT INTO ROTI VALUES(1, 'CROI00001', 'Croissant', 'A Very Exquisite Bread', 9000, 10, 1, 1, 1, 'CROI00001.jpg');
INSERT INTO ROTI VALUES(2, 'RATO00001', 'Raisin Toast', 'Healthy!', 16000, 7, 1, 2, 2, 'RATO00001.jpg');
INSERT INTO ROTI VALUES(3, 'BRIO00001', 'Brioche', 'A Highly Enriched Bread From France!', 22000, 9, 1, 1, 3, 'BRIO00001.jpg');
INSERT INTO ROTI VALUES(4, 'BEWA00001', 'Belgian Waffle', 'Larger Wafle Is Yummy!', 25000, 10, 1, 6, 4, 'BEWA00001.jpg');
INSERT INTO ROTI VALUES(5, 'BISC00001', 'Biscuit', 'Perfect For a Snack!', 8000, 35, 1, 5, 5, 'BISC00001.jpg');
INSERT INTO ROTI VALUES(6, 'CABR00001', 'Carrot Bread', 'A Healthy Choice!', 18000, 8, 0, 1, 6, 'CABR00001.jpg');
INSERT INTO ROTI VALUES(7, 'CHAP00001', 'Chapati', 'It Would Be Nice For a Kebab', 5000, 20, 1, 3, 7, 'CHAP00001.jpg');
INSERT INTO ROTI VALUES(8, 'CREP00001', 'Crepe', 'Extremeley Thin Pancake', 7500, 16, 1, 7, 8, 'CREP00001.jpg');
INSERT INTO ROTI VALUES(9, 'DORA00001', 'Dorayaki', 'Japanese Trademark', 18000, 40, 1, 7, 9, 'DORA00001.jpg');
INSERT INTO ROTI VALUES(10, 'FOUG00001', 'Fougasse', 'Delicate Bread', 16000, 14, 1, 1, 10, 'FOUG00001.jpg');
INSERT INTO ROTI VALUES(11, 'ROBA00001', 'Roti Bakar', 'Indonesia Number One Bread', 15000, 21, 1, 2, 11, 'ROBA00001.jpg');
INSERT INTO ROTI VALUES(12, 'TORT00001', 'Tortilla', 'Flat Yet Applicable To Any Food', 6000, 37, 1, 3, 12, 'TORT00001.jpg');
INSERT INTO ROTI VALUES(13, 'WHBR00001', 'White Bxread', 'Classic', 13500, 10, 1, 1, 13, 'WHBR00001.jpg');
INSERT INTO ROTI VALUES(14, 'BIZI00001', 'Bing Zi', 'Originally From China', 7500, 5, 1, 3, 14, 'BIZI00001.jpg');
INSERT INTO ROTI VALUES(15, 'POBR00001', 'Potato Bread', 'Who Would Deny This?', 12000, 7, 0, 1, 15, 'POBR00001.jpg');

-- JABATAN
INSERT INTO JABATAN VALUES(1, 'KARYAWAN');
INSERT INTO JABATAN VALUES(2, 'MANAGER');
INSERT INTO JABATAN VALUES(3, 'CHEF');

-- KARYAWAN
INSERT INTO KARYAWAN VALUES(0, 'ADMN00001', 'Admin', 'Admin', 'Admin', 'L', 'Jalan Admin', 'admin@gmail.com', 0888888888, TO_DATE('01/01/1000', 'dd/mm/yyyy'), 1, 1, 'ADMN0001.jpg');
INSERT INTO KARYAWAN VALUES(1, 'BURA00001', 'BudiR', 'BudiR', 'Budi Raharja', 'L', 'Jalan Mendoan No. 4', 'budiraharja@gmail.com', 082114670146, TO_DATE('03/08/1986', 'dd/mm/yyyy'), 1, 1, 'BURA00001.jpg');
INSERT INTO KARYAWAN VALUES(2, 'SEAD00001', 'SetyoA', 'SetyoA', 'Setyo Adi', 'L', 'Jalan Pavilion No. 21', 'setyoadi@gmail.com', 082241246613, TO_DATE('13/06/1987', 'dd/mm/yyyy'), 1, 1, 'SEAD00001.jpg');
INSERT INTO KARYAWAN VALUES(3, 'YADA00001', 'YaekoD', 'YaekoD', 'Yaeko Dario', 'P', 'Jalan Ngagel Jaya No. 26', 'yaekodario@gmail.com', 081257224666, TO_DATE('01/06/1990', 'dd/mm/yyyy'), 1, 1, 'YADA00001.jpg');
INSERT INTO KARYAWAN VALUES(4, 'MEPU00001', 'MelianaP', 'MelianaP', 'Meliana Purnama', 'P', 'Jalan Penamparan No. 15', 'melianapurnama@gmail.com', 0838555682, TO_DATE('01/06/1990', 'dd/mm/yyyy'), 0, 2, 'MEPU00001.jpg');
INSERT INTO KARYAWAN VALUES(5, 'MIED00001', 'MihaelaE', 'MihaelaE', 'Mihaela Edvige', 'L', 'Jalan Kipas Dewa No. 6', 'mihaelaedvige@gmail.com', 0897555472, TO_DATE('12/08/1984', 'dd/mm/yyyy'), 1, 2, 'MIED00001.jpg');
INSERT INTO KARYAWAN VALUES(6, 'EMBR00001', 'EmiliB', 'EmiliB', 'Emili Brendan', 'P', 'Jalan Kursi Terbang No. 12', 'emilibrendan@gmail.com', 0875433694, TO_DATE('28/06/1990', 'dd/mm/yyyy'), 1, 2, 'EMBR00001.jpg');
INSERT INTO KARYAWAN VALUES(7, 'KAPA00001', 'KassandrosP', 'KassandrosP', 'Kassandros Paraskeve', 'L', 'Jalan Ikan Koi No. 67', 'kassandros@gmail.com', 08713772929 , TO_DATE('30/07/1992', 'dd/mm/yyyy'), 1, 3, 'KAPA00001.jpg');
INSERT INTO KARYAWAN VALUES(8, 'FANI00001', 'FabianN', 'FabianN', 'Fabian Nicodemo', 'L', 'Jalan Ikan Mas No. 64', 'fabianico@gmail.com', 08743533798 , TO_DATE('16/11/1988', 'dd/mm/yyyy'), 1, 3, 'FANI00001.jpg');
INSERT INTO KARYAWAN VALUES(9, 'ALAR00001', 'AliceA', 'AliceA', 'Alice Artur', 'P', 'Jalan Ikan Badut No. 91', 'aliceeee@gmail.com', 08721197429 , TO_DATE('04/11/1989', 'dd/mm/yyyy'), 1, 3, 'ALAR00001.jpg');

-- PELANGGAN
INSERT INTO PELANGGAN VALUES(1, 'RATR00001', 'RaymondT', 'RaymondT', 'Raymond Tranatung', 'L', 'Jalan Penampungan No. 6', 'raymondt@gmail.com', 08345671222, TO_DATE('13/06/2001', 'dd/mm/yyyy'), 1, 'a');
INSERT INTO PELANGGAN VALUES(2, 'LUGE00001', 'LucianaG', 'LucianaG', 'Luciana Geraldine', 'P', 'Jalan Penampungan No. 9', 'lucianaa@gmail.com', 08512222451, TO_DATE('06/06/2001', 'dd/mm/yyyy'), 1, 'a');
INSERT INTO PELANGGAN VALUES(3, 'ROYU00001', 'RoyY', 'RoyY', 'Roy Yuri', 'L', 'Jalan Ngagel Selatan No. 12', 'royy@gmail.com', 08512366135, TO_DATE('30/05/1992', 'dd/mm/yyyy'), 1, 'a');
INSERT INTO PELANGGAN VALUES(4, 'ORHA00001', 'OrlaH', 'OrlaH', 'Orla Hanifa', 'P', 'Jalan Pamungkas Blok A No. 121', 'orlah@gmail.com', 0878555167, TO_DATE('21/12/1989', 'dd/mm/yyyy'), 1, 'a');
INSERT INTO PELANGGAN VALUES(5, 'WAIV00001', 'WaelI', 'WaelI', 'Wael Ivanka', 'L', 'Jalan Sudirman Gang 2 No. 12', 'waeli@gmail.com', 0812215949, TO_DATE('24/03/1993', 'dd/mm/yyyy'), 0, 'a');
INSERT INTO PELANGGAN VALUES(6, 'AIVI00001', 'AikaterineV', 'AikaterineV', 'Aikaterine Vitaliano', 'P', 'Jalan Kapasari Gang 1 No. 4', 'aikaterine@gmail.com', 0859525744, TO_DATE('15/10/1992', 'dd/mm/yyyy'), 1, 'a');
INSERT INTO PELANGGAN VALUES(7, 'GLVL00001', 'GlendaV', 'GlendaV', 'Glenda Vlad', 'L', 'Jalan Roti Bakar No. 34a', 'glendav@gmail.com', 0812675723, TO_DATE('17/09/1996', 'dd/mm/yyyy'), 1, 'a');
INSERT INTO PELANGGAN VALUES(8, 'THPA00001', 'ThorsteinP', 'ThorsteinP', 'Thorstein Parminder', 'P', 'Jalan Perjalanan No. 45', 'thorstein@gmail.com', 0838555838, TO_DATE('05/05/2003', 'dd/mm/yyyy'), 1, 'a');
INSERT INTO PELANGGAN VALUES(9, 'UMVA00001', 'UmarV', 'UmarV', 'Umar Valentin', 'L', 'Jalan Perjalanan No. 46', 'umarr@gmail.com', 08385123167, TO_DATE('09/09/1999', 'dd/mm/yyyy'), 1, 'a');

-- VOURCHER
INSERT INTO VOUCHER VALUES(1, 'HEMATDULUBOS', 'POTONGAN', 30000);
INSERT INTO VOUCHER VALUES(2, 'DISKONMURAH', 'POTONGAN', 15000);
INSERT INTO VOUCHER VALUES(3, 'BREADTOKAEREK', 'DISKON', 30);
INSERT INTO VOUCHER VALUES(4, 'ROYALFAMILY', 'DISKON', 50);
INSERT INTO VOUCHER VALUES(5, 'BREADTOKDONG', 'POTONGAN', 25000);

-- USER_VOUCHER
INSERT INTO USER_VOUCHER VALUES(1, 1, 2, TO_DATE('23/05/2021', 'dd/mm/yyyy'), 0);
INSERT INTO USER_VOUCHER VALUES(2, 6, 1, TO_DATE('09/08/2021', 'dd/mm/yyyy'), 0);
INSERT INTO USER_VOUCHER VALUES(3, 5, 4, TO_DATE('09/08/2021', 'dd/mm/yyyy'), 0);
INSERT INTO USER_VOUCHER VALUES(4, 2, 2, TO_DATE('09/08/2021', 'dd/mm/yyyy'), 0);
INSERT INTO USER_VOUCHER VALUES(5, 9, 3, TO_DATE('09/08/2021', 'dd/mm/yyyy'), 0);
INSERT INTO USER_VOUCHER VALUES(6, 4, 5, TO_DATE('09/08/2021', 'dd/mm/yyyy'), 0);

-- H_TRANS
INSERT INTO H_TRANS VALUES('NOTA20210114001', TO_DATE('14-01-2021', 'DD-MM-YYYY'), 61000, 2, 2, 'OVO', null, 3);
INSERT INTO H_TRANS VALUES('NOTA20210114002', TO_DATE('14-01-2021', 'DD-MM-YYYY'), 64000, 3, 1, 'OVO', 1, 2);
INSERT INTO H_TRANS VALUES('NOTA20210206001', TO_DATE('06-02-2021', 'DD-MM-YYYY'), 29500, 1, 1, 'GO-PAY', null, 2);
INSERT INTO H_TRANS VALUES('NOTA20210207001', TO_DATE('07-02-2021', 'DD-MM-YYYY'), 16000, null, 3, 'SHOPEEPAY', null, 1);
INSERT INTO H_TRANS VALUES('NOTA20210207002', TO_DATE('07-02-2021', 'DD-MM-YYYY'), 86000, null, 6, 'GO-PAY', 2, 0);
INSERT INTO H_TRANS VALUES('NOTA20210207003', TO_DATE('07-02-2021', 'DD-MM-YYYY'), 32500, null, 4, 'SHOPEEPAY', null, 1);
INSERT INTO H_TRANS VALUES('NOTA20210209001', TO_DATE('09-02-2021', 'DD-MM-YYYY'), 47500, null, 5, 'OVO', 3, 1);
INSERT INTO H_TRANS VALUES('NOTA20210209002', TO_DATE('09-02-2021', 'DD-MM-YYYY'), 61000, 2, 7, 'SHOPEEPAY', null, 3);
INSERT INTO H_TRANS VALUES('NOTA20210213001', TO_DATE('13-02-2021', 'DD-MM-YYYY'), 35000, null, 2, 'GO-PAY', 4, 0);
INSERT INTO H_TRANS VALUES('NOTA20210215001', TO_DATE('15-02-2021', 'DD-MM-YYYY'), 57500, null, 8, 'OVO', null, 0);
INSERT INTO H_TRANS VALUES('NOTA20210216001', TO_DATE('16-02-2021', 'DD-MM-YYYY'), 68600, null, 9, 'SHOPEEPAY', 5, 1);
INSERT INTO H_TRANS VALUES('NOTA20210218001', TO_DATE('18-02-2021', 'DD-MM-YYYY'), 59000, null, 4, 'GO-PAY', 6, 1);
INSERT INTO H_TRANS VALUES('NOTA20210220001', TO_DATE('20-02-2021', 'DD-MM-YYYY'), 37000, null, 5, 'SHOPEEPAY', null, 1);

-- D_TRANS
INSERT INTO D_TRANS VALUES('NOTA20210114001', 1, 5, 9000, 45000);
INSERT INTO D_TRANS VALUES('NOTA20210114001', 2, 1, 16000, 16000);
INSERT INTO D_TRANS VALUES('NOTA20210114002', 5, 2, 16000, 32000);
INSERT INTO D_TRANS VALUES('NOTA20210114002', 7, 3, 5000, 15000);
INSERT INTO D_TRANS VALUES('NOTA20210114002', 2, 2, 16000, 32000);
INSERT INTO D_TRANS VALUES('NOTA20210206001', 13, 1, 13500, 13500);
INSERT INTO D_TRANS VALUES('NOTA20210206001', 10, 1, 16000, 16000);
INSERT INTO D_TRANS VALUES('NOTA20210207001', 2, 1, 16000, 16000);
INSERT INTO D_TRANS VALUES('NOTA20210207002', 1, 3, 9000, 27000);
INSERT INTO D_TRANS VALUES('NOTA20210207002', 4, 1, 25000, 25000);
INSERT INTO D_TRANS VALUES('NOTA20210207002', 5, 8, 8000, 64000);
INSERT INTO D_TRANS VALUES('NOTA20210207003', 7, 2, 5000, 10000);
INSERT INTO D_TRANS VALUES('NOTA20210207003', 8, 3, 7500, 22500);
INSERT INTO D_TRANS VALUES('NOTA20210209001', 11, 3, 15000, 45000);
INSERT INTO D_TRANS VALUES('NOTA20210209001', 9, 1, 18000, 18000);
INSERT INTO D_TRANS VALUES('NOTA20210209001', 10, 2, 16000, 32000);
INSERT INTO D_TRANS VALUES('NOTA20210209002', 1, 5, 9000, 45000);
INSERT INTO D_TRANS VALUES('NOTA20210209002', 2, 1, 16000, 16000);
INSERT INTO D_TRANS VALUES('NOTA20210213001', 7, 10, 5000, 50000);
INSERT INTO D_TRANS VALUES('NOTA20210215001', 3, 2, 22000, 44000);
INSERT INTO D_TRANS VALUES('NOTA20210215001', 13, 1, 13500, 13500);
INSERT INTO D_TRANS VALUES('NOTA20210216001', 8, 4, 7500, 30000);
INSERT INTO D_TRANS VALUES('NOTA20210216001', 9, 2, 18000, 36000);
INSERT INTO D_TRANS VALUES('NOTA20210216001', 2, 2, 16000, 32000);
INSERT INTO D_TRANS VALUES('NOTA20210218001', 1, 1, 9000, 9000);
INSERT INTO D_TRANS VALUES('NOTA20210218001', 4, 3, 25000, 75000);
INSERT INTO D_TRANS VALUES('NOTA20210220001', 7, 1, 5000, 5000);
INSERT INTO D_TRANS VALUES('NOTA20210220001', 10, 2, 16000, 32000);

commit;



