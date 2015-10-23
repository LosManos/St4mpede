


//		The file was generated  by St4mpede 2015-10-23 20:47:11


/* 
Chose server (LocalDB)\MSSQLLocalDB
Chose database C:\DATA\PROJEKT\ST4MPEDE\St4mpede\ST4MPEDE.TEST\DATABASE\ST4MPEDE.MDF.
Number of tables:3.
Tables parsed:__RefactorLog,Customer,User
Excluded tables are 1:__RefactorLog.
Included tables are 2:Customer, User.
Name:__RefactorLog, Include:False.
Name:Customer, Include:True.
Name:User, Include:True.
Table __RefactorLog:
Name=OperationKey,	DatabaseTypeName=uniqueidentifier
Table Customer:
Name=CustomerID,	DatabaseTypeName=int
Name=Name,	DatabaseTypeName=varchar
Table User:
Name=UserId,	DatabaseTypeName=int
Name=UserName,	DatabaseTypeName=nvarchar
Name=HashedPassword,	DatabaseTypeName=nchar
Name=LastLoggedOnDatetime,	DatabaseTypeName=datetime

Writing database xml Database.xml.
Created xml:
<Database xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Tables>
    <Table>
      <Name>__RefactorLog</Name>
      <Include>false</Include>
      <Columns>
        <Column>
          <Name>OperationKey</Name>
          <DatabaseTypeName>uniqueidentifier</DatabaseTypeName>
        </Column>
      </Columns>
    </Table>
    <Table>
      <Name>Customer</Name>
      <Include>true</Include>
      <Columns>
        <Column>
          <Name>CustomerID</Name>
          <DatabaseTypeName>int</DatabaseTypeName>
        </Column>
        <Column>
          <Name>Name</Name>
          <DatabaseTypeName>varchar</DatabaseTypeName>
        </Column>
      </Columns>
    </Table>
    <Table>
      <Name>User</Name>
      <Include>true</Include>
      <Columns>
        <Column>
          <Name>UserId</Name>
          <DatabaseTypeName>int</DatabaseTypeName>
        </Column>
        <Column>
          <Name>UserName</Name>
          <DatabaseTypeName>nvarchar</DatabaseTypeName>
        </Column>
        <Column>
          <Name>HashedPassword</Name>
          <DatabaseTypeName>nchar</DatabaseTypeName>
        </Column>
        <Column>
          <Name>LastLoggedOnDatetime</Name>
          <DatabaseTypeName>datetime</DatabaseTypeName>
        </Column>
      </Columns>
    </Table>
  </Tables>
</Database>
*/

//
//
//
//
//
//
//
