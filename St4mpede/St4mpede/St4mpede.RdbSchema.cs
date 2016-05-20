//		This file was generated  by St4mpede 2016-05-19 14:53:08

/* 
Choose server (LocalDB)\MSSQLLocalDB
Chose database C:\DATA\PROJEKT\ST4MPEDE\ST4MPEDE\ST4MPEDE\DATABASE\ST4MPEDE.MDF.

Writing database xml St4mpede.RdbSchema.xml.
Created xml:
<Database xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Tables>
    <Table>
      <Name>Customer</Name>
      <Include>true</Include>
      <Columns>
        <Column IsInPrimaryKey="true">
          <Name>CustomerId</Name>
          <DatabaseTypeName>int</DatabaseTypeName>
        </Column>
        <Column IsInPrimaryKey="false">
          <Name>Name</Name>
          <DatabaseTypeName>nvarchar</DatabaseTypeName>
        </Column>
      </Columns>
    </Table>
    <Table>
      <Name>Project</Name>
      <Include>true</Include>
      <Columns>
        <Column IsInPrimaryKey="true">
          <Name>ProjectId</Name>
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
          <DatabaseTypeName>varchar</DatabaseTypeName>
        </Column>
        <Column IsInPrimaryKey="false">
          <Name>HashedPassword</Name>
          <DatabaseTypeName>char</DatabaseTypeName>
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
//
