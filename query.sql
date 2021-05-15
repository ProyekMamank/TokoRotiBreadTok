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