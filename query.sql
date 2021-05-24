CREATE OR REPLACE FUNCTION AUTOGEN_ID_PELANGGAN
RETURN VARCHAR2
IS
    HASIL PELANGGAN.ID%TYPE;
BEGIN

    SELECT MAX(ID) + 1 INTO HASIL FROM PELANGGAN;

    RETURN HASIL;
END;
/
SHOW ERR;



CREATE OR REPLACE TRIGGER AUTOGEN_ID_BAHAN
BEFORE INSERT OR UPDATE ON BAHAN
FOR EACH ROW
DECLARE
    ID varchar2(100);
	CHECK0 varchar2(20);
	CHECK1 varchar2(20);
	START_CODE varchar2(20);
    END_CODE varchar2(20);
	flag number(10);
	pragma autonomous_transaction;
BEGIN
	flag := 0;
	if updating then
		IF INSTR(:OLD.MERK, ' ') > 0 THEN
			CHECK0 := SUBSTR(:OLD.MERK, 1, 2) || SUBSTR(:OLD.MERK, INSTR(:OLD.MERK, ' ') + 1, 2);
			CHECK1 := SUBSTR(:NEW.MERK, 1, 2) || SUBSTR(:NEW.MERK, INSTR(:NEW.MERK, ' ') + 1, 2);
		ELSE
			CHECK0 := SUBSTR(:OLD.MERK, 1, 4);
			CHECK1 := SUBSTR(:NEW.MERK, 1, 4);
		END IF;
		
		if CHECK0 <> CHECK1 then
			flag := 2;
		end if;
	elsif inserting then
		flag := 1;
	end if;
	
	if flag > 0 then
		select to_char(max(to_number(ID))+1)
		into ID
		from BAHAN;
		
		IF INSTR(:NEW.MERK, ' ') > 0 THEN
			START_CODE := SUBSTR(:NEW.MERK, 1, 2) || SUBSTR(:NEW.MERK, INSTR(:NEW.MERK, ' ') + 1, 2);
		ELSE
			START_CODE := SUBSTR(:NEW.MERK, 1, 4);
		END IF;
		
		SELECT LPAD(NVL(MAX(TO_NUMBER(SUBSTR(B.KODE, 5,5))), 0)+1, 5, '0') INTO END_CODE
		FROM BAHAN B
		WHERE SUBSTR(B.KODE, 1, 4) = UPPER(START_CODE);
    :NEW.KODE := START_CODE || END_CODE;
    :NEW.PICTURE_LOCATION := START_CODE || END_CODE || '.jpg';

    if flag = 1 then
      :new.ID := ID;
      :new.STATUS := 1;
    end if;
  end if;
END;
/

CREATE OR REPLACE TRIGGER AUTOGEN_ID_VOUCHER
BEFORE INSERT ON VOUCHER
FOR EACH ROW
DECLARE
    ID varchar2(100);
	CHECK0 varchar2(20);
	CHECK1 varchar2(20);
	START_CODE varchar2(20);
    END_CODE varchar2(20);
	flag number(10);
	pragma autonomous_transaction;
BEGIN
	flag := 0;
	if updating then
		IF INSTR(:OLD.MERK, ' ') > 0 THEN
			CHECK0 := SUBSTR(:OLD.MERK, 1, 2) || SUBSTR(:OLD.MERK, INSTR(:OLD.MERK, ' ') + 1, 2);
			CHECK1 := SUBSTR(:NEW.MERK, 1, 2) || SUBSTR(:NEW.MERK, INSTR(:NEW.MERK, ' ') + 1, 2);
		ELSE
			CHECK0 := SUBSTR(:OLD.MERK, 1, 4);
			CHECK1 := SUBSTR(:NEW.MERK, 1, 4);
		END IF;
		
		if CHECK0 <> CHECK1 then
			flag := 2;
		end if;
	elsif inserting then
		flag := 1;
	end if;
	
	if flag > 0 then
		select to_char(max(to_number(ID))+1)
		into ID
		from BAHAN;
		
		IF INSTR(:NEW.MERK, ' ') > 0 THEN
			START_CODE := SUBSTR(:NEW.MERK, 1, 2) || SUBSTR(:NEW.MERK, INSTR(:NEW.MERK, ' ') + 1, 2);
		ELSE
			START_CODE := SUBSTR(:NEW.MERK, 1, 4);
		END IF;
		
		SELECT LPAD(NVL(MAX(TO_NUMBER(SUBSTR(B.KODE, 5,5))), 0)+1, 5, '0') INTO END_CODE
		FROM BAHAN B
		WHERE SUBSTR(B.KODE, 1, 4) = UPPER(START_CODE);

		:NEW.KODE := START_CODE || END_CODE;
		:NEW.PICTURE_LOCATION := START_CODE || END_CODE || '.jpg';

		if flag = 1 then
			:new.ID := ID;
			:new.STATUS := 1;
		end if;
	end if;
END;
/

CREATE OR REPLACE TRIGGER AUTOGEN_ID_KARYAWAN
BEFORE INSERT ON KARYAWAN
FOR EACH ROW
DECLARE
    ID varchar2(100);
	START_CODE varchar2(20);
    END_CODE varchar2(20);
BEGIN
    select to_char(max(to_number(ID))+1)
    into ID
    from VOUCHER;
    
    :new.ID := ID;
END;
/

CREATE OR REPLACE TRIGGER AUTOGEN_ID_USER_VOUCHER
BEFORE INSERT ON USER_VOUCHER
FOR EACH ROW
DECLARE
    ID varchar2(100);
BEGIN
    select to_char(max(to_number(ID))+1)
    into ID
    from USER_VOUCHER;
    
    :new.ID := ID;
    :NEW.KODE := START_CODE || END_CODE;
		:NEW.PICTURE_LOCATION := START_CODE || END_CODE || '.jpg';

		if flag = 1 then
			:new.ID := ID;
			:new.STATUS := 1;
		end if;
	end if;
END;
/

CREATE OR REPLACE TRIGGER AUTOGEN_ID_KARYAWAN
BEFORE INSERT ON KARYAWAN
FOR EACH ROW
DECLARE
    ID varchar2(100);
	START_CODE varchar2(20);
    END_CODE varchar2(20);
BEGIN
    select to_char(max(to_number(ID))+1)
    into ID
    from KARYAWAN;

    IF INSTR(:NEW.NAMA, ' ') > 0 THEN
        START_CODE := SUBSTR(:NEW.NAMA, 1, 2) || SUBSTR(:NEW.NAMA, INSTR(:NEW.NAMA, ' ') + 1, 2);
    ELSE
        START_CODE := SUBSTR(:NEW.NAMA, 1, 4);
    END IF;
	
    SELECT LPAD(NVL(MAX(TO_NUMBER(SUBSTR(K.KODE, 5,5))), 0)+1, 5, '0') INTO END_CODE
    FROM KARYAWAN K
    WHERE SUBSTR(K.KODE, 1, 4) = UPPER(START_CODE);


    :NEW.KODE := UPPER(START_CODE || END_CODE);
	:NEW.PICTURE_LOCATION := UPPER(START_CODE || END_CODE) ||  '.jpg';

    :new.ID := ID;
END;
/

CREATE OR REPLACE TRIGGER UPDATE_ID_ROTI
BEFORE UPDATE ON ROTI
FOR EACH ROW
DECLARE
    ID varchar2(100);
	START_CODE varchar2(20);
    END_CODE varchar2(20);
	pragma autonomous_transaction;
BEGIN
    IF INSTR(:NEW.NAMA, ' ') > 0 THEN
        START_CODE := SUBSTR(:NEW.NAMA, 1, 2) || SUBSTR(:NEW.NAMA, INSTR(:NEW.NAMA, ' ') + 1, 2);
    ELSE
        START_CODE := SUBSTR(:NEW.NAMA, 1, 4);
    END IF;

    if(START_CODE != SUBSTR(:OLD.KODE,1,4)) then
        select to_char(max(to_number(ID))+1)
        into ID
        from ROTI;

        SELECT LPAD(NVL(MAX(TO_NUMBER(SUBSTR(R.KODE, 5,5))), 0)+1, 5, '0') INTO END_CODE
        FROM ROTI R
        WHERE SUBSTR(R.KODE, 1, 4) = UPPER(START_CODE);

        :NEW.KODE := UPPER(START_CODE || END_CODE);
        :NEW.PICTURE_LOCATION := UPPER(START_CODE || END_CODE) ||  '.jpg';
    else
        :NEW.KODE := :OLD.KODE;
        :NEW.PICTURE_LOCATION := :OLD.KODE ||  '.jpg';
    end if;
END;
/

-- Trigger d_trans
CREATE OR REPLACE TRIGGER D_TRANS_trigger
BEFORE insert on D_TRANS
for each row
DECLARE
    currstok number;
    err_no_stok exception;
    hargaNow number;
BEGIN
    select stok into currstok from ROTI where ID=:NEW.FK_ROTI;
    if (:NEW.QUANTITY > currstok) then
        raise err_no_stok;
    end if;
    select HARGA into hargaNow from ROTI where ID=:NEW.FK_ROTI;
    :NEW.HARGA := hargaNow;
    update ROTI set STOK = STOK-:NEW.QUANTITY where ID = :NEW.FK_ROTI;
EXCEPTION
    when err_no_stok then
        raise_application_error(-20001, 'Stok tidak mencukupi!');
END;
/

--Trigger h_trans
CREATE OR REPLACE TRIGGER H_TRANS_trigger
BEFORE insert on H_TRANS
for each row
DECLARE
BEGIN
    :NEW.TANGGAL_TRANS := sysdate;
    :NEW.STATUS := 0;
END;
/

--Func Autogen no nota
CREATE OR REPLACE FUNCTION NOMOR_NOTA_autogen
return varchar2
is
    newid varchar2(15);
    urutan varchar2(3);
begin
    newid := 'NOTA' || to_char(sysdate,'YYYYMMDD');
    select lpad(nvl(max(to_number(substr(NOMOR_NOTA, 13, 3))), 0)+1, 3, '0') into urutan
    from H_TRANS where NOMOR_NOTA like newid || '%';
    newid := newid || urutan;
    return newid;
end;
/


--Trigger Pelanggan
CREATE OR REPLACE TRIGGER PELANGGAN_trigger_bef_ins
BEFORE insert on PELANGGAN
for each row
DECLARE
    a varchar2(2);
    b varchar2(2);
    gab varchar2(4);
    newUrutan varchar2(5);
BEGIN
--     length(replace(:NEW.NAMA,' ',''))
    if (length(:NEW.NAMA) - length(replace(:NEW.NAMA,' ','')) > 0) then
        a := substr(:NEW.NAMA,1,2);
        b := substr(:NEW.NAMA,instr(:NEW.NAMA,' ')+1,2);
    else
        a := substr(:NEW.NAMA,1,2);
        b := substr(:NEW.NAMA,3,2);
    end if;
    gab := upper(a || b);
    select lpad(nvl(max(to_number(substr(KODE,-4,4))),0)+1,5,'0') into newUrutan from PELANGGAN where KODE like gab || '%';
--     select lpad(nvl(max(to_number(substr(KODE,-4,4))),0)+1,5,'0') from PELANGGAN where KODE like 'RATR' || '%';
    :NEW.KODE := gab || newUrutan;
END;
/

COMMIT;


