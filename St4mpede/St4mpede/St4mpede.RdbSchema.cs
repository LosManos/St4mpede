//		This file was generated  by St4mpede 2016-01-13 13:33:23

/* 
Choose server (LocalDB)\MSSQLLocalDB
Chose database C:\DATA\PROJEKT\ST4MPEDE\St4mpede\Test\ST4MPEDE.TEST\DATABASE\ST4MPEDE.MDF.

Writing database xml St4mpede.RdbSchema.xml.
Created xml:
<Database xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Tables>
    <Table>
      <Name>__RefactorLog</Name>
      <Include>false</Include>
      <Columns>
        <Column IsInPrimaryKey="true">
          <Name>OperationKey</Name>
          <DatabaseTypeName>uniqueidentifier</DatabaseTypeName>
        </Column>
      </Columns>
    </Table>
    <Table>
      <Name>Customer</Name>
      <Include>true</Include>
      <Columns>
        <Column IsInPrimaryKey="true">
          <Name>CustomerID</Name>
          <DatabaseTypeName>int</DatabaseTypeName>
        </Column>
        <Column IsInPrimaryKey="false">
          <Name>Name</Name>
          <DatabaseTypeName>varchar</DatabaseTypeName>
        </Column>
      </Columns>
    </Table>
    <Table>
      <Name>User</Name>
      <Include>true</Include>
      <Columns>
        <Column IsInPrimaryKey="true">
          <Name>UserId</Name>
          <DatabaseTypeName>int</DatabaseTypeName>
        </Column>
        <Column IsInPrimaryKey="false">
          <Name>UserName</Name>
          <DatabaseTypeName>nvarchar</DatabaseTypeName>
        </Column>
        <Column IsInPrimaryKey="false">
          <Name>HashedPassword</Name>
          <DatabaseTypeName>nchar</DatabaseTypeName>
        </Column>
        <Column IsInPrimaryKey="false">
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
//
