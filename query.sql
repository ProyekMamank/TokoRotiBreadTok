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
	:NEW.PICTURE_LOCATION := UPPER(START_CODE || END_CODE);

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

COMMIT;


