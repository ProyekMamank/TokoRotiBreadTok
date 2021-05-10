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
BEFORE INSERT ON BAHAN
FOR EACH ROW
DECLARE
    ID varchar2(100);
BEGIN
    select to_char(max(to_number(ID))+1)
    into ID
    from BAHAN;

    :new.ID := ID;
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