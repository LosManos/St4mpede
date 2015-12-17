# Readme for St4mpede

St4mpede is a T4 solution for creating a full data access layer from a relational database.

All intermediary steps create output [XML](//github.com/LosManos/St4mpede/blob/master/St4mpede/St4mpede/RdbSchema/St4mpede.RdbSchema.xml)s. This makes St4mpede extensible.  
For instance: One St4mpede project retrieves the schema from the database.It writes it down in an [XML](//github.com/LosManos/St4mpede/blob/master/St4mpede/St4mpede/RdbSchema/St4mpede.RdbSchema.xml).  
The next project, the POCO generator, does not know about the innards of Sqlserver but knows how to translate the XML above to an [XML describing classes](https://github.com/LosManos/St4mpede/blob/master/St4mpede/St4mpede/Poco/PocoGenerator.xml) before creating   [POCO](//github.com/LosManos/St4mpede/tree/master/TheDAL/Poco)s from it. 

It is still an alpha so right now it only creates the [POCOs](//github.com/LosManos/St4mpede/tree/master/TheDAL/Poco).

The idea is to point a [config](//github.com/LosManos/St4mpede/blob/master/St4mpede/St4mpede/St4mpede.config.xml) to a relational database. St4mpede then examines its schema and creates POCOs and the rest of the DAL for simple CRUD operations. It is in no way perfect for the big solution but it makes you land with your feet running.

The code is unit- and integration tested. Very uncommon with T4 solutions.  

## Moonshot

Point St4mpede to a relational database  
and it generates:  
* all POCOs.
* the whole data access layer.  
* a fat client user interface.  
* a REST API.  
* all the javascript POJOs required
* a web client user interface.

## Thanks to
http://earlz.net/view/2012/11/21/0346/how-to-unit-test-t4-code-generators  
http://www.luisrocha.net/2008/07/testing-text-template-transformation.html  
https://github.com/jbubriski/ST4bby  
https://github.com/netTiers/netTiers  
http://www.codesmithtools.com/freeware  

*eof*
